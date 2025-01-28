using Legends.Engine;
using Legends.Engine.Graphics2D.Components;
using Microsoft.Xna.Framework;
using System;

namespace Legends.Scripts;

public class AutoGenerateMapBehavior : Behavior
{
    public AutoGenerateMapBehavior(): this(null, null)
    {

    }
    public AutoGenerateMapBehavior(IServiceProvider services, SceneObject parent) : base(services, parent)
    {
        
    }

    public override void Dispose()
    {
        
    }

    public override void Initialize()
    {
        Parent?.GetComponent<Map>().CreateMapFromTexture();
    }

    public override void Reset()
    {
        
    }

    public override void Update(GameTime gameTime)
    {

    }
}