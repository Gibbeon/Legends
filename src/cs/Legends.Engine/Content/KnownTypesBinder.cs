using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json.Serialization;
using SharpFont.MultipleMasters;

namespace Legends.Engine.Content;

public class KnownTypesBinder : ISerializationBinder
{
    public IList<Type> KnownTypes { get; set; }

    public IList<string> AssemblyNames { get; set; }

    private DefaultSerializationBinder _defaultBinder = new();

    public KnownTypesBinder()
    {
        KnownTypes = new List<Type>();
        AssemblyNames = new List<string>();
    }

    public KnownTypesBinder(params string[] assemblyNames) : this(AppDomain.CurrentDomain.GetAssemblies().Where(n => assemblyNames.Contains(n.GetName().Name)).ToArray())
    {

    }
    public KnownTypesBinder(params Assembly[] assemblies) : this()
    {
        foreach(var assembly in assemblies)
        {                        
            Console.WriteLine("KnownTypesBinder: {0}", assembly.GetName().Name);
            AssemblyNames.Add(assembly.GetName().Name);

            foreach(var type in assembly.GetTypes())
            {
                KnownTypes.Add(type);                    
            }
        }
    }

    public Type BindToType(string assemblyName, string typeName)
    {
        if(AssemblyNames.Contains(assemblyName) || string.IsNullOrEmpty(assemblyName))
        {
            var stack = new Stack<string>();

            var processTypeName = typeName;
            //handle generic types
            while(processTypeName.Contains("<"))
            {
                int start = processTypeName.IndexOf('<');
                int end   = processTypeName.LastIndexOf('>');

                var genericTypeName = processTypeName.Substring(0, start);
                processTypeName  = processTypeName.Substring(start + 1, end - start - 1);

                // only handles 1 type param for now
                stack.Push(genericTypeName);
            }

            stack.Push(processTypeName);

            var lastType = default(Type);

            while(stack.Count > 0) {
                var newTypeName = stack.Pop();

                // hack to get generic param
                var type = KnownTypes.SingleOrDefault(t => t.FullName.Split('`')[0] == newTypeName) ?? 
                            _defaultBinder.BindToType(assemblyName, newTypeName);
            
                if(type == null) throw new KeyNotFoundException(string.Format("{0}", newTypeName));

                if(lastType != null && type.IsGenericTypeDefinition)
                {
                    lastType = type.MakeGenericType(lastType);
                }
                else
                {
                    lastType = type;
                }
            }

            return lastType;
        }

        return _defaultBinder.BindToType(assemblyName, typeName);
    }

    public void BindToName(Type serializedType, out string assemblyName, out string typeName)
    {
        _defaultBinder.BindToName(serializedType, out assemblyName, out typeName);        
    }
}