using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Legends.Engine.Graphics2D;

public interface IRenderable
{
    IServiceProvider Services { get; }
    bool Visible { get; }
    RenderState RenderState { get; }
    IViewState ViewState { get; }
    int RenderLayerID { get; }
    void DrawImmediate(GameTime gameTime, GraphicsResource target = null);
}