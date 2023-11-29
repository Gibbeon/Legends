using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Legends.Engine;


public interface ISpriteBatchDrawable : IBatchDrawable<Texture2D>
{
    Rectangle DestinationBounds { get; }
}
