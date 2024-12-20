using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json.Serialization;

namespace Legends.Engine.Content;

public class KnownTypesBinder : ISerializationBinder
{
    public IList<Type> KnownTypes { get; set; }

    public string NamespacePrefix { get; set; }

    private DefaultSerializationBinder _defaultBinder = new();

    public KnownTypesBinder()
    {
        KnownTypes = new List<Type>();
        NamespacePrefix = "";
    }

    public KnownTypesBinder(string nsPrefix) : this()
    {
        NamespacePrefix = nsPrefix;

        foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach(var type in assembly.GetTypes())
            {
                if(!string.IsNullOrEmpty(type.Namespace) && type.Namespace.StartsWith(NamespacePrefix))
                {
                    KnownTypes.Add(type);                    
                }
            }
        }
    }

    public Type BindToType(string assemblyName, string typeName)
    {
        return KnownTypes.SingleOrDefault(t => t.Name == typeName || t.Name == NamespacePrefix + typeName) ?? _defaultBinder.BindToType(assemblyName, typeName);
    }

    public void BindToName(Type serializedType, out string assemblyName, out string typeName)
    {
        _defaultBinder.BindToName(serializedType, out assemblyName, out typeName);        
    }
}