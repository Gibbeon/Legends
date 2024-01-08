using Legends.Engine;
using Legends.Engine.Graphics2D;
using Microsoft.Xna.Framework;
using System;

namespace Legends.Scripts;

public class GenerateWorldBehavior : Behavior
{
    bool _init;
    public GenerateWorldBehavior(): this(null, null)
    {

    }
    public GenerateWorldBehavior(IServiceProvider services, SceneObject parent) : base(services, parent)
    {
        
    }

    public override void Update(GameTime gameTime)
    {
        if(!_init) {
            Parent?.GetComponent<Map>().CreateMapFromTexture();
            _init = true;
        }
    }
}