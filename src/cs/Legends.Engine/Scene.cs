using System;
using System.Linq;
using Legends.Engine.Content;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Legends.Engine;

public class Scene : SceneObject
{
    private Ref<Camera> _camera;
    public Ref<Camera> Camera 
    {
        get => _camera;
        set => _camera = value;
    } 

    protected Scene() : this(null)
    {

    }

    public Scene(IServiceProvider services) : this (services, null)
    {

    }

    public Scene(IServiceProvider services, SceneObject parent) : base(services, parent)
    {

    }


    public virtual void SetCamera(Camera camera)
    {
        if(~Camera != camera)
        {
            if(Camera != null)
            {
                (~Camera).Dispose();                    
            }
            _camera = camera;
        }
    }

    public override void Draw(GameTime gameTime)
    {        
        base.Draw(gameTime);
        (~Camera).Draw(gameTime);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        (~Camera).Update(gameTime);
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}
