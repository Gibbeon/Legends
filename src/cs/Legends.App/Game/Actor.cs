using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Legends.Engine;
using Legends.Engine.Animation;
using Legends.Engine.Graphics2D;
using Legends.Engine.Resolvers;

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

        Size = new MonoGame.Extended.Size2(24, 26);
        _resolver = new ValueResolver<string, Actor>();
        _resolver.Add("walk", (actor) => { return actor.Facing == DirectionConstants.Left; }, "walk_left");
        _resolver.Add("walk", (actor) => { return actor.Facing == DirectionConstants.Right; }, "walk_right");
        _resolver.Add("walk", (actor) => { return actor.Facing == DirectionConstants.Up; }, "walk_up");
        _resolver.Add("walk", (actor) => { return actor.Facing == DirectionConstants.Down; }, "walk_left");

        AttachBehavior(new SpriteRenderBehavior(this));
        AttachBehavior(new AnimationBehavior(this));

        Keyframe<Rectangle>[] walk_left = {
            new Keyframe<Rectangle>( duration: 0.5f, value: new Rectangle(new Point(26 * 0, 0), (Point)this.Size )),
            new Keyframe<Rectangle>( duration: 0.5f, value: new Rectangle(new Point(26 * 1, 0), (Point)this.Size )),
            new Keyframe<Rectangle>( duration: 0.5f, value: new Rectangle(new Point(26 * 2, 0), (Point)this.Size )),
            new Keyframe<Rectangle>( duration: 0.5f, value: new Rectangle(new Point(26 * 1, 0), (Point)this.Size ))
        };

         Keyframe<Rectangle>[] walk_right = {
            new Keyframe<Rectangle>( duration: 0.5f, value: new Rectangle(new Point(26 * 0, 0), (Point)this.Size )),
            new Keyframe<Rectangle>( duration: 0.5f, value: new Rectangle(new Point(26 * 1, 0), (Point)this.Size )),
            new Keyframe<Rectangle>( duration: 0.5f, value: new Rectangle(new Point(26 * 2, 0), (Point)this.Size )),
            new Keyframe<Rectangle>( duration: 0.5f, value: new Rectangle(new Point(26 * 1, 0), (Point)this.Size ))
        };

         Keyframe<Rectangle>[] walk_up = {
            new Keyframe<Rectangle>( duration: 0.5f, value: new Rectangle(new Point(26 * 0, 0), (Point)this.Size )),
            new Keyframe<Rectangle>( duration: 0.5f, value: new Rectangle(new Point(26 * 1, 0), (Point)this.Size )),
            new Keyframe<Rectangle>( duration: 0.5f, value: new Rectangle(new Point(26 * 2, 0), (Point)this.Size )),
            new Keyframe<Rectangle>( duration: 0.5f, value: new Rectangle(new Point(26 * 1, 0), (Point)this.Size ))
        };

         Keyframe<Rectangle>[] walk_down = {
            new Keyframe<Rectangle>( duration: 0.5f, value: new Rectangle(new Point(26 * 0, 0), (Point)this.Size )),
            new Keyframe<Rectangle>( duration: 0.5f, value: new Rectangle(new Point(26 * 1, 0), (Point)this.Size )),
            new Keyframe<Rectangle>( duration: 0.5f, value: new Rectangle(new Point(26 * 2, 0), (Point)this.Size )),
            new Keyframe<Rectangle>( duration: 0.5f, value: new Rectangle(new Point(26 * 1, 0), (Point)this.Size ))
        };

        var walk_left_anim = new KeyframeAnimation<Rectangle>("walk_left",   walk_left);
        var walk_right_anim = new KeyframeAnimation<Rectangle>("walk_right",   walk_right);
        var walk_up_anim = new KeyframeAnimation<Rectangle>("walk_up",   walk_up);
        var walk_down_anim = new KeyframeAnimation<Rectangle>("walk_down",   walk_down);

        walk_left_anim.FrameChanged += TextureFrameChange;

        GetBehavior<AnimationBehavior>().Animations.Add(walk_left_anim);
        GetBehavior<AnimationBehavior>().Animations.Add(walk_right_anim);
        GetBehavior<AnimationBehavior>().Animations.Add(walk_up_anim);
        GetBehavior<AnimationBehavior>().Animations.Add(walk_down_anim);
    }

    public override void Move(Vector2 direction)
    {
        Facing = DirectionConstants.GetNearestFacing(direction);
        GetBehavior<AnimationBehavior>().Play(0, _resolver.Resolve("walk", this)).AtSpeed(Speed);

        base.Move(-Facing * Speed);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    internal void TextureFrameChange(object? sender, KeyframeAnimation<Rectangle>.KeyframeEventArgs? args)
    {
        GetBehavior<SpriteRenderBehavior>().TextureRegion = new MonoGame.Extended.TextureAtlases.TextureRegion2D(GetBehavior<SpriteRenderBehavior>().TextureRegion.Texture, args.NewValue.Value);

    }
}
