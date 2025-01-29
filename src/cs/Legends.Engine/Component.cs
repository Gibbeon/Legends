using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using MonoGame.Extended;

namespace Legends.Engine;

public interface IComponent: IAsset, IUpdate, IInitalizable
{
    [JsonIgnore]
    SceneObject Parent { get; }
    void Draw(GameTime gameTime);
}

public abstract class Component : Asset, IComponent
{    
    [JsonIgnore]
    public IServiceProvider Services { get; private set; }
    
    [JsonIgnore]
    public SceneObject Parent { get; private set; }

    public Component(IServiceProvider services, SceneObject parent) : this(AssetType.Dynamic, "")
    {
        Services = services;
        Parent = parent;
    }

    protected Component(AssetType assetType, string assetName)
    {
        AssetName = assetName;
        AssetType = assetType;
    }
    
    public virtual void Update(GameTime gameTime) {}
    public virtual void Draw(GameTime gameTime) {}    
    public abstract void Dispose();
    public abstract void Initialize();
    public abstract void Reset();
}