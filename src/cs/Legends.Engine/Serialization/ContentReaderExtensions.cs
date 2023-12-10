using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Legends.Engine.Runtime;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;

namespace Legends.Engine.Content;

public static class ContentReaderExtensions
{
    private static int Indent;
    private static void Log(string message, params object[] args)
    {
        if(Indent > 0) Console.Write(new string(Enumerable.Repeat(' ', 2 * Indent).ToArray()));
        Console.WriteLine(message, args);
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