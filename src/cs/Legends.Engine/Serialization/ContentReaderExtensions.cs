using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;

using Legends.Engine.Graphics2D;
using MonoGame.Extended;
using Legends.Content.Pipline;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Legends.Engine.Serialization;

public static class ContentReaderExtensions
{
    private static int Indent;

    private static int IndentSpaces = 2;

    public static bool OutputToConsole = true;

    public static bool InitFile;

    public static void Log<TType>(this ContentReader input, string message, params object[] args)
    {
        if(OutputToConsole)
        {
            Console.Write("{0,8} ", input.BaseStream.Position.ToString("D8"));
            if(Indent * IndentSpaces > 0)
            {
                Console.Write(new string(Enumerable.Repeat(' ', Indent * IndentSpaces).ToArray()));
            }
            Console.Write("{0}: ", typeof(TType).Name);
            Console.WriteLine(message, args);
        }
    }

    //private static MethodInfo _readRawObject;
    private static MethodInfo[] _contentReaderReadMethods;

    static Size2 ReadSize2(this ContentReader input)
    {
        return new Size2(input.ReadSingle(), input.ReadSingle());
    }

    static Asset<TType> ReadAsset<TType>(this ContentReader input)
    {
        var asset = new Asset<TType>(input.ReadString());
        asset.Load(input.ContentManager);
        return asset;
    }

    static IEnumerable<MethodInfo> GetExtensionMethods(Type extendedType, Assembly assembly = default)
    {
        assembly = assembly ?? typeof(ContentReaderExtensions).Assembly;
        var isGenericTypeDefinition = extendedType.IsGenericType && extendedType.IsTypeDefinition;
        var query = from type in assembly.GetTypes()
            where type.IsSealed && !type.IsGenericType && !type.IsNested
            from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            where method.IsDefined(typeof(ExtensionAttribute), false)
            where isGenericTypeDefinition
                ? method.GetParameters()[0].ParameterType.IsGenericType && method.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == extendedType
                : method.GetParameters()[0].ParameterType == extendedType
            select method;
        return query;
    }

    public static object Create(Type type, params object[] objects)
    {
        foreach(var ctor in type.GetConstructors().Where(n => n.GetParameters().Length <= objects?.Length).OrderByDescending(n => n.GetParameters().Length))
        {
            var ctorParamList = ctor.GetParameters();
            var typeParams = new object[ctorParamList.Length]; 
            for(int i = 0; i < ctorParamList.Length; i++)
            {
                if(typeParams[i] == null)
                {
                    typeParams[i] = objects?.SingleOrDefault(n => n.GetType().IsAssignableTo( ctorParamList[i].ParameterType));
                }
            }

            if(typeParams.All(n => n != null))
            {
                return (object)Activator.CreateInstance(type, typeParams);
            }
        }

        return null;
    }

    public static TType Create<TType>(params object[] objects)
    {
        return (TType)Create(typeof(TType), objects);
    }

    public static void ReadFields(this ContentReader input, Type typeOf, object value)
    {
        typeof(ContentReaderExtensions).GetMethods()
            .Single(n => n.IsGenericMethod && n.Name == "ReadFields")
            .MakeGenericMethod(typeOf)
            .Invoke(null, new [] { input, value });
    }

