using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.CompilerServices;
using MonoGame.Extended;
using Legends.Engine.Graphics2D;
using System.Text.RegularExpressions;
using System.Text;

namespace Legends.Content.Pipline;

public static class ContentTypeWriterWriteExtensions
{
    public static void Write(this ContentWriter output, Size2 size2)
    {
        output.Write(size2.Width);
        output.Write(size2.Height);
    }

    public static void Write(this ContentWriter output, Asset asset)
    {
        output.Write(asset.Name);
    }
}

public static class ContentTypeWriterLogExtensions
{
    private static int Indent;
    
    private static int IndentSpaces = 2;

    public static bool OutputToConsole = true;

    public class LogContext : IDisposable
    {
        public LogContext()
        {
            Indent++;
        }
        public void Dispose()
        {
            Indent--;
        }
    }

    public static LogContext LogEntry<TType>(this ContentWriter output, string message, params object?[]? args)
    {
        output.Log<TType>(message, args);

        return new LogContext();
    }

    public static void Log<TType>(this ContentWriter output, string message, params object?[]? args)
    {
        if(OutputToConsole)
        {
            var pos = output.Seek(0, SeekOrigin.Current);
            Console.Write("{0,8} ", pos.ToString("D8"));

            if(Indent * IndentSpaces > 0)
            {
                Console.Write(new string(Enumerable.Repeat(' ', Indent * IndentSpaces).ToArray()));
            }
            Console.Write("{0}: ", typeof(TType).Name);
            Console.WriteLine(message, args);
        }
    }
}

