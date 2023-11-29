using System;
using Microsoft.Xna.Framework;

namespace Legends.Engine;

public class Scene : SceneObject
{
    public Camera Camera { get; set; }

    protected Scene() : this(null)
    {

    }

    public Scene(IServiceProvider? services) : this (services, null)
    {

    }

    public Scene(IServiceProvider? services, SceneObject? parent) : base(services, parent)
    {
        SetCamera(new Camera(services, this));
    }

    public virtual void SetCamera(Camera camera)
    {
        Camera?.Dispose();
        Camera = camera;
        AttachChild(camera);
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

    public override void Dispose()
    {
        Camera?.Dispose();
        base.Dispose();
    }
}