    public static void ReadFields<TType>(this ContentReader input, TType value)
    {
        var fields = typeof(TType).GetFields(
            //BindingFlags.DeclaredOnly |
            BindingFlags.Public |  
            BindingFlags.Instance);

        var properties = typeof(TType).GetProperties(
            //BindingFlags.DeclaredOnly |
            BindingFlags.Public |  
            BindingFlags.Instance);

        _contentReaderReadMethods = _contentReaderReadMethods ?? Enumerable.Concat(input.GetType().GetMethods(
                                                                        BindingFlags.Public |  
                                                                        BindingFlags.Instance)
                                                                        .Where(n => n.Name.StartsWith("Read")),
                                                                        GetExtensionMethods(input.GetType()).Where(n => n.Name.StartsWith("Read"))).ToArray();

        foreach(var property in properties)
        {
            if(property.CustomAttributes.Any(n => n.AttributeType == typeof(JsonIgnoreAttribute)))
            {
                input.Log<TType>("!!Ignoring Property: {0}", property.Name);
                continue;
            }

            input.Log<TType>("Reading Property: {0}", property.Name);
            var readMethod = _contentReaderReadMethods.SingleOrDefault(
                    n => 
                    !n.IsStatic && n.ReturnType.IsAssignableTo(property.PropertyType)
                    || n.IsStatic && n.ReturnType.IsAssignableTo(property.PropertyType)
                    || property.PropertyType.IsGenericType && n.ReturnType.IsGenericType && n.ReturnType.GetGenericTypeDefinition().MakeGenericType(property.PropertyType.GenericTypeArguments).IsAssignableTo(property.PropertyType));

            if(readMethod != null)
            {
                if(readMethod.Name == "ReadObject")
                {
                    input.Read7BitEncodedInt();
                }

                if(readMethod.IsGenericMethod && readMethod.IsStatic)
                {
                    var genericValue = readMethod.MakeGenericMethod(property.PropertyType.GenericTypeArguments).Invoke(null, new object[] { input });
                    property.SetValue(value, genericValue);
                }
                else if(!readMethod.IsStatic)
                {
                    var instanceReadValue = readMethod.Invoke(input, null);
                    property.SetValue(value, instanceReadValue);
                } 
                else
                {
                    var staticReadValue = readMethod.Invoke(null, new object[] { input });
                    property.SetValue(value, staticReadValue);
                }

                input.Log<TType>("..Ordinal: {0} = {1}", property.PropertyType.Name, property.GetValue(value));
            }
            else if(property.PropertyType.IsEnum)
            {
                property.SetValue(value, Enum.Parse(property.PropertyType, input.ReadString()));
                
                input.Log<TType>("..Enum: {0} = {1}", property.PropertyType.Name, property.GetValue(value));
            }
            else if(property.PropertyType.IsArray || property.PropertyType.GetInterface(typeof(IEnumerable).Name) != null)
            {  
                int count = input.ReadInt32();
                
                input.Log<TType>("..Array [{0}] of Size {1} ", property.PropertyType.Name, count);

                var itemType = property.PropertyType.IsArray ? property.PropertyType.GetElementType() :
                    property.PropertyType.GenericTypeArguments[0];

                var list = property.GetValue( value );

                if(list == null)
                {
                    if(property.PropertyType.IsArray)
                    {
                        list = Array.CreateInstance(itemType, count);
                    }
                    else
                    {
                        list = Activator.CreateInstance(property.PropertyType);
                    }

                    property.SetValue(value, list);
                }

                for(int i = 0; i < count; i++)
                {
                    readMethod = _contentReaderReadMethods.SingleOrDefault(
                                n => !n.IsStatic && n.ReturnType.IsAssignableTo(itemType)
                                || n.IsStatic && n.ReturnType.IsAssignableTo(itemType));

                    if(readMethod == null)
                    {
                        readMethod = _contentReaderReadMethods.Single(n => n.Name == "ReadObject" && n.IsGenericMethod && n.GetParameters().Length == 0).MakeGenericMethod(itemType);
                    }

                    object itemValue = default;

                    if(readMethod.Name == "ReadObject")
                    {
                        if(input.Read7BitEncodedInt() == (int)ObjectType.Dynamic) // output.Write7BitEncodedInt((int)ObjectType.Dynamic);
                        {
                            var codeIdentifier = input.ReadString(); //output.Write(dynamicValue.Source);

                            input.Log<TType>("..Reading Dynamic of Type {0} from {1}", itemType.Name, codeIdentifier);
                            var len = input.ReadInt32(); //output.Write(DynamicClassLoader.GetBytes(dynamicValue.Source).Length);
                            var bytes = input.ReadBytes(len); //output.Write(DynamicClassLoader.GetBytes(dynamicValue.Source));
                            var slen = input.ReadInt32(); //output.Write(DynamicClassLoader.GetBytes(dynamicValue.Source).Length);
                            var sbytes = input.ReadBytes(slen); //output.Write(DynamicClassLoader.GetBytes(dynamicValue.Source));
                            var typeName = input.ReadString();

                            input.Log<TType>("..Dynamic Type Found {0}", typeName);

                            var dynamicType = DynamicClassLoader.LoadAndExtractClass(codeIdentifier, bytes,sbytes, typeName);
                            object[] paramConstructors;

                            if(GenericReaderStack.ParentObjects.Count > 0)
                            {
                                paramConstructors = new object[] { input.ContentManager.ServiceProvider, GenericReaderStack.ParentObjects.Peek() };
                            }
                            else
                            {
                                paramConstructors = new object[] { input.ContentManager.ServiceProvider };
                            }

                            itemValue = ContentReaderExtensions.Create(dynamicType, paramConstructors);

                            if(itemValue == null) throw new NotSupportedException();
                            
                            GenericReaderStack.ParentObjects.Push(itemValue);

                            input.ReadFields(itemValue.GetType(), itemValue);

                            GenericReaderStack.ParentObjects.Pop();
                        }
                        else
                        {
                            itemValue = readMethod.Invoke(input, null);
                        }
                    } 
                    else
                    {
                        itemValue = readMethod.Invoke(input, null);
                    }

                    if(list.GetType().IsArray)
                    {
                        ((Array)list).SetValue(itemValue, i);
                    }
                    else
                    {
                        var addMethod = typeof(ICollection<>).MakeGenericType(itemType).GetMethod("Add");
                        addMethod.Invoke(list, new object[] { itemValue });
                    }
                    
                    input.Log<TType>("..Ordinal [{0}]: {1}", i, itemValue?.ToString());
                }
            }
            else
            {
                readMethod = _contentReaderReadMethods.Single(n => n.Name == "ReadObject" && n.IsGenericMethod && n.GetParameters().Length == 0).MakeGenericMethod(property.PropertyType);
                

                if(input.Read7BitEncodedInt() == (int)ObjectType.Dynamic) // output.Write7BitEncodedInt((int)ObjectType.Dynamic);
                {
                    var codeIdentifier = input.ReadString(); //output.Write(dynamicValue.Source);

                    input.Log<TType>("..Reading Dynamic of Type {0} from {1}", property.PropertyType.Name, input.ReadString());
                    var len = input.ReadInt32(); //output.Write(DynamicClassLoader.GetBytes(dynamicValue.Source).Length);
                    var bytes = input.ReadBytes(len); //output.Write(DynamicClassLoader.GetBytes(dynamicValue.Source));
                    var slen = input.ReadInt32(); //output.Write(DynamicClassLoader.GetBytes(dynamicValue.Source).Length);
                    var sbytes = input.ReadBytes(slen); //output.Write(DynamicClassLoader.GetBytes(dynamicValue.Source));
                    var type = input.ReadString();

                    var dynamicType = DynamicClassLoader.LoadAndExtractClass(codeIdentifier, bytes, sbytes, type);
                    object[] paramConstructors;

                    if(GenericReaderStack.ParentObjects.Count > 0)
                    {
                        paramConstructors = new object[] { input.ContentManager.ServiceProvider, GenericReaderStack.ParentObjects.Peek() };
                    }
                    else
                    {
                        paramConstructors = new object[] { input.ContentManager.ServiceProvider };
                    }

                    var result = ContentReaderExtensions.Create(dynamicType, paramConstructors);

                    if(result == null) throw new NotSupportedException();
                    
                    GenericReaderStack.ParentObjects.Push(result);

                    input.ReadFields(result);

                    GenericReaderStack.ParentObjects.Pop();

                    
                    //object objValue = dynamicValue.Properties;
                }
                else
                {
                    var readObject = readMethod.Invoke(input, null);
                    property.SetValue(value, readObject);
                    input.Log<TType>("..Reading Object of Type {0} = {1}", property.PropertyType.Name, readObject);
                }
            }
        }
    }
}

    /*public static void ReadBaseObject<TType>(this ContentReader input, TType result)
    {

        if(typeof(TType).BaseType != null && typeof(TType).BaseType != typeof(object))
        {        
            input.Log<TType>("Reading Base Type: {0}", typeof(TType).BaseType.Name);
            Indent++;
            
            _readRawObject = _readRawObject ?? 
                typeof(ContentReader).GetMethods().Single(n => n.Name == "ReadRawObject" && n.IsGenericMethod && n.GetParameters().Length == 1 && n.GetParameters()[0].ParameterType.IsGenericParameter);
            _readRawObject.MakeGenericMethod(typeof(TType).BaseType)?.Invoke(input, new object[] { result });
            //input.ReadRawObject<SceneObject>(result);
            
            Indent--;
        }
    }*/
