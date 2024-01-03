using System;
using Assimp;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Legends.Engine;

public abstract class Component<TType> : IComponent
    where TType : IComponent
{
    //public Type ComponentType => typeof(TType);
    
    [JsonIgnore]
    public IServiceProvider Services { get; private set; }
    
    [JsonIgnore]
    public SceneObject Parent { get; private set; }

    public Component(IServiceProvider services, SceneObject parent)
    {
        Services = services;
        Parent = parent;
    }
    
    public abstract void Update(GameTime gameTime);

    public virtual void Draw(GameTime gameTime) {}
    
    public virtual void Dispose()
    {
        GC.SuppressFinalize(true);
    }

    public virtual void AttachTo(SceneObject parent)
    {
        if(parent != null && Services == null)
        {
            Services = parent.Services;
        }

        if(Parent != parent)
        {
            Detach();
            Parent = parent;
            parent?.AttachComponent(this);
        }
    }

    public virtual void Detach(bool detachChildren = true)
    {
        if(detachChildren)
        {
            (Parent).DetachComponent<TType>();
        }
        Parent = null;
    }
}
