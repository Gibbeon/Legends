using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Legends.Engine.Graphics2D;

public interface ISelfRenderable: IRenderable
{
    void DrawImmediate(GameTime gameTime);
}
