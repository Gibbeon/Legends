using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using Legends.Engine.Runtime;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Newtonsoft.Json;

namespace Legends.Engine.Content;

public static class ContentWriterExtensions
{
    public static void WriteArray(this ContentWriter writer, ICollection instance, Type type)
    {
        var derivedType = instance != null ? instance.GetType() : type;
        var elementType = typeof(object);

        if(derivedType.IsArray)                 elementType = derivedType.GetElementType();
        else if(derivedType.IsGenericType)      elementType = derivedType.GenericTypeArguments[0];

        //using(LogEntry("Array: {0}<{1}>[{2}]", derivedType.Name, elementType.Name, instance.Count))
        {
            writer.Write(derivedType.FullName); 
            writer.Write(elementType.FullName); 
            writer.Write(instance.Count);

            int count = 0;

            //Console.WriteLine();
            foreach(var element in instance)
            {
                using(ContentLogger.LogBegin(writer.Seek(0, SeekOrigin.Current), "[{0}] ", count++))
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

            if(native != null)
            {
                ContentLogger.LogEnd("Invoke Method", native.GetSignature());
                native.InvokeAny(writer, instance);
                return;
            }

            if(instance is ICollection enumerable)
            {
                ContentLogger.LogEnd("ICollection.Count: [{1}] of type {0}", enumerable.GetType().Name, enumerable.Count);
                writer.WriteArray(enumerable, type);
                return;
            }

            if(instance is IContentReadWrite readWrite)
            {
                ContentLogger.LogEnd("IContentReadWrite.Write() of type {0}", readWrite.GetType().Name);
                readWrite.Write(writer);
                return;
            }

            if(derivedType.IsEnum)
            {
                ContentLogger.LogEnd("Enum of type {0}", derivedType.Name);
                typeof(ContentWriter).GetAnyMethod("Write", typeof(int)).InvokeAny(writer, (int)instance);
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
                foreach(var member in Enumerable.Concat<MemberInfo>(
                    derivedType .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty)
                                .Where(n => n.CanRead && n.CanWrite),
                    derivedType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                    .Where(n => !n.IsDefined(typeof(JsonIgnoreAttribute))))
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