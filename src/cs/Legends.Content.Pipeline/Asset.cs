using System;
using Legends.Engine.Runtime;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Legends.Engine.Content;

public interface IContentReadWrite
{
    public void Read(ContentReader reader);
    public void Write(ContentWriter writer);
}

public abstract class Asset : IContentReadWrite
{
    internal readonly object    _value;
    public object Value      => _value;
    public string Source        { get; set; }
    public abstract Type        AssetType { get; }
    public TType Get<TType>()   {  return (TType)Convert.ChangeType(_value, typeof(TType)); }
    protected abstract object Load(ContentManager manager);
    public Asset(string source) {  Source = source; }
    public Asset(string source, object value) : this(source) { _value = value; }

    public virtual Type MakeGenericType()
    {
        return typeof(Asset<>).MakeGenericType(AssetType);
    }

    public virtual object AsExternalReference()
    {
        return Activator.CreateInstance(typeof(ExternalReference<>).MakeGenericType(MakeGenericType()), Source);
    }

    public static Asset Create(string source, object data, Type type )
    {
        return (Asset)typeof(Asset<>).MakeGenericType(type).Create(source, data);
    }

    public void Read(ContentReader reader)
    {

    }
    
    public void Write(ContentWriter writer)
    {
        writer.Write(Source);
    }
}

public class Asset<TType> : Asset
{
    public override Type AssetType      => typeof(TType);
    public TType Get()                  => Get<TType>();
    protected override object Load(ContentManager manager) { return manager.Load<Asset<TType>>(Source); }
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