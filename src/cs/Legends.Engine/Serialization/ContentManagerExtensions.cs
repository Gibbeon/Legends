using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Legends.Engine.Content;
using Microsoft.Xna.Framework.Content;
namespace Legends.Engine;

public static class ContentManagerExtensions
{
    public static Dictionary<string, object> GetLoadedAssets(this ContentManager contentManager) => (Dictionary<string, object>)typeof(ContentManager)
                                                                    .GetProperty("LoadedAssets", BindingFlags.NonPublic | BindingFlags.Instance)
                                                                    .GetValue(contentManager);

    private static FileSystemWatcher _watcher;
    private static readonly HashSet<string> _events = new();
    private static readonly object _lock = new();

    public static Ref<TType> GetRef<TType>(this ContentManager contentManager, string name) 
        where TType : class
    {
        var instance = contentManager.Load<object>(name);
        if(instance is ContentObject co) return new Ref<TType>(name, (TType)co.Instance);
        return new Ref<TType>(name, (TType)instance);
    }

    public static void DoReloads(this ContentManager contentManager)
    {
        lock(_lock)
        {
            foreach(var item in _events)
            {
                contentManager.ReloadAsset(item);
            }
            _events.Clear();
        }
    }

    public static void QueueReload(this ContentManager contentManager, string name)
    {
        lock(_lock)
        {
            _events.Add(name);
        }
    }

    public static void ReloadAsset(this ContentManager contentManager, string name)
    {
        if(contentManager.GetLoadedAssets().TryGetValue(name.Replace("\\", "/"), out object result))
        {
            if(result is INotifyReload reloaded) 
                reloaded.OnReload();

            while(true)
            {
                try
                {
                    var type = result.GetType();
                    typeof(ContentManager)
                        .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                        .Single(n => n.IsGenericMethod && n.Name == "ReloadAsset")
                        .MakeGenericMethod(type)
                        .Invoke(contentManager, new [] { name, result });

                        return;
                }
                catch(TargetInvocationException err)
                {
                    if(err.InnerException.GetType() == typeof(ContentLoadException))
                    {
                        Thread.Sleep(10);
                    }
                    else
                    {
                        throw err.InnerException;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            } 
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
            IncludeSubdirectories = true,
            EnableRaisingEvents = true
        };

        _watcher.NotifyFilter =     NotifyFilters.LastWrite
                                |   NotifyFilters.Size;

        _watcher.Changed += (sender, args) => { 
            if(args.ChangeType == WatcherChangeTypes.Changed)
                 QueueReload(contentManager, Path.ChangeExtension(Path.GetRelativePath(_watcher.Path, args.FullPath), null) ); 
        };

    }
}
