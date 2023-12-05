using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System;
using Newtonsoft.Json;
using Legends.Engine.Runtime;
using Legends.Engine.Serialization;
using Legends.Engine;
using System.Runtime.InteropServices.ObjectiveC;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Legends.Content.Pipline;

public enum ObjectType
{
    Static,
    Dynamic
}

public static class ContentTypeWriterExtensions
{   
    public static void GenericWriteValue<TType>(this ContentTypeWriter writer, ContentWriter output, Type memberType, object? memberValue)
    {
        var nativeWriteMethod = typeof(ContentWriter).GetAnyMethod("Write", memberType);
                
        if(nativeWriteMethod != null)
        {
            output.Log<TType>("Write Native");
            nativeWriteMethod.InvokeAny(output, memberValue);
        }

        else if(memberType.IsEnum)
        {
            output.Log<TType>("Write Enum as String [{0}]", Enum.GetName(memberType, memberValue == null ? 0 : memberValue));
            typeof(ContentWriter)?.GetAnyMethod("Write", typeof(string))?.InvokeAny(output, Enum.GetName(memberType, memberValue == null ? 0 : memberValue));
        }
        else if(memberType.IsArray)
        {
            var itemType = memberType.GetElementType();
            if(itemType == null) itemType = typeof(object);

            output.Write(memberValue == null ? 0 : ((Array)memberValue).Length);            
            output.Log<TType>("Write Array Size Of [{0}]", memberValue == null ? 0 : ((Array)memberValue).Length);

            if(memberValue != null)
            {
                foreach(var item in (Array)memberValue)
                {
                    writer.GenericWriteValue<TType>(output, item != null ? item.GetType() : itemType, item);
                }
            }
        }
        else if(memberType.GetInterfaces().Any(n => n == typeof(ICollection)))
        {
            var itemType =  typeof(object);
            var memberCollectionValue = (memberValue as ICollection);
            if(memberCollectionValue == null)
            {
                output.Log<TType>("Type implements ICollection but value was NULL");
                output.Write(0);
            }
            else
            {
                output.Log<TType>("Write ICollection Size Of [{0}]", memberCollectionValue.Count);
                output.Write(memberCollectionValue.Count);
                foreach(var item in memberCollectionValue)
                {
                    writer.GenericWriteValue<TType>(output, item != null ? item.GetType() : itemType, item);

                    //typeof(ContentWriter)?.GetAnyMethod("WriteObject", item != null ? item.GetType() : itemType)
                    //    ?.InvokeAny(output, item);
                }
            }                                  
        } 
        else if(memberType.GetInterfaces().Any(n => n == typeof(IDynamicallyCompiledType)))
        {
            if(memberValue is IDynamicallyCompiledType dynamicValue)
            {
                output.Write7BitEncodedInt((int)ObjectType.Dynamic);
                output.Write(dynamicValue.Source);
                output.Write(DynamicClassLoader.GetBytes(dynamicValue.Source).Length);
                output.Write(DynamicClassLoader.GetBytes(dynamicValue.Source));
                output.Write(dynamicValue.TypeName);
                object objValue = dynamicValue.Properties;

                writer.GenericWriteObject(output, objValue.GetType(), objValue);
            }
            else
            {
                output.Log<TType>("IDynamicallyCompiledType [{0}] but value was not castable, Null?", memberType.Name);
            }
        }
        else if(memberType.GetInterfaces().Any(n => n.IsGenericType && n.GetGenericTypeDefinition() == (typeof(ICollection<>))))
        {
            var memberCollectionType = memberType.GetInterfaces().Single(n => n.IsGenericType && n.GetGenericTypeDefinition() == (typeof(ICollection<>)));
            var itemType = memberCollectionType.GenericTypeArguments[0];

            if(memberValue == null)
            {
                output.Log<TType>("Type implements ICollection<> but value was NULL");
                output.Write(0);
            }
            else
            {
                var count = (int?)memberValue.GetType()?.GetProperty("Count")?.GetValue(memberValue);
                
                output.Log<TType>("Write {0} Size Of [{1}]", memberValue.GetType().Name, count);

                output.Write(count == null ? 0 : (int)count);
                foreach(var item in ((IEnumerable)memberValue))
                {
                    writer.GenericWriteValue<TType>(output, item != null ? item.GetType() : itemType, item);

                    //typeof(ContentWriter)?.GetAnyMethod("WriteObject", item != null ? item.GetType() : itemType)
                    //    ?.InvokeAny(output, item);
                }  
            }                                
        }
        else
        {
            output.Log<TType>("WriteObject {0}", memberType.Name);
            output.Write7BitEncodedInt((int)ObjectType.Static);
            typeof(ContentWriter)?.GetAnyMethod("WriteObject", memberType)?.InvokeAny(output, memberValue);
        }
    }

