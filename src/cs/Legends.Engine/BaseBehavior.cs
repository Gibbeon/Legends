using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Legends.Engine.Graphics2D;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework.Graphics;

namespace Legends.Engine;

public abstract class BaseBehavior : IBehavior
{
    public SceneObject Parent { get; private set; }

    public BaseBehavior(SceneObject parent)
    {
        Parent = parent;
    }
    public abstract void Update(GameTime gameTime);
    public virtual void Draw(GameTime gameTime)
    {

    }
    
    public virtual void Dispose()
    {

    }
}
