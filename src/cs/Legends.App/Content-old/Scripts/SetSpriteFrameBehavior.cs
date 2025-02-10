using System;
using Legends.Engine;
using Legends.Engine.Graphics2D.Components;
using Legends.Engine.Graphics2D.Primitives;

namespace Legends.Scripts;

public class SetSpriteFrameBehavior : Behavior
{
    public int Frame { get; set; }

    public SetSpriteFrameBehavior(): this(null, null)
    {

    }
    
    public SetSpriteFrameBehavior(IServiceProvider services, SceneObject parent) : base(services, parent)
    {
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public override void Initialize()
    {
        Parent.GetComponent<Sprite>().FrameIndex = Frame;
    }

    public override void Reset()
    {
        
    }
}