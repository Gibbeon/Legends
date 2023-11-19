using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LitEngine.Framework.Graphics
{
    public interface IDrawable2D : IDrawable
    {
        void DrawBatched(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
