using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Legends.Engine.Runtime;
using Legends.Engine.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current), "derivedType.FullName={0}", derivedType.FullName);
        writer.Write(derivedType.FullName); 
        ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current), "elementType.FullName={0}", elementType.FullName);
        writer.Write(elementType.FullName); 
        ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current), "count={0}", count);
        writer.Write(count);

        int logCounter = 0;

        if(elementType.IsPrimitive && derivedType.IsArray)
        {
            using(ContentLogger.LogBegin(writer.Seek(0, SeekOrigin.Current), "Write Complete BinaryArray {0}", elementType.Name))
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
            using(ContentLogger.LogBegin(writer.Seek(0, SeekOrigin.Current), "Write Complete Dictionary {0}", elementType.Name))
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
        }
        else
        {
            using(ContentLogger.LogBegin(writer.Seek(0, SeekOrigin.Current), "Write IEnumerable of {0}", elementType.Name))
            {
                foreach(var element in instance)
                {
                    using(ContentLogger.LogBegin(writer.Seek(0, SeekOrigin.Current), "[{0}] of element={1} ", logCounter++, element))
                    {
                        if(element == null) throw new NullReferenceException();
                        writer.WriteField(element, element.GetType()); 
                    } 
                }
            }
        }
    }

    public static void WriteField(this ContentWriter writer, object instance, Type type, DefaultValueAttribute defaultValueAttribute = null)
    {
        // for nullable<T> types, when there's an underlying value GetType() does not return Nullable<T> but instead typeof(T)
        // when it is null, it returns Nullable<T>

        ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current), 
            "var derivedType = instance != null ? instance.GetType() : type;");
        var derivedType = instance != null ? instance.GetType() : type;

        ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current), 
            "var defaultValue = (!derivedType.IsValueType || (derivedType.IsGenericType && derivedType.GetGenericTypeDefinition() == typeof(Nullable<>))) ? null : defaultValueAttribute?.Value ?? Activator.CreateInstance(derivedType);");
        var defaultValue = (!derivedType.IsValueType || (derivedType.IsGenericType && derivedType.GetGenericTypeDefinition() == typeof(Nullable<>))) ? null : defaultValueAttribute?.Value ?? Activator.CreateInstance(derivedType);

        ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current), 
            "if(derivedType.IsGenericType && derivedType.GetGenericTypeDefinition() == typeof(Nullable<>))");
        if(derivedType.IsGenericType && derivedType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {            
            ContentLogger.LogEnd("Nullable<{0}> value was default {1}", Nullable.GetUnderlyingType(derivedType).Name, defaultValue ?? "null");
            writer.Write7BitEncodedInt(0);
            return;
        } 

        ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current), 
            "if(!(derivedType.IsGenericType && derivedType.GetGenericTypeDefinition() == typeof(Nullable<>)) && Equals(instance, defaultValue))");
        // if it the underlying type is nullable don't test for equality, always write the value        
        if(!(derivedType.IsGenericType && derivedType.GetGenericTypeDefinition() == typeof(Nullable<>)) && Equals(instance, defaultValue))
        {
            ContentLogger.LogEnd("value was default {0} of type {1}", defaultValue ?? "null", derivedType.Name);
            writer.Write7BitEncodedInt(0);
            return;
        } 
            
        ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current), 
            "writer.Write7BitEncodedInt(1); //since value was not default or null of type {0}", derivedType.Name);
        writer.Write7BitEncodedInt(1);

        var native = writer.GetType().GetAnyMethod("Write", derivedType);

        ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current),
            "if(instance[{0}] is IContentReadWrite readWrite) [{1}]", instance, instance is IContentReadWrite);
        if(instance is IContentReadWrite readWrite)
        {
            ContentLogger.LogEnd("IContentReadWrite.Write() of type {0}", readWrite.GetType().Name);
            readWrite.Write(writer);
            return;
        }
        ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current),
            "if(native[{0}] != null) [{1}]", native, native != null);
        if(native != null)
        {
            ContentLogger.LogEnd("Invoke Method {0} {1}", native.GetSignature(), instance);

            native.InvokeAny(writer, instance);
            return;
        }
        ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current),
            "if(instance is ICollection enumerable) [{0}]", instance is ICollection);
        if(instance is ICollection enumerable)
        {
            ContentLogger.LogEnd("ICollection.Count: [{1}] of type {0}", enumerable.GetType().Name, enumerable.Count);
            writer.WriteArray(enumerable, derivedType);
            return;
        }
        ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current),
            "if(derivedType.IsEnum) [{0}]", derivedType.IsEnum);
        if(derivedType.IsEnum)
        {
            ContentLogger.LogEnd("Enum of type {0}", derivedType.Name);
            typeof(ContentWriter).GetAnyMethod("Write", typeof(int)).InvokeAny(writer, (int)instance);
            return;
        }
        ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current),
            "if(instance is IEnumerable) [{0}]", instance is IEnumerable);
        if(instance is IEnumerable)
        {
            ContentLogger.LogEnd("IEnumerable.Count: [{1}] of type {0}", instance.GetType().Name, ((Array)instance).Length);
            writer.WriteArray(instance as IEnumerable, derivedType);
            return;
        }

        ContentLogger.LogEnd("");
        //LogEnd("typeof(object) of type {0}", derivedType.Name);
        
        ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current),
            "writer.WriteObject(instance[{0}], type[{1}]);", instance, derivedType);
        writer.WriteObject(instance, derivedType);
    }

    public static void WriteObject(this ContentWriter writer, object instance, Type type)
    {   
        ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current), 
            "WriteObject(instance[{0}], type=[{1}])", instance, type);
        try
        {
            ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current), 
                "writer.Write7BitEncodedInt(instance[{0}] == null ? 0 : 1);", instance);
            writer.Write7BitEncodedInt(instance == null ? 0 : 1);

            ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current), 
                "var derivedType = instance[{0}] != null ? instance.GetType()[{1}] : type[{2}];", instance, instance != null ? instance.GetType() : null, type);
            var derivedType = instance != null ? instance.GetType() : type;
            
            ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current),
                "if(instance[{0}] == null) [{1}]", instance, instance == null);
            if(instance == null) {
                ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current), 
                "return;");
                return;
            }

            ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current), 
                "bool isDynamic = DynamicClassLoader.Contains(derivedType.Assembly [{0}]);", derivedType.Assembly);
            bool isDynamic = DynamicClassLoader.Contains(derivedType.Assembly);

            ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current), 
                "writer.Write7BitEncodedInt(isDynamic[{0}] ? 1 : 0);", isDynamic);
            writer.Write7BitEncodedInt(isDynamic ? 1 : 0);

            ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current),
                "writer.Write(derivedType.FullName [{0}]);", derivedType.FullName);
            writer.Write(derivedType.FullName);

            ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current),
                "if(isDynamic [{0}])", isDynamic);
            if(isDynamic) {
                ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current),
                    "writer.Write(DynamicClassLoader.GetAssetName(derivedType.Assembly [{0}]));", derivedType.Assembly);
                writer.Write(DynamicClassLoader.GetAssetName(derivedType.Assembly));
            }

            //Console.WriteLine("{0}.IsArray={1} or is IsAssignableTo(IEnumerable)={2} test={3}", derivedType.Name, derivedType.IsArray, derivedType.IsAssignableFrom(typeof(IEnumerable)), instance is IEnumerable);
            ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current),
                "if(derivedType.IsArray [{0}] || derivedType.IsAssignableTo(typeof(IEnumerable)) [{1}] || instance is IEnumerable [{2}]", derivedType.IsArray, derivedType.IsAssignableTo(typeof(IEnumerable)), instance is IEnumerable);

            if(derivedType.IsArray || 
                derivedType.IsAssignableTo(typeof(IEnumerable)) ||
                instance is IEnumerable
                )
            {
                ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current),
                    "writer.WriteArray(instance as IEnumerable [{0}], derivedType [{1}]);", instance as IEnumerable, derivedType);
                writer.WriteArray(instance as IEnumerable, derivedType);
            }
            else
            {
                using(ContentLogger.Log(writer.Seek(0, SeekOrigin.Current), "Object: {0} of type {1} ({2})", derivedType.Name, type.Name, instance == null ? "null" : "not null"))
                {   
                    foreach(var member in ContentHelpers.GetContentMembers(derivedType))
                    {    
                        ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current),
                            "if(member[{0}] is PropertyInfo property) [{1}]", member, member is PropertyInfo);
                        if(member is PropertyInfo property)
                        {
                            using(ContentLogger.LogBegin(writer.Seek(0, SeekOrigin.Current), ".{0} = '{1}' (property) [{2}]\t", property.Name, property.GetValue(instance), property.PropertyType))
                            {
                                ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current),
                                    "writer.WriteField(property.GetValue(instance) [{0}], property.PropertyType [{1}], property.GetCustomAttribute<DefaultValueAttribute>() [{2}]);", property.GetValue(instance), property.PropertyType, property.GetCustomAttribute<DefaultValueAttribute>());
                                writer.WriteField(property.GetValue(instance), property.PropertyType, property.GetCustomAttribute<DefaultValueAttribute>());
                            }
                        }
                        else if(member is FieldInfo field)
                        {
                            ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current),
                                "if(member[{0}] is FieldInfo field) [{1}]", member, member is FieldInfo);

                            using(ContentLogger.LogBegin(writer.Seek(0, SeekOrigin.Current), ".{0} = '{1}' (field) [{2}]\t", field.Name, field.GetValue(instance), field.FieldType))
                            {
                                ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current),
                                    "writer.WriteField(field.GetValue(instance)[{0}], field.FieldType[{1}], field.GetCustomAttribute<DefaultValueAttribute>()[{2}]);", field.GetValue(instance), field.FieldType, field.GetCustomAttribute<DefaultValueAttribute>());
                                writer.WriteField(field.GetValue(instance), field.FieldType, field.GetCustomAttribute<DefaultValueAttribute>());
                            }
                        }
                        else
                        {
                            ContentLogger.Trace(writer.Seek(0, SeekOrigin.Current),
                                "member[{0}] is ignored", member);
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