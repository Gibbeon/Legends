using Microsoft.Xna.Framework.Graphics;

namespace Legends.Engine;


public interface IRenderService
{
    GraphicsDevice  GraphicsDevice { get; }
    RenderState     DefaultRenderState { get; }
    Texture2D?      DefaultTexture { get; }
    void Initialize();
    void DrawBatched(IDrawable drawable);
}
