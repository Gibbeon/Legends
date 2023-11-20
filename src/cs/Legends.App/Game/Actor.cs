using Microsoft.Xna.Framework;
using SlyEngine.Graphics2D;
using SlyEngine.Resolvers;

namespace Legends.Game;

public class Actor : SpatialNode
{
    public Sprite Body;
    public Vector2 Facing;
    public float Speed;
    
    ValueResolver<string, Actor> _resolver;

    public Actor()
    {
        Speed = 1.0f;
        Body = new Sprite(this);
        Facing = DirectionConstants.Down;
        _resolver = new ValueResolver<string, Actor>();
        _resolver.Add("walk", (actor) => { return actor.Facing == DirectionConstants.Left; }, "walk_left");
        _resolver.Add("walk", (actor) => { return actor.Facing == DirectionConstants.Right; }, "walk_right");
        _resolver.Add("walk", (actor) => { return actor.Facing == DirectionConstants.Up; }, "walk_up");
        _resolver.Add("walk", (actor) => { return actor.Facing == DirectionConstants.Down; }, "walk_left");
    }

    public override void Move(Vector2 direction)
    {
        Facing = DirectionConstants.GetNearestFacing(direction);
        Body.Animation.Play(0, _resolver.Resolve("walk", this)).AtSpeed(Speed);

        base.Move(Facing * Speed);
    }
}
