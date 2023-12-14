using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Legends.Engine.Runtime;
using Legends.Engine.Serialization;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;

namespace Legends.Engine.Content;

public static class ContentReaderExtensions
{
    private static readonly Stack<object> _parents = new();

    public static object ReadArray(this ContentReader reader, ICollection instance)
    {
        var typeName = reader.ReadString();
        var typeElementName = reader.ReadString();
        var count       = reader.ReadInt32(); 

        var type        = Type.GetType(typeName);
        var elementType = Type.GetType(typeElementName); 

        if(type == null)
        {
            throw new NullReferenceException(string.Format("ICollection Type not found, {0}.", typeName));
        } 
        if(type == null)
        {
            throw new NullReferenceException(string.Format("ICollection.ElementType not found, {0}.", typeElementName));
        }          

        instance ??=    type.IsArray
                        ? Array.CreateInstance(elementType, count)
                        : type.Create() as ICollection;

        if(type.IsArray && elementType.IsPrimitive)
        {
            int elementSize = Marshal.SizeOf(elementType);
            using(ContentLogger.LogBegin(reader.BaseStream.Position, "Read elementSize: {0} bytesSize: {1}", elementSize, count * elementSize))
            {
                Buffer.BlockCopy(reader.ReadBytes(count * elementSize), 0, (Array)instance, 0, count);
                ContentLogger.LogEnd(" (BlockCopy)");
            }
        }
        else
        {
            for(var index = 0; index < count; index++)
            {
                using(ContentLogger.LogBegin(reader.BaseStream.Position, "[{0}] ", index))
                {
                    var element = reader.ReadField(null, elementType);
                    if(element == null) throw new NullReferenceException();

                    if(type.IsArray) ((Array)instance).SetValue(element, index);
                    else if(instance is IList list) { list.Add(element); }
                    else type.GetType().GetAnyMethod("Add*", elementType).InvokeAny(instance, element);
                }
            }
        }

        return instance;
    }

    public static object ReadField(this ContentReader reader, object instance, Type type)
    {
        var derivedType = instance != null ? instance.GetType() : type;
        var native = typeof(ContentReader).GetAnyMethod(derivedType, "Read*");
        object result = default;

        if(native != null)
        {
            ContentLogger.LogAppend("(invoke)", native.GetSignature()); 
            var value = native.InvokeAny(reader);               
            result = Convert.ChangeType(value, derivedType); 
            if(result is IRef) return result;
            
            ContentLogger.LogEnd("{0}", result);  
            return result;         
        }

        else if(derivedType.GetInterfaces().Any(n => n == typeof(ICollection)))
        {
            ContentLogger.LogAppend("(array)", derivedType.Name);
            result =  reader.ReadArray(instance as ICollection);
            ContentLogger.LogEnd("{0}", result);
            return result;
        }

        else if(derivedType.GetInterfaces().Any(n => n == typeof(IContentReadWrite)))
        {
            ContentLogger.LogAppend("(IContentReadWrite)");

            instance ??= derivedType.Create();

            derivedType.GetAnyMethod("Read", reader.GetType()).InvokeAny(instance, reader);
            result = instance;
            ContentLogger.LogEnd("{0}", result);
            return result;
        }

        else if(derivedType.IsEnum)
        {
            ContentLogger.LogAppend("(enum)", derivedType.Name);         
            var intValue = reader.ReadInt32();
            //((int)typeof(ContentReader).GetAnyMethod(typeof(int), "Read").InvokeAny(reader));   
            result = Enum.Parse(derivedType,intValue.ToString());
            ContentLogger.LogEnd("{0}", result);
            return result;
        }
        ContentLogger.LogEnd("(object)", derivedType.Name); 
        return reader.ReadComplexObject(instance, derivedType);
    }

    public static object ReadComplexObject(this ContentReader reader, object instance, Type type)
    {
        using(ContentLogger.LogBegin(reader.BaseStream.Seek(0, SeekOrigin.Current), "Reading Object of type {0}", type == null ? "(unknown type)" : type?.Name))
        {
            var isNull = reader.Read7BitEncodedInt() == 0;
            if(isNull)
            {               
                ContentLogger.LogEnd("value is (null)");
                return null;
            }
        }
        ContentLogger.LogEnd("");

        var typeName = reader.ReadString();
        var derivedType = Type.GetType(typeName) ?? DynamicClassLoader.GetType(typeName) ?? type;

        using(ContentLogger.Log(reader.BaseStream.Seek(0, SeekOrigin.Current), "Object found of type {0}, existing instance is ({1})", derivedType.Name, instance == null ? "null" : "not null"))
        {
            instance ??=  derivedType.Create(_parents.Count == 0  
                ? new[] { reader.ContentManager.ServiceProvider }
                : new[] { reader.ContentManager.ServiceProvider, _parents.Peek() });

            _parents.Push(instance);

            try{
                foreach(var member in Enumerable.Concat<MemberInfo>(
                    derivedType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty),
                    derivedType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                    .Where(n => !n.IsDefined(typeof(JsonIgnoreAttribute))))
                {
                    if(member is PropertyInfo property)
                    {
                        using(ContentLogger.LogBegin(reader.BaseStream.Seek(0, SeekOrigin.Current), ".{0} = ", property.Name))
                        {
                            property.SetValue(instance, reader.ReadField(property.GetValue(instance), property.PropertyType));
                        }
                    }
                    if(member is FieldInfo field)
                    {
                        using(ContentLogger.LogBegin(reader.BaseStream.Seek(0, SeekOrigin.Current), ".{0} = ", field.Name))
                        {
                            field.SetValue(instance, reader.ReadField(field.GetValue(instance), field.FieldType));
                        }
                    }
                }   

                return instance;
            }
            catch(Exception err)
            {
                ContentLogger.Log(reader.BaseStream.Seek(0, SeekOrigin.Current), "{0}\n{1}", err.Message, err.StackTrace);
                throw;
            }
            finally{
                _parents.Pop();
            }
        }
    }
}