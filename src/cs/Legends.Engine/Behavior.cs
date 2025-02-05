using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Legends.Engine;

public interface IBehavior : IComponent
{   
    
}

public abstract class Behavior : Component, IBehavior
{
    protected Behavior(IServiceProvider services, SceneObject parent, string assetName = null) : base(services, parent, assetName)
    {
    }
}
