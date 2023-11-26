using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Legends.Engine.Graphics2D;

public class Scene : SceneObject
{
    public Legends.Engine.Graphics2D.Camera? Camera { get; set; }

    public Scene(SystemServices services) : this (services, null)
    {
        Camera = new Camera(services);
    }

    public Scene(SystemServices services, SceneObject? parent) : base(services, parent)
    {

    }

    public override void Draw(GameTime gameTime)
    {        
        base.Draw(gameTime);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }
}
