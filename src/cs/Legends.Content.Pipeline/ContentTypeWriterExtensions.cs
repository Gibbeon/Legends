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

namespace Legends.Content.Pipline;

public static class ContentTypeWriterExtensions
{
    private static MethodInfo? _writeRawObjectMethod;
    private static MethodInfo? _writeObjectMethod;
    private static MethodInfo[]? _contentWriterWriteMethods;
    
    private static int Indent;
    private static int IndentSpaces = 2;

    public static bool OutputToConsole = true;
    public static bool OutputToFile = true;

    public static bool InitFile;


    public static void Log<TType>(string message, params object?[]? args)
    {
        if(!InitFile)
        {
            InitFile = true;
            if(File.Exists("pipeline.log"))
                File.Delete("pipeline.log");
        }
        if(OutputToConsole)
        {
            if(Indent * IndentSpaces > 0)
            {
                Console.Write(new string(Enumerable.Repeat(' ', Indent * IndentSpaces).ToArray()));
            }
            Console.Write("{0}: ", typeof(TType).Name);
            Console.WriteLine(message, args);
        }

        if(OutputToFile)
        {
            using(var sw = new StreamWriter(File.OpenWrite("pipeline.log")))
            {
                if(Indent * IndentSpaces > 0)
                {
                    sw.Write(new string(Enumerable.Repeat(' ', Indent * IndentSpaces).ToArray()));
                }

                sw.Write("{0}: ", typeof(TType).Name);
                sw.WriteLine(message, args);

                sw.Flush();
                sw.Close();
            }
        }
    }

    public static void WriteAll<TType>(this ContentWriter output, TType? value)
    {
        output.WriteBaseObject<TType>(value);
        output.WriteFields<TType>(value);
    }
    public static void WriteBaseObject<TType>(this ContentWriter output, TType? value)
    {
        if(typeof(TType).BaseType != null && typeof(TType).BaseType != typeof(Object))
        {
            Log<TType>("Writing Base Type: {0}", typeof(TType).BaseType.Name);
            try
            {
                Indent++;
                _writeRawObjectMethod = _writeRawObjectMethod ?? typeof(ContentWriter).GetMethods().Single(n => n.Name == "WriteRawObject" && n.GetParameters().Length == 1);
                _writeRawObjectMethod.MakeGenericMethod(typeof(TType).BaseType).Invoke(output, new object[] { value });
                Indent--;
            } 
            catch(Exception)
            {
                throw;
            }
            finally
            {
                Indent--;
            }
        }
    }

    public static void Write(this ContentWriter output, Size2 size2)
    {
        output.Write(size2.Width);
        output.Write(size2.Height);
    }

