using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Newtonsoft.Json;

namespace Legends.Engine.Graphics2D;

public class TextureRegion : Spatial, IInitalizable
{
    public Ref<Texture2D> Texture { get; set; }
    public Size2 Slice { get; set; }
    private int _frame;
    public int Frame { get => _frame; protected set => _frame = value; }

    [JsonIgnore]
    public int TileCount => (int)(Texture.Get().Width / Slice.Width * (Texture.Get().Height / Slice.Height));

    public  TextureRegion()
        : this(null, 0, 0, 0, 0)
    {
    }

    public TextureRegion(Texture2D texture, Rectangle region)
        : this(texture, region.X, region.Y, region.Width, region.Height)
    {
    }

    public TextureRegion(Texture2D texture)
        : this(texture, 0, 0, texture.Width, texture.Height)
    {
    }

    public TextureRegion(Texture2D texture, int x, int y, int width, int height) : base()
    {
        Texture = texture;
        Position = new Vector2(x, y);
        Size = new Size2(width, height);
    }

    public void SetFrame(int frame)
    {        
        _frame = frame;

        OffsetPosition = new Vector2(
            (int)(_frame * Slice.Width) % (int)Size.Width, 
            (int)(_frame * Slice.Width / (int)Size.Width) * Slice.Height
        );
    }

    public override string ToString()
    {
        return $"{string.Empty} {BoundingRectangle}";
    }

    public override RectangleF GetBoundingRectangle()
    {
        return new RectangleF(Position - Origin, Slice);
    }

    public void Initialize()
    {
        if(Slice == Size2.Empty) Slice = Size;

        OffsetPosition = new Vector2(
            (int)(_frame * Slice.Width) % (int)Size.Width, 
            (int)(_frame * Slice.Width / (int)Size.Width) * Slice.Height
        );
    }

    public void Reset()
    {
        _frame = 0;
    }

    public void Dispose()
    {

    }
}