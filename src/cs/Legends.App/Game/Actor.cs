﻿using System;
using Microsoft.Xna.Framework;
using Legends.Engine;
using Legends.Engine.Animation;
using Legends.Engine.Graphics2D;
using Legends.Engine.Resolvers;

namespace Legends.App;

public class Actor : SceneObject
{
    public Vector2 Facing;
    public float Speed;
    ValueResolver<string, Actor> _resolver;

    public Actor(IServiceProvider services, Scene parent) : base(services, parent)
    {
        Speed = 1.0f;
        Facing = DirectionConstants.Down;

        Size = new MonoGame.Extended.Size2(24, 36);
        _resolver = new ValueResolver<string, Actor>();
        
        _resolver.Add("walk", (actor) => { return actor.Facing == DirectionConstants.Left; }, "walk_left");
        _resolver.Add("walk", (actor) => { return actor.Facing == DirectionConstants.Right; }, "walk_right");
        _resolver.Add("walk", (actor) => { return actor.Facing == DirectionConstants.Up; }, "walk_up");
        _resolver.Add("walk", (actor) => { return actor.Facing == DirectionConstants.Down; }, "walk_down");

        _resolver.Add("idle", (actor) => { return actor.Facing == DirectionConstants.Left; }, "idle_left");
        _resolver.Add("idle", (actor) => { return actor.Facing == DirectionConstants.Right; }, "idle_right");
        _resolver.Add("idle", (actor) => { return actor.Facing == DirectionConstants.Up; }, "idle_up");
        _resolver.Add("idle", (actor) => { return actor.Facing == DirectionConstants.Down; }, "idle_down");

        AttachBehavior(new SpriteRenderBehavior(services, this));
        //AttachBehavior(new AnimationBehavior(services, this));
        AttachBehavior(new RandomMovementBehavior(services, this));

        foreach(var data in Data.AnimationData)
        {
            GetBehavior<AnimationBehavior>().Animations.Add( new SpriteKeyframeAnimation(GetBehavior<SpriteRenderBehavior>(), data));
        }
    }

    bool IsMoving = false;

    public override void Move(Vector2 direction)
    {
        Facing = DirectionConstants.GetNearestFacing(direction);
        GetBehavior<AnimationBehavior>().Play(_resolver.Resolve("walk", this));
        IsMoving = true;
        base.Move(Facing * Speed);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if(!IsMoving)
        {
            GetBehavior<AnimationBehavior>().Play(_resolver.Resolve("idle", this));
        } 
        IsMoving = false;
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}
