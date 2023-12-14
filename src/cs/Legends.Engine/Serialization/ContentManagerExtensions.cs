using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Legends.Engine.Content;
using Microsoft.Xna.Framework.Content;
namespace Legends.Engine;

public static class ContentManagerExtensions
{
    public static Dictionary<string, object> GetLoadedAssets(this ContentManager contentManager) => (Dictionary<string, object>)typeof(ContentManager)
                                                                    .GetProperty("LoadedAssets", BindingFlags.NonPublic | BindingFlags.Instance)
                                                                    .GetValue(contentManager);

    private static FileSystemWatcher _watcher;

    public static Ref<TType> GetRef<TType>(this ContentManager contentManager, string name) 
        where TType : class
    {
        var instance = contentManager.Load<object>(name);
        if(instance is ContentObject co) return new Ref<TType>(name, (TType)co.Instance);
        return new Ref<TType>(name, (TType)instance);
    }

    public static void ReloadAsset(this ContentManager contentManager, string name)
    {
        if(contentManager.GetLoadedAssets().TryGetValue(name.Replace("\\", "/"), out object result))
        {
            if(result is INotifyReload reloaded) 
                reloaded.OnReload();

            var type = result.GetType();
            typeof(ContentManager)
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Single(n => n.IsGenericMethod && n.Name == "ReloadAsset")
                .MakeGenericMethod(type)
                .Invoke(contentManager, new [] { name, result });
        }
    }

    public static string GetExecutingDirectoryName()
    {
        var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
        return new FileInfo(location.AbsolutePath).Directory.FullName;
    }

    public static void EnableAssetWatching(this ContentManager contentManager)
    {
        _watcher = new FileSystemWatcher(Path.Combine(GetExecutingDirectoryName(), contentManager.RootDirectory))
        {
            //_watcher.NotifyFilter =     NotifyFilters.DirectoryName
            //                        |   NotifyFilters.LastWrite
            //                        |   NotifyFilters.Size;

            IncludeSubdirectories = true,
            EnableRaisingEvents = true
        };
        _watcher.Changed += (sender, args) => { if(args.ChangeType == WatcherChangeTypes.Changed) ReloadAsset(contentManager, Path.ChangeExtension(Path.GetRelativePath(_watcher.Path, args.FullPath), null) ); };

        //_watcher.Changed += OnChanged;
        _watcher.Created += OnCreated;
        _watcher.Deleted += OnDeleted;
        _watcher.Renamed += OnRenamed;
        _watcher.Error += OnError;
    }

    private static void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (e.ChangeType != WatcherChangeTypes.Changed)
        {
            return;
        }
        Console.WriteLine($"Changed: {e.FullPath}");
    }

    private static void OnCreated(object sender, FileSystemEventArgs e)
    {
        string value = $"Created: {e.FullPath}";
        Console.WriteLine(value);
    }

    private static void OnDeleted(object sender, FileSystemEventArgs e) =>
        Console.WriteLine($"Deleted: {e.FullPath}");

    private static void OnRenamed(object sender, RenamedEventArgs e)
    {
        Console.WriteLine($"Renamed:");
        Console.WriteLine($"    Old: {e.OldFullPath}");
        Console.WriteLine($"    New: {e.FullPath}");
    }

    private static void OnError(object sender, ErrorEventArgs e) =>
        PrintException(e.GetException());

    private static void PrintException(Exception ex)
    {
        if (ex != null)
        {
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine("Stacktrace:");
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine();
            PrintException(ex.InnerException);
        }
    }
}
