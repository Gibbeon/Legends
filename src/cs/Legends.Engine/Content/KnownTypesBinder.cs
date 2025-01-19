using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json.Serialization;

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
        Console.WriteLine("BindToType: {0}, {1}", assemblyName, typeName);
        if(AssemblyNames.Contains(assemblyName) || string.IsNullOrEmpty(assemblyName))
        {
            return KnownTypes.SingleOrDefault(t => t.FullName == typeName) ?? _defaultBinder.BindToType(assemblyName, typeName);
        }

        return _defaultBinder.BindToType(assemblyName, typeName);
    }

    public void BindToName(Type serializedType, out string assemblyName, out string typeName)
    {
        _defaultBinder.BindToName(serializedType, out assemblyName, out typeName);        
    }
}