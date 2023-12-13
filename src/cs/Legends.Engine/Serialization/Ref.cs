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

public interface INamedObject
{
    string Name { get; }
}

public interface IRef : INamedObject
{
    bool IsExternal { get; }
    bool IsExtended { get; }
    Type RefType { get; }

    void Load(ContentManager manager);

    object Get();
    void Set(object value);
}

public interface IRef<TType> : IRef
{
    new TType Get();
}

public class Ref<TType> : IRef, IComparable<Ref<TType>>, IEquatable<Ref<TType>>
    where TType : class
{
    private TType _value;
    private string _name;
    private bool _external;
    private bool _extended;
    public bool IsExternal => _external;
    public bool IsExtended { get => _extended; set => _extended = value; }
    public Type RefType => typeof(TType);
    public string Name => (_value is INamedObject namedValue) ? namedValue.Name : _name;
    public TType Get() => _value;    
    object IRef.Get() => _value;

    public TType Set(TType value) => _value = value;
    void IRef.Set(object value) => _value = (TType)value;

    public void Load(ContentManager manager)
    {
        _value = manager.Load<TType>(_name);
    }

    public int CompareTo(Ref<TType> other)
    {
        return Comparer<Ref<TType>>.Default.Compare(this._value, other._value);
    }

    public static implicit operator TType (Ref<TType> reference) => reference?.Get();
    public static implicit operator Ref<TType> (TType value) => new (value);
    public static implicit operator Ref<TType> (string value) => new (value);
    public static TType operator~ (Ref<TType> reference) => reference?.Get();
    public Ref(string name): this(name, null)
    {
  
    }
    public Ref(TType reference): this(null, reference)
    {

    }
    public Ref(string name, TType reference): this(name, reference, !string.IsNullOrEmpty(name))
    {

    }  
    public Ref(string name, TType reference, bool external): this(name, reference, external, false)
    {
    }

    public Ref(string name, TType reference, bool external, bool extended)
    {
        _name = name;
        _value = reference;
        _external = external;
        _extended = extended;
    }
  
  
    public override string ToString()
    {
        return string.Format("ref {0}", IsExternal ? _name : _value == null ? "(null)" : _value.ToString());
    }

    public bool Equals(Ref<TType> other)
    {
        return this._value == other._value;
    }
}
