using System.Linq;
using System.Reflection;
using System.Collections;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Legends.Content.Pipline;

public static class ContentTypeWriterExtensions
{
    private static MethodInfo? _writeRawObjectMethod;
    private static MethodInfo? _writeObjectMethod;
    private static MethodInfo[]? _contentWriterWriteMethods;

    public static void WriteAll<TType>(this ContentWriter output, TType? value)
    {
        output.WriteBaseObject<TType>(value);
        output.WriteFields<TType>(value);
    }
    public static void WriteBaseObject<TType>(this ContentWriter output, TType? value)
    {
        if(typeof(TType).BaseType != null && value != null)
        {
            _writeRawObjectMethod = _writeRawObjectMethod ?? typeof(ContentWriter).GetMethods().Single(n => n.Name == "WriteRawObject" && n.GetParameters().Length == 1);
            _writeRawObjectMethod.MakeGenericMethod(typeof(TType).BaseType).Invoke(output, new object[] { value });
        }
    }

    public static void WriteFields<TType>(this ContentWriter output, TType? value)
    {
        var fields = typeof(TType).GetFields(
            BindingFlags.DeclaredOnly |
            BindingFlags.Public |  
            BindingFlags.Instance);

        _contentWriterWriteMethods = _contentWriterWriteMethods ?? output.GetType().GetMethods(BindingFlags.DeclaredOnly |
                                                                        BindingFlags.Public |  
                                                                        BindingFlags.Instance)
                                                                        .Where(n => n.Name == "Write").ToArray();

        _writeObjectMethod = _writeObjectMethod ?? typeof(ContentWriter).GetMethods().Single(n => n.Name == "WriteObject" && n.GetParameters().Length == 1);

        foreach(var field in fields)
        {
            var writeMethod = _contentWriterWriteMethods.SingleOrDefault(n => n.GetParameters().All(m => m.ParameterType == field.FieldType));
            if(writeMethod != null)
            {
                writeMethod.Invoke(output, new object[] { field.GetValue(value) });
            }
            else if(field.FieldType.IsArray || field.FieldType.GetInterface(typeof(IEnumerable).Name) != null)
            {
                if(field.GetValue(value) is IEnumerable list)
                {
                    int count = 0;
                    var enumerator = list.GetEnumerator();
                    while(enumerator.MoveNext()) ++count;
                    output.Write(count);
                    
                    foreach(var item in list)
                    {
                        writeMethod = _contentWriterWriteMethods.SingleOrDefault(n => n.GetParameters().All(m => m.ParameterType == item.GetType()));
                        if(writeMethod != null)
                        {
                            writeMethod.Invoke(output, new object[] { item });
                        }
                        else 
                        {
                            _writeObjectMethod.MakeGenericMethod(field.FieldType).Invoke(output, new object[] { field.GetValue(value) });
                        }
                    }
                }
            }
            else
            {
                _writeObjectMethod.MakeGenericMethod(field.FieldType).Invoke(output, new object[] { field.GetValue(value) });
            }
        }
    }
}
