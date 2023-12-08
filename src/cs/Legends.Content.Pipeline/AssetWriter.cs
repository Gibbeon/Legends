using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Legends.Content.Pipeline;
using Legends.Engine.Runtime;
using Legends.Engine.Serialization;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended.Content;
using Newtonsoft.Json;

namespace Legends.Engine.Content;

public static class ContentExtensions
{
    public static void WriteArray(this ContentWriter writer, ICollection instance, Type type)
    {
        writer.Write(type.AssemblyQualifiedName); 

        if(type.IsArray)        writer.Write(type.GetElementType().AssemblyQualifiedName);
        else if(type.IsGenericType)  writer.Write(type.GenericTypeArguments[0].AssemblyQualifiedName);
        else writer.Write(typeof(object).AssemblyQualifiedName);

        writer.Write(instance.Count);

        foreach(var element in instance)
        {
            writer.WriteField(element, element.GetType());
        }
    }

    public static void WriteField(this ContentWriter writer, object instance, Type type)
    {
        var native = typeof(ContentWriter).GetAnyMethod("Write", type);

        if(native != null)
        {
            native.InvokeAny(writer, instance);
            return;
        }

        if(instance is ICollection enumerable)
        {
            writer.WriteArray(enumerable, type);
            return;
        }

        writer.WriteObject(instance, type);
    }

    public static void WriteObject(this ContentWriter writer, object instance, Type type)
    {        
        try
        {
            writer.Write(type.AssemblyQualifiedName); 

            foreach(var member in Enumerable.Concat<MemberInfo>(
                type.GetProperties(BindingFlags.Public | BindingFlags.Instance),
                type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                .Where(n => !n.IsDefined(typeof(JsonIgnoreAttribute))))
            {
                if(member is PropertyInfo property)
                {
                    writer.WriteField(property.GetValue(instance), property.PropertyType);
                }
                if(member is FieldInfo field)
                {
                    writer.WriteField(field.GetValue(instance), field.FieldType);
                }
            }   
        } 
        catch(Exception err)
        {
            Console.WriteLine("{0}\n{1}", err.Message, err.StackTrace);
        }  
    }

    private static readonly Stack<object> _parents = new Stack<object>();

    public static object ReadArray(this ContentReader reader, ICollection instance)
    {
        var type        = Type.GetType(reader.ReadString());
        var elementType = Type.GetType(reader.ReadString());
        var count       = reader.ReadInt32();

        instance ??=    type.IsArray
                        ? Array.CreateInstance(elementType, count)
                        : type.Create() as ICollection;

        for(var index = 0; index < count; index++)
        {
            var element = reader.ReadField(null, elementType);

            if(type.IsArray) ((Array)instance).SetValue(element, index);
            else if(instance is IList list) { list.Add(element); }
            else type.GetType().GetAnyMethod("Add*", elementType).InvokeAny(instance, element);
        }

        return instance;
    }

    public static object ReadField(this ContentReader reader, object instance, Type type)
    {
        var native = typeof(ContentReader)
                        .GetAllMethods()
                        .SingleOrDefault(n =>   !n.IsGenericMethod 
                                                && n.Name.StartsWith("Read") 
                                                && n.ReturnParameter.ParameterType == type);

        if(native != null)
        {
            return Convert.ChangeType(native.InvokeAny(reader), type);            
        }

        if(type.GetInterfaces().Any(n => n == typeof(ICollection)))
        {
            return reader.ReadArray(instance as ICollection);
        }

        return reader.ReadObject(instance);
    }

    public static object ReadObject(this ContentReader reader, object instance)
    {
        var type = Type.GetType(reader.ReadString());

        instance ??=  type.Create(_parents.Count == 0  
            ? new[] { reader.ContentManager.ServiceProvider }
            : new[] { reader.ContentManager.ServiceProvider, _parents.Peek() });

        _parents.Push(instance);
        try{
            foreach(var member in Enumerable.Concat<MemberInfo>(
                type.GetProperties(BindingFlags.Public | BindingFlags.Instance),
                type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                .Where(n => !n.IsDefined(typeof(JsonIgnoreAttribute))))
            {
                if(member is PropertyInfo property)
                {
                    property.SetValue(instance, reader.ReadField(property.GetValue(instance), property.PropertyType));
                }
                if(member is FieldInfo field)
                {
                    field.SetValue(instance, reader.ReadField(field.GetValue(instance), field.FieldType));
                }
            }   

            return instance;
        }
        catch(Exception err)
        {
            Console.WriteLine("{0}\n{1}", err.Message, err.StackTrace);
            throw;
        }
        finally{
            _parents.Pop();
        }
    }
}

[ContentTypeWriter]
public class SceneObjectWriter : ContentTypeWriter<SceneLike>
{
    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return typeof(SceneObjectReader).AssemblyQualifiedName;
    }

    protected override void Write(ContentWriter output, SceneLike value)
    {
        output.WriteObject(value, value.GetType());
    }
}

public class SceneObjectReader : ContentTypeReader<Asset<SceneLike>>
{
    protected override Asset<SceneLike> Read(ContentReader input, Asset<SceneLike> existingInstance)
    {
        return input.ReadObject(existingInstance);       
    }
}

public class AssetLoader<TType> : IContentLoader<Asset<TType>>
{
    public Asset<TType> Load(ContentManager contentManager, string path)
    {
        return contentManager.Load<Asset<TType>>(path);
    }
}