using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Legends.Engine.Runtime;
using Legends.Engine.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Legends.Engine.Content;

public static class ContentWriterExtensions
{
    public static void WriteArray(this ContentWriter writer, IEnumerable instance, Type type)
    {
        var derivedType = instance != null ? instance.GetType() : type;
        Type elementType;
        int count;

        if(derivedType.IsArray)
        {
            elementType = derivedType.GetElementType();
            count = ((Array)instance).Length;
        }
        else if(instance is ICollection collection)
        {
            var interfaceType = derivedType.GetInterfaces().FirstOrDefault(n => n.IsGenericType && n.GetGenericTypeDefinition() == typeof(ICollection<>));
            
            if(interfaceType != null)
            {
                elementType = interfaceType.GenericTypeArguments[0];
            }
            else
            {
                throw new NotSupportedException();
            }

            count = collection.Count;
        }
        else
        {
            throw new NotSupportedException(); 
        }

        writer.Write(derivedType.FullName); 
        writer.Write(elementType.FullName); 
        writer.Write(count);

        int logCounter = 0;

        if(elementType.IsPrimitive && derivedType.IsArray)
        {
            using(ContentLogger.LogBegin(writer.Seek(0, SeekOrigin.Current), "Write Complete Array {0}", elementType.Name))
            {
                foreach(var item in instance)
                {
                    byte[] data =  (byte[])typeof(BitConverter).GetMethods().First(n => n.Name == "GetBytes" && 
                            n.GetParameters()[0].ParameterType.IsAssignableFrom(elementType)).Invoke(null, new object[] { item });

                    logCounter += data.Length;

                    //Console.WriteLine("{0}", typeof(BitConverter).GetMethods().First(n => n.Name == "GetBytes" && n.GetParameters()[0].ParameterType.IsAssignableFrom(typeof(ushort))).GetSignature());
                    writer.Write(data);
                    
                }
                ContentLogger.LogEnd(" size of {0}", logCounter);
            }
        }
        else if(instance is IDictionary dictionary)
        {
             var dictionaryType = dictionary.GetType().GetInterfaces().SingleOrDefault(x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(IDictionary<,>));
                
            if(dictionaryType == null)
            {
                throw new NotSupportedException("IDictionary must implement generic interface, IDictionary<,>");
            }
            
            var enumerator = dictionary.GetEnumerator();
            while(enumerator.MoveNext())
            {
                    writer.WriteField(enumerator.Key, dictionaryType.GenericTypeArguments[0]);
                    writer.WriteField(enumerator.Value, dictionaryType.GenericTypeArguments[1]);            
            }
        }
        else
        {
            //Console.WriteLine();
            foreach(var element in instance)
            {
                using(ContentLogger.LogBegin(writer.Seek(0, SeekOrigin.Current), "[{0}] ", logCounter++))
                {
                    if(element == null) throw new NullReferenceException();
                    writer.WriteField(element, element.GetType()); 
                } 
            }
        }
    }

    public static void WriteField(this ContentWriter writer, object instance, Type type, DefaultValueAttribute defaultValueAttribute = null)
    {
        var derivedType = instance != null ? instance.GetType() : type;

        var defaultValue = !derivedType.IsValueType ? null : defaultValueAttribute?.Value ?? Activator.CreateInstance(derivedType);

        if(Equals(instance, defaultValue))
        {
            ContentLogger.LogEnd("value was default {0}", defaultValue ?? "null");
            writer.Write7BitEncodedInt(0);
            return;
        } 
            
        writer.Write7BitEncodedInt(1);

        //using(LogEntry ("Field: {0} of type {1}", derivedType.Name, type.Name))
        {
            var native = writer.GetType()
                            .GetAnyMethod("Write", derivedType);

            if(instance is IContentReadWrite readWrite)
            {
                ContentLogger.LogEnd("IContentReadWrite.Write() of type {0}", readWrite.GetType().Name);
                readWrite.Write(writer);
                return;
            }

            if(native != null)
            {
                ContentLogger.LogEnd("Invoke Method {0} {1}", native.GetSignature(), instance);

                native.InvokeAny(writer, instance);
                return;
            }

            if(instance is ICollection enumerable)
            {
                ContentLogger.LogEnd("ICollection.Count: [{1}] of type {0}", enumerable.GetType().Name, enumerable.Count);
                writer.WriteArray(enumerable, type);
                return;
            }

            if(derivedType.IsEnum)
            {
                ContentLogger.LogEnd("Enum of type {0}", derivedType.Name);
                typeof(ContentWriter).GetAnyMethod("Write", typeof(int)).InvokeAny(writer, (int)instance);
                return;
            }
            
            if(instance is IEnumerable)
            {
                ContentLogger.LogEnd("ICollection.Count: [{1}] of type {0}", instance.GetType().Name, ((Array)instance).Length);
                writer.WriteArray(instance as IEnumerable, type);
                return;
            }

            ContentLogger.LogEnd("");
            //LogEnd("typeof(object) of type {0}", derivedType.Name);
            writer.WriteObject(instance, type);
        }
    }

    public static void WriteObject(this ContentWriter writer, object instance, Type type)
    {     
        try
        {
            var derivedType = instance != null ? instance.GetType() : type;

            writer.Write7BitEncodedInt(instance == null ? 0 : 1);
            if(instance == null) return;

            bool isDynamic = DynamicClassLoader.Contains(derivedType.Assembly);
                    
            writer.Write7BitEncodedInt(isDynamic ? 1 : 0);
            writer.Write(derivedType.FullName);
            if(isDynamic) {
                writer.Write(DynamicClassLoader.GetAssetName(derivedType.Assembly));
            }

            if(derivedType.IsArray ||
                derivedType.GetInterfaces().Any(n => n == typeof(IEnumerable)))
            {
                writer.WriteArray(instance as IEnumerable, derivedType);
            }
            else
            {
                using(ContentLogger.Log(writer.Seek(0, SeekOrigin.Current), "Object: {0} of type {1} ({2})", derivedType.Name, type.Name, instance == null ? "null" : "not null"))
                {   
                    foreach(var member in ContentHelpers.GetContentMembers(derivedType))
                    {    
                        if(member is PropertyInfo property)
                        {
                            using(ContentLogger.LogBegin(writer.Seek(0, SeekOrigin.Current), ".{0} = '{1}' (property)\t", property.Name, property.GetValue(instance), property.GetCustomAttribute<DefaultValueAttribute>()))
                            {
                                writer.WriteField(property.GetValue(instance), property.PropertyType);
                            }
                        }
                        if(member is FieldInfo field)
                        {
                            using(ContentLogger.LogBegin(writer.Seek(0, SeekOrigin.Current), ".{0} = '{1}' (property)\t", field.Name, field.GetValue(instance), field.GetCustomAttribute<DefaultValueAttribute>()))
                            {
                                writer.WriteField(field.GetValue(instance), field.FieldType);
                            }
                        }
                    }   
                }
            }
        } 
        catch(Exception err)
        {
            ContentLogger.Log(writer.Seek(0, SeekOrigin.Current), "Error: {0}\n{1}", err.Message, err.StackTrace);
            throw;
        }  
    }
}