public static class TypeExtensions
{    
        public static string GetSignature(this MethodInfo method, bool callable = false)
        {
            var firstParam = true;
            var sigBuilder = new StringBuilder();
            if (callable == false)
            {
                if (method.IsPublic)
                    sigBuilder.Append("public ");
                else if (method.IsPrivate)
                    sigBuilder.Append("private ");
                else if (method.IsAssembly)
                    sigBuilder.Append("internal ");
                if (method.IsFamily)
                    sigBuilder.Append("protected ");
                if (method.IsStatic)
                    sigBuilder.Append("static ");
                sigBuilder.Append(TypeName(method.ReturnType));
                sigBuilder.Append(' ');
            }
            sigBuilder.Append(method.Name);

            // Add method generics
            if(method.IsGenericMethod)
            {
                sigBuilder.Append('<');
                foreach(var g in method.GetGenericArguments())
                {
                    if (firstParam)
                        firstParam = false;
                    else
                        sigBuilder.Append(", ");
                    sigBuilder.Append(TypeName(g));
                }
                sigBuilder.Append('>');
            }
            sigBuilder.Append('(');
            firstParam = true;
            var secondParam = false;
            foreach (var param in method.GetParameters())
            {
                if (firstParam)
                {
                    firstParam = false;
                    if (method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false))
                    {
                        if (callable)
                        {
                            secondParam = true;
                            continue;
                        }
                        sigBuilder.Append("this ");
                    }
                }
                else if (secondParam == true)
                    secondParam = false;
                else
                    sigBuilder.Append(", ");
                if (param.ParameterType.IsByRef)
                    sigBuilder.Append("ref ");
                else if (param.IsOut)
                    sigBuilder.Append("out ");
                if (!callable)
                {
                    sigBuilder.Append(TypeName(param.ParameterType));
                    sigBuilder.Append(' ');
                }
                sigBuilder.Append(param.Name);
            }
            sigBuilder.Append(")");
            return sigBuilder.ToString();
        }

        public static string TypeName(Type type)
        {
            var nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType != null)
                return nullableType.Name + "?";

            if (!(type.IsGenericType && type.Name.Contains('`')))
                switch (type.Name)
                {
                    case "String": return "string";
                    case "Int32": return "int";
                    case "Decimal": return "decimal";
                    case "Object": return "object";
                    case "Void": return "void";
                    default:
                        {
                            return string.IsNullOrWhiteSpace(type.FullName) ? type.Name : type.FullName;
                        }
                }

            var sb = new StringBuilder(type.Name.Substring(0,
            type.Name.IndexOf('`'))
            );
            sb.Append('<');
            var first = true;
            foreach (var t in type.GetGenericArguments())
            {
                if (!first)
                    sb.Append(',');
                sb.Append(TypeName(t));
                first = false;
            }
            sb.Append('>');
            return sb.ToString();
        }

    private static string WildCardToRegular(string value) {
        return "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$"; 
    }

    public static IEnumerable<MethodInfo> GetExtensionMethods(this Type extendedType)
    {
        var isGenericTypeDefinition = extendedType.IsGenericType && extendedType.IsTypeDefinition;
        var query = from type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(n => n.GetTypes())
            where type.IsSealed && !type.IsGenericType && !type.IsNested
            from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            where method.IsDefined(typeof(ExtensionAttribute), false)
            where isGenericTypeDefinition
                ? method.GetParameters()[0].ParameterType.IsGenericType && method.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == extendedType
                : method.GetParameters()[0].ParameterType == extendedType
            select method;
        return query;
    }

    public static IEnumerable<MethodInfo> MatchMethodSignature(this IEnumerable<MethodInfo> methods, Type instanceType, params Type?[]? parameterTypes)
    {
        var instanceMethods = methods.Where(n =>    !n.IsStatic
                                                &&  n.GetParameters().Length == parameterTypes?.Length 
                                                &&  n.GetParameters().All( m => parameterTypes.Any(x => x != null && x.IsAssignableFrom(m.ParameterType))));

        var genericMethods  = methods.Where(n =>    !n.IsStatic
                                                &&  n.IsGenericMethod
                                                &&  n.GetParameters().Length == parameterTypes?.Length 
                                                &&  n.GetParameters().Where(p => !p.ParameterType.IsGenericMethodParameter).All( m => parameterTypes.Any(x =>x != null &&  x.IsAssignableTo(m.ParameterType))));

        var extensionMethods = methods.Where(n =>   n.IsStatic
                                                &&  n.IsDefined(typeof(ExtensionAttribute), false)
                                                &&  n.GetParameters().Length == parameterTypes?.Length + 1
                                                &&  n.GetParameters()[0].ParameterType.IsAssignableFrom(instanceType)
                                                &&  n.GetParameters().Skip(1).All( m => parameterTypes != null && parameterTypes.Any(x => x != null && x.IsAssignableTo(m.ParameterType))));

        return Enumerable.Concat(instanceMethods, Enumerable.Concat(extensionMethods, genericMethods));
                
    }

    public static IEnumerable<MethodInfo> MatchReturnSignature(this IEnumerable<MethodInfo> methods, Type? returnValue)
    {
        return returnValue == null ? methods.Where(n => n.ReturnParameter.ParameterType.IsAssignableTo(returnValue)) : methods;
    }

    public static IEnumerable<MethodInfo> MatchMethodName(this IEnumerable<MethodInfo> methods, string? methodName)
    {
        if(string.IsNullOrEmpty(methodName)) return methods;

        return methods.Where(n => Regex.IsMatch(n.Name, WildCardToRegular(methodName)));
    }

    public static MethodInfo? GetAnyMethod(this Type type, string? methodName, params Type?[]? parameterTypes)
    {
        return type.GetAllMethods()
                    .MatchMethodName(methodName)
                    .MatchMethodSignature(type, parameterTypes)
                    .OrderBy(n => !n.IsDefined(typeof(ExtensionAttribute)))
                    .FirstOrDefault()
                    .MakeGenericFromSignature(parameterTypes);
    }

    public static IEnumerable<MethodInfo> GetAllMethods(this Type type)
    {
        return Enumerable.Concat(type.GetMethods(), type.GetExtensionMethods());
    }

    public static MethodInfo? MakeGenericFromSignature(this MethodInfo? methodInfo, params Type?[]? parameterTypes)
    {
        if(methodInfo != null && methodInfo.IsGenericMethod && parameterTypes != null)
        {
            var signatureMap = methodInfo.GetParameters().Zip(parameterTypes).Where(n => n.First.ParameterType.IsGenericMethodParameter).DistinctBy(n => n.First.ParameterType.GenericParameterPosition).OrderBy(n => n.First.ParameterType.GenericParameterPosition);
            //var typeArray = (methodInfo.ReturnType.IsGenericMethodParameter ? Enumerable.Concat(new [] { methodInfo.ReturnParameter }, signatureMap) : signatureMap).DistinctBy(n => n.ParameterType.GenericParameterPosition).OrderBy(n => n.ParameterType.GenericParameterPosition).ToArray();
           
            return methodInfo.MakeGenericMethod(signatureMap.Select(n => n.Second == null ? typeof(object) : n.Second).ToArray());
        }

        return methodInfo;
    }

    public static object? InvokeAny(this MethodInfo method, object? instance, params object?[]? values)
    {
        if(!method.IsStatic)
        {
            return method.Invoke(instance, values);
        }
        else if(method.IsStatic && method.IsDefined(typeof(ExtensionAttribute)))
        {
            return method.Invoke(null, values != null ? Enumerable.Concat( new [] { instance }, values).ToArray() : new[] { instance });
        }

        throw new NotSupportedException();
    }
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
                    typeof(ContentWriter)?.GetAnyMethod("WriteObject", item != null ? item.GetType() : itemType)
                        ?.InvokeAny(output, item);
                }
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
                    typeof(ContentWriter)?.GetAnyMethod("WriteObject", item != null ? item.GetType() : itemType)
                        ?.InvokeAny(output, item);
                }  
            }                                
        }
        else
        {
            output.Log<TType>("WriteObject {0}", memberType.Name);
            typeof(ContentWriter)?.GetAnyMethod("WriteObject", memberType)?.InvokeAny(output, memberValue);
        }
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
            writer.GenericWriteBaseObject<TType>(output, value);

            IEnumerable<MemberInfo> members = Enumerable.Concat<MemberInfo>(
                typeof(TType).GetFields(
                    BindingFlags.DeclaredOnly |
                    BindingFlags.Public |  
                    BindingFlags.Instance),
                typeof(TType).GetProperties(
                    BindingFlags.DeclaredOnly |
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

    public static void GenericWriteBaseObject<TType>(this ContentTypeWriter writer, ContentWriter output, TType? value)
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
    }
}
