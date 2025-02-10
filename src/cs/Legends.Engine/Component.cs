using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

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
    public SceneObject Parent { get; private set; }

    public Component()
    {
        
    }

    public Component(IServiceProvider services, SceneObject parent, string assetName = null) : base(services, assetName)
    {
        Parent = parent;
    }

    
    public virtual void Update(GameTime gameTime) {}
    public virtual void Draw(GameTime gameTime) {} 
}