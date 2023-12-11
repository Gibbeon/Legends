using System;
using System.Reflection;

using Microsoft.CodeAnalysis;
using System.IO;
using System.Runtime.Loader;
using Legends.Engine.Content;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Legends.Engine.Serialization;

public class DynamicAssembly : IContentReadWrite
{
    private byte[] _rawData;
    public MetadataReference MetadataReference { get; protected set; }
    public string Identifier { get; protected set; }
    public Assembly Assembly { get; protected set; }
    public byte[] RawData => _rawData;

    public DynamicAssembly(){}
    public DynamicAssembly(string identifier, byte[] rawData){ Identifier = identifier; SetRawData(rawData); }

    protected void SetRawData(byte[] rawData)
    {
        _rawData = rawData;
        Assembly = AssemblyLoadContext.Default.LoadFromStream(new MemoryStream(rawData));
        MetadataReference = MetadataReference.CreateFromImage(rawData);
    }

    public Type GetType(string typeName)
    {
        return Assembly.GetType(typeName);
    }

    public void Read(ContentReader reader)
    {
        Identifier = reader.ReadString();
        SetRawData(reader.ReadBytes(reader.Read7BitEncodedInt()));
        DynamicClassLoader.Register(this);
    }

    public void Write(ContentWriter writer)
    {
        writer.Write(Identifier);
        writer.Write7BitEncodedInt(RawData.Length);
        writer.Write(RawData);
    }
}
