using System;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Autofac.Core.Activators.Reflection;

namespace Legends.Engine.Runtime;

public static class TypeExtensions
{    
        public static string GetSignature(this MethodBase method, bool callable = false)
        {
            if(method == null) return "(no method found)";
            
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
                if (method is MethodInfo mi)
                    sigBuilder.Append(TypeName(mi.ReturnType));

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

    private static Dictionary<Type, IEnumerable<MethodInfo>> _extensionCache = new();
    private static IEnumerable<MethodInfo> _allExtensionMethods;

    public static IEnumerable<MethodInfo> GetExtensionMethods(this Type extendedType)
    {        
        if(_extensionCache.TryGetValue(extendedType, out IEnumerable<MethodInfo> result)) return result;

        _allExtensionMethods ??= from type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(n => n.GetTypes())
            where   type.IsSealed && !type.IsGenericType && !type.IsNested
            from    method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            where   method.IsDefined(typeof(ExtensionAttribute), false)
            select  method;

        var isGenericTypeDefinition = extendedType.IsGenericType && extendedType.IsTypeDefinition;
        var query = _allExtensionMethods.Where(method => 
                isGenericTypeDefinition
                ? method.GetParameters()[0].ParameterType.IsGenericType && method.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == extendedType
                : method.GetParameters()[0].ParameterType == extendedType);

        _extensionCache.Add(extendedType, query);                
        return query;
    }

    public static IEnumerable<MethodInfo> MatchMethodSignature(this IEnumerable<MethodInfo> methods, Type instanceType, params Type[] parameterTypes)
    {
        var instanceMethods = methods.Where(n =>    !n.IsStatic
                                                &&  n.GetParameters().Length == parameterTypes?.Length 
                                                &&  n.GetParameters().All( m => parameterTypes.Any(x => x != null && x.IsAssignableTo(m.ParameterType))));

        var genericMethods  = methods.Where(n =>    !n.IsStatic
                                                &&  n.IsGenericMethod
                                                &&  n.GetParameters().Length == parameterTypes?.Length 
                                                &&  n.GetParameters().Where(p => !p.ParameterType.IsGenericMethodParameter).All( m => parameterTypes.Any(x =>x != null &&  x.IsAssignableTo(m.ParameterType))));

        var extensionMethods = methods.Where(n =>   n.IsStatic
                                                &&  n.IsDefined(typeof(ExtensionAttribute), false)
                                                &&  n.GetParameters().Length == parameterTypes?.Length + 1
                                                &&  n.GetParameters()[0].ParameterType.IsAssignableFrom(instanceType)
                                                &&  n.GetParameters().Skip(1)
                                                                     .All(m => parameterTypes != null 
                                                                            && parameterTypes.Any(x => 
                                                                                                        x.IsAssignableTo(m.ParameterType)
                                                                                                    ||  (n.IsGenericMethod 
                                                                                                        && m.ParameterType.IsGenericMethodParameter)
                                                                                                    ||  (n.IsGenericMethod
                                                                                                        && m.ParameterType.IsGenericType)
                                                                                                        
                                                                                                    
                                                )));

        return Enumerable.Concat(instanceMethods, Enumerable.Concat(extensionMethods, genericMethods));
                
    }

    public static IEnumerable<MethodInfo> MatchReturnSignature(this IEnumerable<MethodInfo> methods, Type returnValue)
    {
        return returnValue == null ? methods.Where(n => n.ReturnParameter.ParameterType.IsAssignableTo(returnValue)) : methods;
    }

    public static IEnumerable<MethodInfo> MatchMethodName(this IEnumerable<MethodInfo> methods, string methodName)
    {
        if(string.IsNullOrEmpty(methodName)) return methods;

        return methods.Where(n => Regex.IsMatch(n.Name, WildCardToRegular(methodName)));
    }

    private static Dictionary<string, MethodInfo> _methodCache = new();

    private static string Hash(Type returnType, string methodName, params Type[] parameterTypes)
    {
        return (returnType ?? typeof(void)).Name 
        + "_" + methodName 
        + "_" + (parameterTypes != null ? string.Join('_', parameterTypes.Select(n => n.Name)) : "");
    }
    
    public static MethodInfo GetAnyMethod(this Type type, string methodName, params Type[] parameterTypes)
    {
        var hash = Hash(typeof(void), methodName, parameterTypes);

        if(_methodCache.TryGetValue(hash, out MethodInfo result)) return result.MakeGenericFromSignature(parameterTypes);

        result = type.GetAllMethods()
                    .MatchMethodName(methodName)
                    .MatchMethodSignature(type, parameterTypes)
                    .OrderBy(n => !n.IsDefined(typeof(ExtensionAttribute)))
                    .FirstOrDefault();
        
        _methodCache.Add(hash, result);

        return result.MakeGenericFromSignature(parameterTypes);
    }

    public static MethodInfo GetAnyMethod(this Type type, Type returnType, string methodName, params Type[] parameterTypes)
    {
        var hash = Hash(returnType, methodName, parameterTypes);

        if(_methodCache.TryGetValue(hash, out MethodInfo result)) return result.MakeGenericFromSignature(parameterTypes);

        result = type.GetAllMethods()
                    .MatchMethodName(methodName)
                    .MatchMethodSignature(type, parameterTypes)
                    .OrderBy(n => !n.IsDefined(typeof(ExtensionAttribute)))
                    .FirstOrDefault(n => n.ReturnParameter.ParameterType.IsAssignableFrom(returnType));
        
        _methodCache.Add(hash, result);

        return result.MakeGenericFromSignature(parameterTypes);
    }

    public static IEnumerable<MethodInfo> GetAllMethods(this Type type)
    {
        return Enumerable.Concat(type.GetMethods(), type.GetExtensionMethods());
    }

    public static MethodInfo MakeGenericFromSignature(this MethodInfo methodInfo, params Type[] parameterTypes)
    {
        if(methodInfo != null && methodInfo.IsGenericMethod && parameterTypes != null)
        {
            var signatureMap = methodInfo
                                    .GetParameters()
                                    .Skip(methodInfo.IsDefined(typeof(ExtensionAttribute)) ? 1 : 0)
                                    .Zip(parameterTypes.SelectMany( n => !n.IsGenericType ? new Type[] { n } : n.GetGenericArguments()))
                                    .Where(n => n.First.ParameterType.IsGenericMethodParameter 
                                            ||  n.First.ParameterType.IsGenericType 
                                                && n.First.ParameterType.GetGenericArguments()[0].IsGenericMethodParameter)
                                    ;

            return methodInfo.MakeGenericMethod(signatureMap.Select(n => n.Second == null ? typeof(object) : n.Second).ToArray());
        }

        return methodInfo;
    }

    public static object InvokeAny(this MethodInfo method, object instance, params object[] values)
    {
        try
        {
            if(!method.IsStatic)
            {
                return method.Invoke(instance, values);
            }
            else if(method.IsStatic && method.IsDefined(typeof(ExtensionAttribute)))
            {
                return method.Invoke(null, values != null ? Enumerable.Concat( new [] { instance }, values).ToArray() : new[] { instance });
            }
        } 
        catch(Exception error)
        {
            //Console.WriteLine("instance {0}", instance);
            //Console.WriteLine("valuesCount {0}", values == null ? -1 : values.Length);
            //foreach(var value in values) Console.WriteLine("value {0}", value);
            //Console.WriteLine("InvokeAny failed to invoke the desired method {0}", method.GetSignature());
            throw error.InnerException;
        }

        throw new NotSupportedException();
    }

    public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
    {
        var interfaceTypes = givenType.GetInterfaces();

        foreach (var it in interfaceTypes)
        {
            if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                return true;
        }

        if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            return true;

        Type baseType = givenType.BaseType;
        if (baseType == null) return false;

        return IsAssignableToGenericType(baseType, genericType);
    }

    public static IEnumerable<ConstructorInfo> MatchConstructorSignature(this IEnumerable<ConstructorInfo> methods, params Type[] parameterTypes)
    {
        /*return methods.Where(n =>   n.GetParameters().Length > 0
                                &&  n.GetParameters().All( m => 
                                        parameterTypes.Any(x => 
                                                x != null 
                                            &&  x.IsAssignableTo(m.ParameterType))
                                        ||  m.Attributes.HasFlag(ParameterAttributes.Optional)));
        */

        foreach(var ctor in methods.Where(n => 
            /*min*/ n.GetParameters().Count(m => !m.Attributes.HasFlag(ParameterAttributes.Optional)) <= parameterTypes.Length &&
            /*max*/ n.GetParameters().Length >= parameterTypes.Length))
        {
            var matches = 0;

            //Console.WriteLine(ctor.GetSignature());

            for(; matches < parameterTypes.Length; matches++)
            {
                // all the required parameters match
                if(parameterTypes[matches] != typeof(void) && !parameterTypes[matches].IsAssignableTo(ctor.GetParameters()[matches].ParameterType))
                {
                    //Console.WriteLine("{0}.IsAssignableTo({1}) = false", parameterTypes[matches], ctor.GetParameters()[matches].ParameterType);
                    break;
                }
                
                //Console.WriteLine("{0}.IsAssignableTo({1}) = true", parameterTypes[matches], ctor.GetParameters()[matches].ParameterType);
            }

            // if you skip the matches do all the rest have the optional flag
            if(!ctor.GetParameters().Skip(matches).All(m => m.Attributes.HasFlag(ParameterAttributes.Optional)))
                break;

            yield return ctor;
        }
    }

    public static object Create(this Type type, params object[] parameters)
    {
        var result = CreateOrDefault(type, parameters);
        if(result == null) throw new NoConstructorsFoundException(type);
        return result;
        //return Activator.CreateInstance(type);
    }

    public static object CreateOrDefault(this Type type, params object[] parameters)
    {
        if(type.GetConstructors().Length == 0 && parameters.Length == 0) return Activator.CreateInstance(type);

        if(type.GetConstructors()
            .MatchConstructorSignature(parameters.Select(n => n == null ? typeof(void) : n.GetType()).ToArray())
            .SingleOrDefault() is ConstructorInfo ctor)
        {
            return ctor.Invoke(
                    Enumerable.Concat(parameters, 
                    ctor.GetParameters().Skip(parameters.Length).Select(n => Type.Missing)
                    ).Take(ctor.GetParameters().Length).ToArray());
        }

        return null;
    }
}

