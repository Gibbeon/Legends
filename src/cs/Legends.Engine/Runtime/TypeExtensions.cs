using System;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Legends.Engine.Runtime;

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

