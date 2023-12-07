using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Newtonsoft.Json;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Legends.Engine.Runtime;
using System.Collections.Generic;
using Legends.Content.Pipline.JsonConverters;
using System.Dynamic;

namespace Legends.Content.Pipline;

public static class ContentManagerExtensions
{
    public static FileSystemWatcher _watcher;
    
    public static void EnableAssetWatching(this ContentManager contentManager)
    {
        _watcher = new FileSystemWatcher(contentManager.RootDirectory);
        _watcher.Changed += (sender, args) => { if(args.ChangeType == WatcherChangeTypes.Changed) contentManager.ReloadAsset(args.Name); }; 
    }

    public static void ReloadAsset(this ContentManager contentManager, string assetName)
    {
        var asset = contentManager.Load<Asset>(assetName);

        typeof(ContentManager).GetAnyMethod("ReloadAsset", asset.AssetType, typeof(string), typeof(Asset)).Invoke(contentManager, new []
        {
            assetName,
            Convert.ChangeType(asset, asset.GetType())
        });
    }

    public static void ReloadAsset<TType>(this ContentManager contentManager, string assetName, Asset<TType> asset)
    {
        //contentManager.ReloadAsset<TType>(assetName, asset);
    }

    public static void LoadAsset<T>(this ContentManager contentManager, string assetName, out Asset<T> assetValue)
    {
        assetValue = contentManager.Load(assetName, new AssetLoader<T>());        
    }
}

public abstract class Asset
{
    internal readonly object    _value;
    public string Source  { get; set; }
    public abstract Type        AssetType { get; }
    public TType Get<TType>()   {  return (TType)Convert.ChangeType(_value ?? Load(), typeof(TType)); }
    protected abstract object Load();
    public Asset(string name) {  Source = name; }
}

public class Asset<TType> : Asset
{
    public override Type AssetType      => typeof(TType);
    public TType Get()                  => Get<TType>();

    protected override object Load() { return _value; }
    public Asset(string name) : base(name) {}
    public static implicit operator TType (Asset<TType> asset) { return asset.Get(); }
    public static TType operator~ (Asset<TType> asset) { return asset.Get(); }
}

public class Scriptable : Asset<IBehaviorLike>
{
    public string TypeName { get; set; }
    public Scriptable(string name) : base(name) {}

    public override string ToString()
    {
        return string.Format("TypeName: {0} Source: {1}", TypeName, Source);
    }
}


[ContentImporter(".json", DisplayName = "Legends Dynamic Importer", DefaultProcessor = "DynamicProcessor")]
public class DynamicImporter : ContentImporter<SceneLike>
{
    public override SceneLike Import(string filename, ContentImporterContext context)
    {
        var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
            };
        
        settings.Converters.Add(new AssetJsonConverter());
            
        return JsonConvert.DeserializeObject<SceneLike>(File.ReadAllText(filename), settings);
    }
}

[ContentProcessor(DisplayName = "Legends Asset Processor")]
public class DynamicProcessor : ContentProcessor<SceneLike, Asset>
{
    public override Asset Process(SceneLike input, ContentProcessorContext context)
    {
        var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

        settings.Converters.Add(new AssetJsonConverter());

        Console.WriteLine(JsonConvert.ToString(JsonConvert.SerializeObject(input, settings)));

        return null;//Asset.Create(input, ((object)input).GetType());
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
        return null;
    }
}

public interface IBehaviorLike
{
    public string Duval { get; }
}
public class BehaviorLike : IBehaviorLike
{
    public string Duval { get; set; }
    public Asset<Texture2D> Texture { get; set; }  
}

public class SceneLike
{
    public string Name { get; set; }
    public Asset<Texture2D> Texture { get; set; }  
    public List<Asset<IBehaviorLike>> Behaviors { get; set; }
    private Texture2D SourceData => ~Texture;

    public void Update()
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
        content.LoadAsset("Scenes/test", out _scene);
    }

    public void Update()
    {
        (~_scene).Update();
    }
}