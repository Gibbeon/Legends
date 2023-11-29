using MonoGame.Extended.BitmapFonts;

namespace Legends.Engine;

public interface IBitmapFontBatchDrawable : IBatchDrawable<BitmapFont>
{
    public string? Text { get; }

}
