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
    private static int Indent;
    private static void Log(string message, params object[] args)
    {
        if(Indent > 0) Console.Write(new string(Enumerable.Repeat(' ', 2 * Indent).ToArray()));
        Console.WriteLine(message, args);
    }

    public static void WriteArray(this ContentWriter writer, ICollection instance, Type type)
    {
        var derivedType = instance != null ? instance.GetType() : type;
        var elementType = typeof(object);

        if(derivedType.IsArray)                 elementType = derivedType.GetElementType();
        else if(derivedType.IsGenericType)      elementType = derivedType.GenericTypeArguments[0];

        Log("Array: {0}<{1}>[{2}]", derivedType.Name, elementType.Name, instance.Count);

        writer.Write(derivedType.AssemblyQualifiedName); 
        writer.Write(elementType.AssemblyQualifiedName); 
        writer.Write(instance.Count);

        int count = 0;

        foreach(var element in instance)
        {
            Log("[{0}]", count++);
            writer.WriteField(element, element.GetType());
        }
    }

    public static void WriteField(this ContentWriter writer, object instance, Type type)
    {
        var derivedType = instance != null ? instance.GetType() : type;

        Log("WriteField: {0} of type {1}", derivedType.Name, type.Name); 
        Indent++;

        var native = typeof(ContentWriter).GetAnyMethod("Write", derivedType);

        if(native != null)
        {
            Log("native Found [{0}]", native.GetSignature());
            native.InvokeAny(writer, instance);
            Indent--;
            return;
        }

        if(instance is ICollection enumerable)
        {
            Log("typeof(ICollection) Found {0}", enumerable.GetType().Name);
            writer.WriteArray(enumerable, type);
            Indent--;
            return;
        }

        if(instance is IContentReadWrite readWrite)
        {
            Log("typeof(IContentReadWrite) Found {0}", readWrite.GetType().Name);
            readWrite.Write(writer);
            Indent--;
            return;
        }

        Log("typeof(object) Found {0}", derivedType.Name);
        writer.WriteObject(instance, type);
        Indent--;
    }

    public static void WriteObject(this ContentWriter writer, object instance, Type type)
    {     
          
        try
        {
            var derivedType = instance != null ? instance.GetType() : type;

            Log("WriteObject: {0} of type {1}", derivedType.Name, type.Name); Indent++; 
            writer.Write(derivedType.AssemblyQualifiedName); 

            foreach(var member in Enumerable.Concat<MemberInfo>(
                derivedType .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty)
                            .Where(n => n.CanRead && n.CanWrite),
                derivedType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                .Where(n => !n.IsDefined(typeof(JsonIgnoreAttribute))))
            {
                if(member is PropertyInfo property)
                {
                    Log(".{0} = '{1}' (field)", property.Name, property.GetValue(instance)); Indent++;
                    writer.WriteField(property.GetValue(instance), property.PropertyType); Indent--;
                }
                if(member is FieldInfo field)
                {
                    Log(".{0} = '{1}' (property)", field.Name, field.GetValue(instance)); Indent++;
                    writer.WriteField(field.GetValue(instance), field.FieldType); Indent--;
                }
            }   
        } 
        catch(Exception err)
        {
            Log("Error: {0}\n{1}", err.Message, err.StackTrace);
        }  
        Indent--;
    }

    private static readonly Stack<object> _parents = new();

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
public class SceneObjectWriter : ContentTypeWriter<IContentObject>
{
    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return typeof(ContentObjectReader).AssemblyQualifiedName;
    }

    protected override void Write(ContentWriter output, IContentObject value)
    {
        output.WriteObject(value, value.GetType());
    }
}

public class ContentObjectReader : ContentTypeReader<IContentObject>
{
    protected override IContentObject Read(ContentReader input, IContentObject existingInstance)
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