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
using Newtonsoft.Json;

namespace Legends.Engine;

/*
public abstract class Asset : IContentReadWrite
{
    internal object    _value;
    public object Value      => _value;
    public string Source        { get; set; }
    public abstract Type        AssetType { get; }
    public TType Get<TType>()   {  return (TType)Convert.ChangeType(_value, typeof(TType)); }
    public abstract void Load(ContentManager manager);
    public Asset() {}
    public Asset(string source) {  Source = source; }
    public Asset(string source, object value) : this(source) { _value = value; }

    public virtual Type MakeGenericType()
    {
        return typeof(Asset<>).MakeGenericType(AssetType);
    }

    public virtual object AsExternalReference()
    {
        return Activator.CreateInstance(typeof(ExternalReference<>).MakeGenericType(AssetType), Source);
    }

    public static Asset Create(string source, object data, Type type )
    {
        return (Asset)typeof(Asset<>).MakeGenericType(type).Create(source, data);
    }

    public virtual void Read(ContentReader reader)
    {
        Source = reader.ReadString();
        Load(reader.ContentManager);
    }
    
    public virtual void Write(ContentWriter writer)
    {
        writer.Write(Source ?? "");
    }
}

public class Asset<TType> : Asset
{
    public override Type AssetType      => typeof(TType);
    public TType Get()                  => Get<TType>();
    public override void Load(ContentManager manager) { _value = manager.Load<TType>(Source); }
    public Asset() {}
    public Asset(string source) : base(source) {}
    public Asset(string source, TType value) : base(source, value) {}
    public static implicit operator TType (Asset<TType> asset) { return asset.Get(); }
    public static implicit operator Asset<TType> (string source) { return new Asset<TType>(source); }
    public static TType operator~ (Asset<TType> asset) { return asset.Get(); }

    public override Type MakeGenericType()
    {
        return typeof(Asset<TType>);
    }
    public static Asset<TType> Create(string source, TType data)
    {
        return new Asset<TType>(source, data);
    }
}
*/


public interface INamedObject
{
    string Name { get; }
}

public interface IRef : INamedObject
{
    bool IsExternal { get; }
    Type RefType { get; }

    object Get();
}

public interface IRef<TType> : IRef
{
    new TType Get();
}

public class Ref<TType> : IRef, IComparable<Ref<TType>>
    where TType : class
{
    private TType _value;
    private string _name;
    public bool IsExternal => !string.IsNullOrEmpty(_name);
    public Type RefType => typeof(TType);
    public string Name => (_value is INamedObject namedValue) ? namedValue.Name : _name;
    public TType Get() => _value;    
    object IRef.Get() => _value;

    public int CompareTo(Ref<TType> other)
    {
        return Comparer.Default.Compare(this._value, other._value);
    }

    public static implicit operator TType (Ref<TType> reference) => reference?.Get();
    public static implicit operator Ref<TType> (TType value) => new (value);
    public static implicit operator Ref<TType> (string value) => new (value);
    public static TType operator~ (Ref<TType> reference) => reference?.Get();
    public Ref(string name)
    {
        _name = name;
    }
    public Ref(TType reference)
    {
        _value = reference;
    }

    public override string ToString()
    {
        return string.Format("ref {0}", IsExternal ? _name : _value == null ? "(null)" : _value.ToString());
    }
}

public interface INotifyReload
{
    void OnReload();
}

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

public static class ContentManagerExtensions
{
    public static Dictionary<string, object> GetLoadedAssets(this ContentManager contentManager) => (Dictionary<string, object>)typeof(ContentManager)
                                                                    .GetProperty("LoadedAssets", BindingFlags.NonPublic | BindingFlags.Instance)
                                                                    .GetValue(contentManager);

    public static Ref<TType> GetRef<TType>(this ContentManager contentManager, string name) 
        where TType : class
    {
        var instance = contentManager.Load<object>(name);
        if(instance is ContentObject co) return new Ref<TType>((TType)co.Instance);
        return new Ref<TType>((TType)instance);
    }

    public static void ReloadAsset(this ContentManager contentManager, string name)
    {
        if(contentManager.GetLoadedAssets().TryGetValue(name, out object result))
        {
            var type = result.GetType();
            typeof(ContentManager)
                .GetMethods()
                .Single(n => n.IsGenericMethod && n.Name == "ReloadAsset")
                .MakeGenericMethod(type)
                .Invoke(contentManager, new [] { name, result });
            
            if(result is INotifyReload reloaded) reloaded.OnReload();
        }
    }
}
