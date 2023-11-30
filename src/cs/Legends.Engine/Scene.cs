using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Legends.Engine;

public class Scene : SceneObject
{
    public Camera? Camera { get; set; }

    protected Scene() : this(null)
    {

    }

    public Scene(IServiceProvider? services) : this (services, null)
    {

    }

    public Scene(IServiceProvider? services, SceneObject? parent) : base(services, parent)
    {
        if(services != null)
        {
            SetCamera(new Camera(services, this));
        }
    }

    public override void Initialize()
    {
        base.Initialize();

        if(Camera == null)
        {
            var camera = (Camera?)Children.SingleOrDefault(n => n.GetType().IsAssignableTo(typeof(Camera)));

            if(camera != null)
            {
                camera?.Initialize();
                camera?.Detach();

                SetCamera(camera);
            }
        }
    }

    public virtual void SetCamera(Camera camera)
    {
        Camera?.Dispose();
        Camera = camera;
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
