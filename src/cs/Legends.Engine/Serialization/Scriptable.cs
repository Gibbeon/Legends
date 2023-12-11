using System;
using System.IO;
using System.Reflection;
using System.Text.Json.Nodes;
using Legends.Engine.Runtime;
using Legends.Engine.Serialization;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Newtonsoft.Json;

namespace Legends.Engine.Content;

public interface IScriptable
{
    string Source { get; }
    Type AssetType { get; }
    string TypeName { get; }
    dynamic Properties { get; }

    void Set(object value);
}

public static class Scriptable
{
    public static Scriptable<TType> Wrap<TType>(TType value) { return new Scriptable<TType>("", value); }
}

public class Scriptable<TType> : Asset<TType>, IScriptable
{
    public string TypeName { get; set; }
    public dynamic Properties { get; set; }
    public Scriptable() : base() {}
    public Scriptable(string name) : base(name) {}
    internal Scriptable(string name, TType value) : base(name) { _value = value; }

    public override string ToString()
    {
        return string.Format("TypeName: {0} Source: {1}", TypeName, Source);
    }

    void IScriptable.Set(object value) { _value = value; }

    public override void Write(ContentWriter writer)
    {
        base.Write(writer);
        writer.Write(TypeName ?? "");
        writer.WriteObject(Value, AssetType);
    }

    public override void Read(ContentReader reader)
    {        
        base.Read(reader);
        TypeName = reader.ReadString();

        if(!string.IsNullOrEmpty(Source) && !string.IsNullOrEmpty(TypeName))
        {
            var assembly = reader.ContentManager.Load<Assembly>(Path.ChangeExtension(Source, null) + "_0");
        }
        _value = reader.ReadObject(_value, AssetType);
    }
}