using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Legends.Engine;

public interface IBounds
{
    RectangleF  BoundingRectangle { get; }
    bool Contains(Vector2 point);
}
