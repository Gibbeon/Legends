using System;
using System.IO;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Content;

namespace Legends.Engine.Content;

public static class ContentManagerExtensions
{
    public static FileSystemWatcher _watcher;
    
    public static void EnableAssetWatching(this ContentManager contentManager)
    {
        _watcher = new FileSystemWatcher(contentManager.RootDirectory);
        _watcher.Changed += (sender, args) => { if(args.ChangeType == WatcherChangeTypes.Changed) contentManager.Reload(args.Name); }; 
    }

    public static void Load<T>(this ContentManager contentManager, string assetName, out Asset<T> assetValue)
    {
        assetValue = contentManager.Load(assetName, new AssetLoader<T>());        
    }

    public static void Reload(this ContentManager contentManager, string assetName)
    {
            
    }
}