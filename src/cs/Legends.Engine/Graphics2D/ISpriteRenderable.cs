using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Legends.Engine.Graphics2D;
public interface ISpriteRenderable: IRenderable
{
    Vector2 Position { get; }
    Color Color { get; }
}