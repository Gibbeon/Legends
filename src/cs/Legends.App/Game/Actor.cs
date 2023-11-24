using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Legends.Engine;
using Legends.Engine.Animation;
using Legends.Engine.Graphics2D;
using Legends.Engine.Resolvers;
using System;

namespace Legends.App;

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

        Keyframe<Rectangle>[] walk_left = {
            new Keyframe<Rectangle>( duration: 0.2f, value: new Rectangle(new Point(24 * 0, 36 * 1), (Point)this.Size )),
            new Keyframe<Rectangle>( duration: 0.2f, value: new Rectangle(new Point(24 * 1, 36 * 1), (Point)this.Size )),
            new Keyframe<Rectangle>( duration: 0.2f, value: new Rectangle(new Point(24 * 2, 36 * 1), (Point)this.Size ))
        };

        Keyframe<Rectangle>[] walk_right = {
            new Keyframe<Rectangle>( duration: 0.2f, value: new Rectangle(new Point(24 * 0, 36 * 2), (Point)this.Size )),
            new Keyframe<Rectangle>( duration: 0.2f, value: new Rectangle(new Point(24 * 1, 36 * 2), (Point)this.Size )),
            new Keyframe<Rectangle>( duration: 0.2f, value: new Rectangle(new Point(24 * 2, 36 * 2), (Point)this.Size ))
        };

        Keyframe<Rectangle>[] walk_up = {
            new Keyframe<Rectangle>( duration: 0.2f, value: new Rectangle(new Point(24 * 0, 36 * 3), (Point)this.Size )),
            new Keyframe<Rectangle>( duration: 0.2f, value: new Rectangle(new Point(24 * 1, 36 * 3), (Point)this.Size )),
            new Keyframe<Rectangle>( duration: 0.2f, value: new Rectangle(new Point(24 * 2, 36 * 3), (Point)this.Size ))
        };

        Keyframe<Rectangle>[] walk_down = {
            new Keyframe<Rectangle>( duration: 0.2f, value: new Rectangle(new Point(24 * 0, 0), (Point)this.Size )),
            new Keyframe<Rectangle>( duration: 0.2f, value: new Rectangle(new Point(24 * 1, 0), (Point)this.Size )),
            new Keyframe<Rectangle>( duration: 0.2f, value: new Rectangle(new Point(24 * 2, 0), (Point)this.Size ))
        };

        Keyframe<Rectangle>[] idle_left = {
            new Keyframe<Rectangle>( duration: 0.5f, value: new Rectangle(new Point(24 * 1, 36 * 1), (Point)this.Size ))
        };

        Keyframe<Rectangle>[] idle_right = {
            new Keyframe<Rectangle>( duration: 0.5f, value: new Rectangle(new Point(24 * 1, 36 * 2), (Point)this.Size ))
        };

        Keyframe<Rectangle>[] idle_up = {
            new Keyframe<Rectangle>( duration: 0.5f, value: new Rectangle(new Point(24 * 1, 36 * 3), (Point)this.Size ))
        };

        Keyframe<Rectangle>[] idle_down = {
            new Keyframe<Rectangle>( duration: 0.5f, value: new Rectangle(new Point(24 * 1, 0), (Point)this.Size ))
        };

        var walk_left_anim = new SpriteKeyframeAnimation(GetBehavior<SpriteRenderBehavior>(), "walk_left",      LoopType.Reverse,   walk_left);
        var walk_right_anim = new SpriteKeyframeAnimation(GetBehavior<SpriteRenderBehavior>(), "walk_right",    LoopType.Reverse,   walk_right);
        var walk_up_anim = new SpriteKeyframeAnimation(GetBehavior<SpriteRenderBehavior>(), "walk_up",          LoopType.Reverse,   walk_up);
        var walk_down_anim = new SpriteKeyframeAnimation(GetBehavior<SpriteRenderBehavior>(), "walk_down",      LoopType.Reverse,   walk_down);

        var idle_left_anim = new SpriteKeyframeAnimation(GetBehavior<SpriteRenderBehavior>(), "idle_left",      LoopType.None,   idle_left);
        var idle_right_anim = new SpriteKeyframeAnimation(GetBehavior<SpriteRenderBehavior>(), "idle_right",    LoopType.None,   idle_right);
        var idle_up_anim = new SpriteKeyframeAnimation(GetBehavior<SpriteRenderBehavior>(), "idle_up",          LoopType.None,   idle_up);
        var idle_down_anim = new SpriteKeyframeAnimation(GetBehavior<SpriteRenderBehavior>(), "idle_down",      LoopType.None,   idle_down);


        GetBehavior<AnimationBehavior>().Animations.Add(walk_left_anim);
        GetBehavior<AnimationBehavior>().Animations.Add(walk_right_anim);
        GetBehavior<AnimationBehavior>().Animations.Add(walk_up_anim);
        GetBehavior<AnimationBehavior>().Animations.Add(walk_down_anim);        

        GetBehavior<AnimationBehavior>().Animations.Add(idle_left_anim);
        GetBehavior<AnimationBehavior>().Animations.Add(idle_right_anim);
        GetBehavior<AnimationBehavior>().Animations.Add(idle_up_anim);
        GetBehavior<AnimationBehavior>().Animations.Add(idle_down_anim);
    }

    bool IsMoving = false;

    static int frameCount = 0;
    public override void Move(Vector2 direction)
    {
        Facing = DirectionConstants.GetNearestFacing(direction);
        GetBehavior<AnimationBehavior>().Play(_resolver.Resolve("walk", this));
        IsMoving = true;
        base.Move(Facing * Speed);
    }

    public override void Update(GameTime gameTime)
    {
        frameCount++;
        
        if(!IsMoving)
        {
            GetBehavior<AnimationBehavior>().Play(_resolver.Resolve("idle", this));
        } 
        base.Update(gameTime);
        IsMoving = false;
    }
}
