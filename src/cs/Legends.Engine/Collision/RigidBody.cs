
using System;
using System.ComponentModel;
using System.Drawing;
using Microsoft.Xna.Framework;

namespace Legends.Engine.Collision;

public class RigidBody : Component
{
    public bool Dynamic { get; set; }
    private IBounds _bounds;
    private Vector2 _previousPosition;
    public IBounds Bounds
    {
        get => _bounds;
        set => _bounds = value;
    }

    public RigidBody(IServiceProvider services, SceneObject parent) : base(services, parent)
    {

    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
        Services?.Get<ICollisionService>().Remove(this);
    }

    public void ResolveCollision(RigidBody other)
    {        
        if(this.Dynamic)
        {
            this.Parent.Position = _previousPosition;    
        }    
    }

    public override void Draw(GameTime gameTime)
    {
        _previousPosition = Parent.Position;
    }

    public override void Initialize()
    {
        _previousPosition = this.Parent.Position;
        _bounds ??= new RectangleBounds();
        Services.Get<ICollisionService>().Add(this);
    }

    public override void Reset()
    {
    }
}