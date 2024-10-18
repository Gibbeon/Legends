
using System;
using System.Collections.Generic;
using System.Reflection;
using Legends.Engine.Runtime;
using Microsoft.Xna.Framework;

namespace Legends.Engine.Collision;

public interface ICollisionService : IUpdate
{
    void Add(RigidBody body);
    void Remove(RigidBody body);
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