using System;

namespace Legends.Engine;

public interface IBehavior : IComponent
{   
    
}

public abstract class Behavior : Component, IBehavior
{
    protected Behavior(IServiceProvider services, SceneObject parent) : base(services, parent)
    {
        
    }
}
