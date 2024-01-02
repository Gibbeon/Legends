using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using Legends.Engine.Runtime;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Newtonsoft.Json;

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
        else if(derivedType.IsGenericType && instance is ICollection collection)
        {
            elementType = derivedType.GenericTypeArguments[0];
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

    public static void WriteField(this ContentWriter writer, object instance, Type type)
    {
        var derivedType = instance != null ? instance.GetType() : type;

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

            using(ContentLogger.Log(writer.Seek(0, SeekOrigin.Current), "Object: {0} of type {1} ({2})", derivedType.Name, type.Name, instance == null ? "null" : "not null"))
            {
                writer.Write7BitEncodedInt(instance == null ? 0 : 1);
                if(instance == null) return;
                
                writer.Write(derivedType.FullName);
                
                foreach(var member in ContentHelpers.GetContentMembers(derivedType))
                {
                    if(member is PropertyInfo property)
                    {
                        using(ContentLogger.LogBegin(writer.Seek(0, SeekOrigin.Current), ".{0} = '{1}' (property)\t", property.Name, property.GetValue(instance)))
                        {
                            writer.WriteField(property.GetValue(instance), property.PropertyType);
                        }
                    }
                    if(member is FieldInfo field)
                    {
                        using(ContentLogger.LogBegin(writer.Seek(0, SeekOrigin.Current), ".{0} = '{1}' (property)\t", field.Name, field.GetValue(instance)))
                        {
                            writer.WriteField(field.GetValue(instance), field.FieldType);
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