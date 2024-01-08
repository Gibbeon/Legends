using MonoGame.Extended.BitmapFonts;

namespace Legends.Engine.Graphics2D;

public interface IBitmapFontBatchRenderable : ISpriteRenderable<BitmapFont>
{
    public string Text { get; }
}
