using Microsoft.Xna.Framework.Graphics;

namespace Legends.Engine;


public interface ISpriteFontBatchDrawable : IBatchDrawable<SpriteFont>
{
    public string Text { get; }

}
