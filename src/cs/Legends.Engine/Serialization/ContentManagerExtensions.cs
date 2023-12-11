using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Legends.Engine.Runtime;

namespace Legends.Engine.Content;

public static class ContentManagerExtensions
{
    private static FileSystemWatcher _watcher;
    private static Dictionary<string, IList<Asset>> _assetCache = new();
    
    public static void EnableAssetWatching(this ContentManager contentManager)
    {
        _watcher = new FileSystemWatcher(contentManager.RootDirectory);
        _watcher.Changed += (sender, args) => { if(args.ChangeType == WatcherChangeTypes.Changed) contentManager.Reload(Path.GetRelativePath(contentManager.RootDirectory, args.Name)); }; 
    }

    public static void Reload(this ContentManager contentManager, string assetName)
    {
        if(_assetCache.TryGetValue(assetName, out IList<Asset> list))
        {
            contentManager.UnloadAsset(assetName);
            foreach(var item in list) item.Load(contentManager);
        }
    }

    public static void Track(this ContentManager contentManager, Asset asset)
    {
        typeof(ContentManagerExtensions)
            .GetMethods()
            .Single(n => n.Name == "Track" && n.IsGenericMethod)
            .MakeGenericMethod(asset.MakeGenericType())
            .InvokeAny(contentManager, asset);
        
    }

    public static void Track<TType>(this ContentManager contentManager, Asset<TType> asset)
    {
        if(!_assetCache.TryGetValue(asset.Source, out IList<Asset> list))
        {
            list = new List<Asset>();
            _assetCache.Add(asset.Source, list);
        }
        if(!list.Contains(asset)) list.Add(asset);
    }
}