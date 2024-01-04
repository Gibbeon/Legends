using System;
using Microsoft.Xna.Framework;

namespace Legends.Engine;

public interface IBehavior : IComponent
{   
    
}

public abstract class Behavior : Component<IBehavior>, IBehavior
{
    protected Behavior(IServiceProvider services, SceneObject parent) : base(services, parent)
    {
    }
}
