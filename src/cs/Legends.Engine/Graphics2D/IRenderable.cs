using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Legends.Engine.Graphics2D;

public interface IRenderable
{
    bool Visible { get; }
    RenderState RenderState { get; }
    IViewState ViewState { get; }
    void DrawImmediate(GameTime gameTime, GraphicsResource target = null);
}