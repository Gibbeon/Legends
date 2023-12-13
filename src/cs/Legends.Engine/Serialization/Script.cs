using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Legends.Engine.Content;
using Legends.Engine.Runtime;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended.Content;
using Newtonsoft.Json;

namespace Legends.Engine;

public abstract class Script : INamedObject, INotifyReload
{    
    private readonly List<WeakReference> _references = new();
    private readonly object _lock = new();
    private byte[] _data;
    public string Name { get; protected set; }    
    public Type ScriptType { get; }

    public void Purge()
    {
        _references.RemoveAll(n => !n.IsAlive);
    }

    public object Load(object instance)
    {
        return instance;
        //ContentReaderExtensions.ReadObject(new BinaryReader(new MemoryStream(_data))), instance, ScriptType);
    }

    public void OnReload()
    {
        lock(_lock)
        {
            Purge();
            foreach(var reference in _references)
            {
                Load(reference.Target);
            }
        }
    }
}

public class Script<TType> : Script
    where TType : class
{
    public Ref<TType> Create(params object[] args)
    { 
        return Register((TType)Load(Activator.CreateInstance(ScriptType, args)));
    }

    public Ref<TType> Register(TType instance)
    {
        return new Ref<TType>(instance);
    }
}
