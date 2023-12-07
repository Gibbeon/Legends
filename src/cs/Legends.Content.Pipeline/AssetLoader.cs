using System;
using System.Runtime.InteropServices;
using Legends.Engine.Content;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended.Content;

namespace Legends.Engine.Content;

[ContentTypeWriter]
public class AssetWriter : ContentTypeWriter<Asset>
{
    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return typeof(AssetReader).AssemblyQualifiedName;
    }

    protected override void Write(ContentWriter output, Asset value)
    {
        output.Write(value.AssetType.AssemblyQualifiedName);
        output.Write(value.Source);
    }
}

public class AssetReader : ContentTypeReader<Asset>
{
    protected override Asset Read(ContentReader input, Asset existingInstance)
    {
        var typeName = input.ReadString();
        var source = input.ReadString();

        return Asset.Create(source, null, Type.GetType(typeName));
    }
}

public class AssetLoader<TType> : IContentLoader<Asset<TType>>
{
    public Asset<TType> Load(ContentManager contentManager, string path)
    {
        return contentManager.Load<Asset<TType>>(path);
    }
}