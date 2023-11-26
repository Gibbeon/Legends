using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Legends.Engine;

public interface IBatchDrawable
{
    bool IsVisible { get; }
    Rectangle SourceBounds { get; }
    Vector2 Position { get; }
    float Rotation { get; }
    Vector2 Scale { get; }
    Color Color { get; }
    Vector2 Origin { get; }
    SpriteEffects Effect { get; }

    RenderState? RenderState { get; }
}

public interface IBatchDrawable<TType> : IBatchDrawable
{
    TType SourceData { get; }
}
