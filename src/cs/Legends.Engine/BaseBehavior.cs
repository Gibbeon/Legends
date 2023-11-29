using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Legends.Engine;

public abstract class BaseBehavior : IBehavior
{
    [JsonIgnore]
    public IServiceProvider? Services { get; private set; }
    
    [JsonIgnore]
    public SceneObject? Parent { get; private set; }

    public BaseBehavior(IServiceProvider? services, SceneObject? parent)
    {
        Services = services;
        Parent = parent;
    }

    public abstract void Update(GameTime gameTime);
    
    public virtual void Draw(GameTime gameTime) {}
    
    public abstract void Dispose();
}
