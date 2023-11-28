using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Legends.Engine.Graphics2D;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace Legends.Engine;

public abstract class BaseBehavior : IBehavior
{
    [JsonIgnore]
    public SystemServices? Services { get; private set; }
    
    [JsonIgnore]
    public SceneObject? Parent { get; private set; }

    public BaseBehavior(SystemServices? services, SceneObject? parent)
    {
        Services = services;
        Parent = parent;
    }

    public abstract void Update(GameTime gameTime);
    
    public virtual void Draw(GameTime gameTime) {}
    
    public abstract void Dispose();
}
