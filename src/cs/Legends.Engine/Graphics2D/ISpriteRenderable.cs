using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Legends.Engine.Graphics2D;


public interface ISpriteRenderable: IRenderable
{
    Rectangle? DestinationBounds { get; }
    Vector2 Position { get; }
    float Rotation { get; }
    Vector2 Scale { get; }
    Color Color { get; set; }
    Vector2 Origin { get; }
    SpriteEffects Effect { get; }
    RenderState RenderState { get; }
    IViewState ViewState { get; }
}

public interface ISpriteRenderable<TType> : ISpriteRenderable
{
    TType SourceData { get; }
}
