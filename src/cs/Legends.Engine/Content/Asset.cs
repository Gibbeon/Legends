using System;
using System.Reflection;
using Newtonsoft.Json;

namespace Legends.Engine;


public interface IAsset : IInitalizable
{    
    [JsonIgnore] IServiceProvider   Services    { get; }
    string                          Name        { get; }    
}

public abstract class Asset : IAsset
{   
    [JsonIgnore] public IServiceProvider Services { get; protected set; }
    
    public string Name { get; protected set; }

    public Asset(IServiceProvider services)
    {
        Services        = services;
    }

    public abstract void Initialize();
    public abstract void Reset();
    public abstract void Dispose();
}

public class AssetWrapper<TType> : Asset
    where TType : class
{
    protected TType  Instance { get; set; }
    public AssetWrapper(IServiceProvider services): base(services) {}
    
    public override void Initialize()
    {
        Reset();
    }

    public override void Reset()
    {
        if(Instance != null && Instance is IDisposable disposable)
        {
            disposable.Dispose();
        }

        Instance = Services.GetContentManager().Load<TType>(Name);
    }
    public override void Dispose()
    {
        if(Instance != null && Instance is IDisposable disposable)
        {
            disposable.Dispose();
            Instance = default;
        }
    }

    public static implicit operator TType(AssetWrapper<TType> resource) => resource.Instance;
}
