using System;
using System.Linq;
using Legends.Engine.Content;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Legends.Engine;

public class Scene : SceneObject
{
    private Ref<Camera> _camera;

    [JsonIgnore]
    public Camera Camera 
    {
        get => CameraReference.Get();
        set => SetCamera(value);
    } 

    [JsonProperty("camera")]
    protected Ref<Camera> CameraReference
    { 
        get => _camera; 
        set { _camera = value; SetCamera(value); }
    }

    protected Scene() : this(null, null)
    {

    }

    public Scene(IServiceProvider services, SceneObject parent = default) : base(services, parent)
    {

    }

    public override void Initialize()
    {
        base.Initialize();
        _camera ??= new Camera(Services, this);
        Camera.Initialize();
    }

    public virtual void SetCamera(Ref<Camera> camera)
    {
        if(_camera != null && _camera != camera) 
        {
            (~_camera).Dispose(); 
        }
                          
        _camera = camera;
    }

    public override void Draw(GameTime gameTime)
    {        
        base.Draw(gameTime);
        Camera.Draw(gameTime);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        Camera.Update(gameTime);
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
        Camera?.Dispose();    
        base.Dispose();
    }
}
