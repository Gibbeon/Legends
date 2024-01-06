using System;
using Microsoft.Xna.Framework;
using Legends.Engine;
using Legends.Engine.Input;
using Legends.Engine.Graphics2D;
using Microsoft.Xna.Framework.Input;

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

    bool _init = false;
    public override void Update(GameTime gameTime)
    {
        if(!_init)
        {
            (~Parent.GetComponent<Sprite>().TextureRegion).Frame = Frame;
            _init = true;
        }

    }
}