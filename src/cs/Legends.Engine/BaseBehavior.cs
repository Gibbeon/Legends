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
    public GameObject Parent { get; private set; }

    public BaseBehavior(GameObject parent)
    {
        Parent = parent;
    }
    public abstract void Update(GameTime gameTime);
}