    public static void GenericWriteObject(this ContentTypeWriter writer, ContentWriter output, Type type, object value)
    {
        typeof(ContentTypeWriterExtensions)
            .GetMethods()
            .Single(n => n.IsStatic && n.IsGenericMethod && n.Name == "GenericWriteObject")
            .MakeGenericMethod(type)
            .Invoke(null, new object[] { writer, output, value });
    }

    public static void GenericWriteObject<TType>(this ContentTypeWriter writer, ContentWriter output, TType? value)
    {
        if(value == null)
        {
            output.Log<TType>("Skip Write [value is null]");
            return;
        }

        using(output.LogEntry<TType>("Write Object of Type [{0}]", value.GetType().Name))
        {           
            //writer.GenericWriteBaseObject<TType>(output, value);

            IEnumerable<MemberInfo> members = Enumerable.Concat<MemberInfo>(
                typeof(TType).GetFields(
                    //BindingFlags.DeclaredOnly |
                    BindingFlags.Public |  
                    BindingFlags.Instance),
                typeof(TType).GetProperties(
                    //BindingFlags.DeclaredOnly |
                    BindingFlags.Public |  
                    BindingFlags.Instance));

            foreach(var member in members)
            {
                var property    = member as PropertyInfo;
                var field       = member as FieldInfo;

                if(field == null && property == null) throw new InvalidOperationException();

                var memberDesc  = property != null ? "Property" : "Field";
                var memberType  = property != null ? property.PropertyType : field != null ? field.FieldType : typeof(object);

                Func<object?, object?>               funcGetValue        = (instance) =>            property != null ? property?.GetValue(instance) : field?.GetValue(instance);
                //Func<object?, object, object?>       funcGetValueIndexed = (instance, index) =>     property != null ? property?.GetValue(instance, new [] { index }) : ((Array?)field?.GetValue(instance))?.GetValue((int)index);

                if(member.GetCustomAttribute<JsonIgnoreAttribute>(true) != null)
                {
                    output.Log<TType>("Skip {0} {1} {2}", memberDesc, memberType.Name, member.Name);
                    continue;
                } 

                var memberValue = funcGetValue(value);

                using(output.LogEntry<TType>("{0} {1} {2} => {3}", memberDesc, memberType.Name, member.Name, memberValue))
                {                    
                    writer.GenericWriteValue<TType>(output, memberType, memberValue);
                }
            }
        }
    }

    /*public static void GenericWriteBaseObject<TType>(this ContentTypeWriter writer, ContentWriter output, TType? value)
    {
        if(value== null) throw new NotSupportedException();

        if(typeof(TType).BaseType != null && typeof(TType).BaseType != typeof(Object))
        {
            using(output.LogEntry<TType>("Write BaseClass of Type {0}", typeof(TType).BaseType?.Name))
            {                 
                typeof(ContentWriter)?.GetAnyMethod("WriteRawObject", typeof(TType).BaseType)
                    ?.InvokeAny(output, value);
            }
        }
    }*/
}