    static IEnumerable<MethodInfo> GetExtensionMethods(Type extendedType, Assembly? assembly = default)
    {
        assembly = assembly ?? typeof(ContentTypeWriterExtensions).Assembly;
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

    public static void WriteFields<TType>(this ContentWriter output, TType? value)
    {
        try
        {
            Indent++;
        
            var fields = typeof(TType).GetFields(
                BindingFlags.DeclaredOnly |
                BindingFlags.Public |  
                BindingFlags.Instance);

            var properties = typeof(TType).GetProperties(BindingFlags.DeclaredOnly |
                BindingFlags.Public |  
                BindingFlags.Instance);

            _contentWriterWriteMethods = _contentWriterWriteMethods ?? Enumerable.Concat(output.GetType().GetMethods(
                                                                            BindingFlags.Public |  
                                                                            BindingFlags.Instance)
                                                                            .Where(n => n.Name == "Write"),
                                                                            GetExtensionMethods(output.GetType()).Where(n => n.Name == "Write")).ToArray();


            //Console.WriteLine("typeof(float) == typeof(Single) = {0}", typeof(float) == typeof(Single));
            //Console.WriteLine("typeof(float) == typeof(Single) = {0}", typeof(float) == typeof(Single));

            _writeObjectMethod = _writeObjectMethod ?? typeof(ContentWriter).GetMethods().Single(n => n.Name == "WriteObject" && n.GetParameters().Length == 1);

            foreach(var field in fields)
            {
                if(field.CustomAttributes.Any(n => n.AttributeType == typeof(JsonIgnoreAttribute)))
                {
                    Log<TType>("!!Ignoring Field: {0}", field.Name);
                    continue;
                }
                Log<TType>("Writing Field: {0}", field.Name);

                var writeMethod = _contentWriterWriteMethods.SingleOrDefault(
                    n => !n.IsStatic && n.GetParameters().All(m => m.ParameterType.IsAssignableTo(field.FieldType))
                        || n.IsStatic && n.GetParameters().Any(m => m.ParameterType.IsAssignableTo(field.FieldType)));
                if(writeMethod != null)
                {
                    Log<TType>("..Writing Ordinal Value: {0}", field.GetValue(value)?.ToString());
                    writeMethod.Invoke(output, new object?[] { field.GetValue(value) });
                }
                else if(field.FieldType.IsArray || field.FieldType.GetInterface(typeof(IEnumerable).Name) != null)
                {
                    if(field.GetValue(value) is IEnumerable list)
                    {
                        int count = 0;
                        var enumerator = list.GetEnumerator();
                        while(enumerator.MoveNext()) ++count;
                        output.Write(count);
                        
                        Log<TType>("..Array [{0}] of Size {1} ", field.FieldType.Name, count);
                        count = 0;
                        
                        foreach(var item in list)
                        {
                            writeMethod = _contentWriterWriteMethods.SingleOrDefault(n => n.GetParameters().All(m => m.ParameterType == item.GetType()));
                            if(writeMethod != null)
                            {
                                Log<TType>("..Ordinal [{0}]: {1}", count++, item?.ToString());
                                writeMethod.Invoke(output, new object[] { item });
                            }
                            else 
                            {
                                Log<TType>("..Object [{0}] of Type {1}", count++, item.GetType().Name);
                                _writeObjectMethod.MakeGenericMethod(item.GetType()).Invoke(output, new object?[] { item });
                            }
                        }
                    }
                }
                else
                {
                    Log<TType>("..Writing Object of Type {0}", field.FieldType.Name);
                    _writeObjectMethod.MakeGenericMethod(field.FieldType).Invoke(output, new object?[] { field.GetValue(value) });
                }
            }

            foreach(var property in properties)
            {
                if(property.CustomAttributes.Any(n => n.AttributeType == typeof(JsonIgnoreAttribute)))
                {
                    Log<TType>("!!Ignoring Property: {0}", property.Name);
                    continue;
                }

                Log<TType>("Writing Property: {0}", property.Name);

                var writeMethod = _contentWriterWriteMethods.SingleOrDefault(
                    n => !n.IsStatic && n.GetParameters().All(m => m.ParameterType.IsAssignableTo(property.PropertyType))
                        || n.IsStatic && n.GetParameters().Any(m => m.ParameterType.IsAssignableTo(property.PropertyType)));
                        
                if(writeMethod != null)
                {
                    Log<TType>("..Ordinal: {0}", property.GetValue(value)?.ToString());
                    if(!writeMethod.IsStatic)
                    {
                        writeMethod.Invoke(output, new object?[] { property.GetValue(value) });
                    } 
                    else
                    {
                        writeMethod.Invoke(null, new object?[] { output, property.GetValue(value)});
                    }
                }
                else if(property.PropertyType.IsArray || property.PropertyType.GetInterface(typeof(IEnumerable).Name) != null)
                {   
                    if(property.GetValue(value) is IEnumerable list)
                    {
                        int count = 0;
                        var enumerator = list.GetEnumerator();
                        while(enumerator.MoveNext()) ++count;
                        output.Write(count);

                        Log<TType>("..Array [{0}] of Size {1} ", property.PropertyType.Name, count);

                        count = 0;
                        
                        foreach(var item in list)
                        {
                            writeMethod = _contentWriterWriteMethods.SingleOrDefault(
                                                n => !n.IsStatic && n.GetParameters().All(m => m.ParameterType.IsAssignableTo(item.GetType()))
                                                    || n.IsStatic && n.GetParameters().Any(m => m.ParameterType.IsAssignableTo(item.GetType())));

                            if(writeMethod != null)
                            {
                                Log<TType>("..Ordinal [{0}]: {1}", count++, item?.ToString());
                                if(!writeMethod.IsStatic)
                                {
                                    writeMethod.Invoke(output, new object?[] { item });
                                } 
                                else
                                {
                                    writeMethod.Invoke(null, new object?[] { output, item });
                                }
                            }
                            else 
                            {
                                Log<TType>("..Object [{0}] of Type {1}", count++, item.GetType().Name);
                                Indent++;
                                _writeObjectMethod.MakeGenericMethod(item.GetType()).Invoke(output, new object?[] { item });
                                Indent--;
                            }
                        }
                    }
                    else
                    {
                        Log<TType>("ERROR -- Array but not Enumerable?");
                    }
                }
                else
                {
                    Log<TType>("..Writing Object of Type {0}", property.PropertyType.Name);
                    Indent++;
                    _writeObjectMethod.MakeGenericMethod(property.PropertyType).Invoke(output, new object?[] { property.GetValue(value) });
                    Indent--;
                }
            }
        }
        catch
        {
            throw;
        }
        finally
        {
            Indent--;
        }
    }
}
