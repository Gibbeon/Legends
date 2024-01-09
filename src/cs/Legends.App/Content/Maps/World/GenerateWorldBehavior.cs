using Legends.Engine;
using Legends.Engine.Graphics2D.Components;
using System;

namespace Legends.Scripts;

public class GenerateWorldBehavior : Behavior
{
    public GenerateWorldBehavior(): this(null, null)
    {

    }
    public GenerateWorldBehavior(IServiceProvider services, SceneObject parent) : base(services, parent)
    {
        
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public override void Initialize()
    {
        Parent?.GetComponent<Map>().CreateMapFromTexture();
    }

    public override void Reset()
    {
        
    }
}