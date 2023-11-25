using Microsoft.Xna.Framework;
using Legends.Engine;
using Legends.Engine.Animation;
using Legends.Engine.Graphics2D;
using Legends.Engine.Resolvers;
using System;
using MonoGame.Extended;

namespace Legends.App;

public class RandomMovementBehavior : BaseBehavior
{
    private float _waitTime;
    private Vector2 _targetPosition;

    private Random _random;
    public RandomMovementBehavior(GameObject parent) : base(parent)
    {
        _random = new Random();
        _waitTime = 1 + _random.NextSingle(4);
    }

    public override void Update(GameTime gameTime)
    {
        if(_waitTime > 0)
        {
            _waitTime -= gameTime.GetElapsedSeconds();
        }

        if(_waitTime <= 0)
        {
            if(_targetPosition != Parent.Position)
            {
                var move = (_targetPosition - Parent.Position);
                if(MathF.Abs(move.X) + Math.Abs(move.Y) > 1)
                {
                    move.Normalize();                    
                    Parent.Move(move);
                } else {
                    Parent.Position = _targetPosition;
                }                
            }

            if(Parent.Position == _targetPosition)
            {
                _waitTime = 1 + _random.NextSingle(4);
                if(_random.Next() % 2 == 1)
                {
                    _targetPosition = new Vector2(_random.Next(-320, 320), Parent.Position.Y);
                } 
                else
                {
                    _targetPosition = new Vector2(Parent.Position.X, _random.Next(-200, 200));
                }
            }
        }
    }
}
public class Actor : GameObject
{
    public Vector2 Facing;
    public float Speed;
    ValueResolver<string, Actor> _resolver;

    public Actor(SystemServices services) : base(services)
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

        AttachBehavior(new SpriteRenderBehavior(this));
        AttachBehavior(new AnimationBehavior(this));
        AttachBehavior(new RandomMovementBehavior(this));

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
}
