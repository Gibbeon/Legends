
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Legends.Engine;
using Legends.Engine.Runtime;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Legends.Engine.Collision;

public interface ICollisionService : IUpdate
{
    void Add(RigidBody body);
    void Remove(RigidBody body);
}

public interface IBounds : IEquatable<IBounds>, IEquatableByRef<IBounds>
{

}

public class RectangleBounds : IBounds
{
    private BoundingRectangle _rectangle;

    public RectangleBounds() : this(BoundingRectangle.Empty)
    {

    }

    public RectangleBounds(BoundingRectangle rectangle)
    {
        _rectangle = rectangle;
    }

    public bool Collide(RectangleBounds other)
    {
        return BoundingRectangle.Intersects(_rectangle, other._rectangle);
    }

    public bool Equals(IBounds other)
    {
        return other is RectangleBounds rectangleBounds ? rectangleBounds._rectangle == this._rectangle : false;
    }

    public bool Equals(ref IBounds other)
    {
        return other is RectangleBounds rectangleBounds ? rectangleBounds._rectangle == this._rectangle : false;
    }
}

public class RigidBody : Component<RigidBody>
{
    public bool Static { get; set; }

    [DefaultValue(true)]
    public bool BindToParent { get; set; }
    private IBounds _bounds;
    private Vector2 _previousPosition;
    public IBounds Bounds
    {
        get => BindToParent && Parent != null ? new RectangleBounds(Parent.GetBoundingRectangle()) : _bounds;
        set => _bounds = value;
    }
    public RigidBody(IServiceProvider services, SceneObject parent) : base(services, parent)
    {
        BindToParent = true;
        Bounds = new RectangleBounds();
        if(services != null)
        {
            services.Get<ICollisionService>().Add(this);
        }
    }

    public override void Dispose()
    {
        if(Services != null)
        {
            Services.Get<ICollisionService>().Remove(this);
        }

        base.Dispose();
    }

    public void ResolveCollision(RigidBody other)
    {        
        this.Parent.Position = _previousPosition;        
    }

    public override void Update(GameTime gameTime)
    {

    }

    public override void Draw(GameTime gameTime)
    {
        _previousPosition = Parent.Position;
    }
}

public class CollisionService : ICollisionService
{
    public IServiceProvider Services { get; private set; }
    public CollisionService(IServiceProvider services)
    {
        Services = services;
        Services.Add<ICollisionService>(this);
    }

    public void Update(GameTime gameTime)
    {
        foreach(var lhs in _bodies)
        {
            foreach(var rhs in _bodies)
            {
                if(Collide(lhs.Bounds, rhs.Bounds))
                {
                    lhs.ResolveCollision(rhs);
                }
            }
        }
    }  

    private static Dictionary<Tuple<Type, Type>, MethodInfo> _cache = new();
    
    public static bool Collide(IBounds lhs, IBounds rhs)
    {
        
        if(lhs.Equals(ref rhs)) return false;

        if(_cache.TryGetValue(Tuple.Create(lhs.GetType(), rhs.GetType()), out MethodInfo methodInfo))
        {        
            return (bool)methodInfo.InvokeAny(lhs, rhs);
        }

        if(_cache.TryGetValue(Tuple.Create(rhs.GetType(), lhs.GetType()), out methodInfo))
        {
            return (bool)methodInfo.InvokeAny(rhs, lhs);
        }

        methodInfo = lhs.GetType().GetAnyMethod(typeof(bool), "*", rhs.GetType());
        if(methodInfo != null)
        {
            _cache.Add(Tuple.Create(lhs.GetType(), rhs.GetType()), methodInfo);
            return (bool)methodInfo.InvokeAny(lhs, rhs);
        }
        else
        {
             methodInfo = rhs.GetType().GetAnyMethod(typeof(bool), "*", lhs.GetType());
             if(methodInfo != null)
             {
                _cache.Add(Tuple.Create(rhs.GetType(), lhs.GetType()), methodInfo);
                return (bool)methodInfo.InvokeAny(rhs, lhs);
             }
             else
             {
                throw new NotSupportedException();
             }
        }        
    }

    private List<RigidBody> _bodies = new();

    public void Add(RigidBody body)
    {
        _bodies.Add(body);
    }

    public void Remove(RigidBody body)
    {
        _bodies.Remove(body);
    }
}