using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Legends.Engine;

public abstract class BaseBehavior : IBehavior
{
    [JsonIgnore]
    public IServiceProvider Services { get; private set; }
    
    [JsonIgnore]
    public Ref<SceneObject> Parent { get; private set; }

    public BaseBehavior(IServiceProvider services, SceneObject parent)
    {
        Services = services;
        Parent = parent;
    }
    
    public abstract void Update(GameTime gameTime);
    
    public virtual void Draw(GameTime gameTime) {}
    
    public abstract void Dispose();

    public void AttachTo(SceneObject parent)
    {
        if(parent != null && Services == null)
        {
            Services = parent.Services;
        }

        if(~Parent != parent)
        {
            Detach();
            Parent = parent;
            parent?.AttachBehavior(this);
        }
    }

    public void Detach(bool detachChildren = true)
    {
        if(detachChildren)
        {
            (~Parent).DetachBehavior(this);
        }
        Parent = null;
    }
}
