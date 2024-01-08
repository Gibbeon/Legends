
using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;

namespace Legends.Engine.Collision;

public class RigidBody : Component
{
    public bool Static { get; set; }

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
        if(!this.Static)
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
        _bounds ??= new RectangleBounds(Vector2.Zero, Parent.Size);
        Services.Get<ICollisionService>().Add(this);
    }

    public override void Reset()
    {
    }
}