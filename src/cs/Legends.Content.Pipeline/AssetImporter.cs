using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Newtonsoft.Json;
using Legends.Engine;
using Legends.Engine.Graphics2D;
using SharpFont;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Legends.Content.Pipline;

public abstract class Asset
{
    internal readonly object _value;
    public abstract Type AssetType { get; }
    public TType Get<TType>() { return (TType)Convert.ChangeType(_value, typeof(TType)); }
    public static Asset Create(object value, Type assetType)
    {
        return (Asset)Activator.CreateInstance(typeof(Asset<>).MakeGenericType(assetType), new[] { value } );
    }

    public Asset(object value)
    {
        _value = value;
    }
}

public class Asset<TType> : Asset
{
    public override Type AssetType      => typeof(TType);
    public TType Get()                  => Get<TType>();

    public static Asset<TType> Create(Asset value)
    {
        return new (value.Get<TType>());
    }

    public Asset(TType value): base(value)
    {

    }

    public static implicit operator TType (Asset<TType> asset)
    {
        return asset.Get();
    }

        public static TType operator~ (Asset<TType> asset)
    {
        return asset.Get();
    }
}

[ContentImporter(".json", DisplayName = "Legends Dynamic Importer", DefaultProcessor = "DynamicProcessor")]
public class DynamicImporter : ContentImporter<dynamic>
{
    public override dynamic Import(string filename, ContentImporterContext context)
    {
        var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
            };
            
        return JsonConvert.DeserializeObject(File.ReadAllText(filename), settings);
    }
}

[ContentProcessor(DisplayName = "Legends Asset Processor")]
public class DynamicProcessor : ContentProcessor<dynamic, Asset>
{
    public override Asset Process(dynamic input, ContentProcessorContext context)
    {
        Console.WriteLine("{0}", ((object)input).GetType().Name);

        return Asset.Create(input, ((object)input).GetType());
    }
}

[ContentTypeWriter]
public class AssetWriter : ContentTypeWriter<Asset>
{
    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        throw new NotImplementedException();
    }

    protected override void Write(ContentWriter output, Asset value)
    {
        throw new NotImplementedException();
    }
}

public class AssetReader : ContentTypeReader<Asset>
{
    protected override Asset Read(ContentReader input, Asset existingInstance)
    {
        throw new NotImplementedException();
    }
}

public class AssetLoader<TType> : IContentLoader<Asset<TType>>
{
    public Asset<TType> Load(ContentManager contentManager, string path)
    {
        return Asset<TType>.Create(contentManager.Load<Asset>(path));
    }
}

public static class ContentManagerExtensions
{
    public static void Load<TType>(this ContentManager contentManager, string path, out Asset<TType> value)
    {
        value = contentManager.Load(path, new AssetLoader<TType>());
    } 
}

public class SceneLike
{
    public string Name { get; protected set; }
    public Asset<Texture2D> _textureAsset { get; protected set; }
    private Texture2D SourceData => ~_textureAsset;

    public void Update()
    {
        
    }
}

public static class ExtensionTest
{
    public static void Update<TType>(this Asset<TType> value)
    {
        
    }
}

public class TesterClass
{
    private Asset<SceneLike> _scene;
    public ContentManager Content;
    public TesterClass(ContentManager content)
    {
        Content = content;
        content.Load("Scenes/test", out _scene);
    }

    public void Update()
    {
        (~_scene).Update();
    }
}