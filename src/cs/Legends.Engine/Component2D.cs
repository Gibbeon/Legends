using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using MonoGame.Extended;

namespace Legends.Engine;


public abstract class Component2D : Box2D, IComponent
{    
    [JsonIgnore]
    public IServiceProvider Services { get; private set; }
    
    [JsonIgnore]
    public SceneObject Parent { get; private set; }

    public Component2D(IServiceProvider services, SceneObject parent)
    {
        Services = services;
        Parent = parent;
    }
    
    public virtual void Update(GameTime gameTime) {}
    public virtual void Draw(GameTime gameTime) {}    
    public abstract void Dispose();
    public abstract void Initialize();
    public abstract void Reset();
}