using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Legends.Engine;

public class Scene : SceneObject
{
    public class SceneDesc : SceneObjectDesc
    {
        public Camera.CameraDesc Camera;

        public SceneDesc()
        {
            Camera = new Camera.CameraDesc();
        }
    }

    public Camera? Camera { get; set; }

    public Scene(SystemServices services) : this (services, null)
    {
        //Camera = new Camera(services, this);
    }

    public Scene(SystemServices services, SceneObject? parent) : base(services, parent)
    {

    }

    public override void Draw(GameTime gameTime)
    {        
        base.Draw(gameTime);
        Camera?.Draw(gameTime);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        Camera?.Update(gameTime);
    }
}
