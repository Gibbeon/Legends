using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Legends.Engine;

public class Scene : SceneObject
{
    private Camera _camera;

    [JsonIgnore]
    public Camera Camera 
    {
        get => _camera;
        set => SetCamera(value);
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

    public virtual void SetCamera(Camera camera)
    {
        if(_camera != null && _camera != camera) 
        {
            _camera.Dispose(); 
        }
                          
        _camera = camera;
    }
    
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        Camera.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {        
        base.Draw(gameTime);
        Camera.Draw(gameTime);
    }


    //public override IEnumerable<SceneObject> GetObjectsAt(Vector2 position)
    //{
     //   foreach(var child in Children.SelectMany(n => n.GetObjectsAt(position)))
      //  {
       //     yield return child;
        //}
   // }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
        Camera?.Dispose();    
        base.Dispose();
    }
}
