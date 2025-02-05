
using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Newtonsoft.Json;

namespace Legends.Engine.Graphics2D;

//
// Summary:
//     Represents a region of a texture.
public class Texture2DRegion
{
    public string       Name { get; protected set; }
    public Texture2D    Texture { get; set; }
    public Rectangle    Bounds { get; set; }
    private Size        _tileSize;
    public Size         TileSize { get => _tileSize.IsEmpty ? Bounds.Size : _tileSize; set => _tileSize = value; }

    [JsonIgnore]
    public int Rows => (int)Bounds.Height / (int)TileSize.Height;

    [JsonIgnore]
    public int Columns => (int)Bounds.Width / (int)TileSize.Width;

    [JsonIgnore]
    public int FrameCount => Rows * Columns;

    [JsonIgnore] 
    public Rectangle    this[int index] 
    {
        get { return new Rectangle(Bounds.Location.X + TileSize.Width * (index % Rows), Bounds.Location.Y + TileSize.Height * (index % Columns), TileSize.Width, TileSize.Height); }
    }

    [JsonIgnore] public float TopUV     => (float)Bounds.Top / (float)Texture.Height;

    [JsonIgnore] public float RightUV   => (float)Bounds.Right / (float)Texture.Width;

    [JsonIgnore] public float BottomUV  => (float)Bounds.Bottom / (float)Texture.Height;

    [JsonIgnore] public float LeftUV    => (float)Bounds.Left / (float)Texture.Width;


    public Texture2DRegion()
        : this(null, 0, 0, 0, 0)
    {
    }

    public Texture2DRegion(Texture2D texture)
        : this(texture, 0, 0, texture.Width, texture.Height)
    {
    }

    public Texture2DRegion(Texture2D texture, Rectangle region)
        : this(texture, region.X, region.Y, region.Width, region.Height)
    {
    }

    
    public Texture2DRegion(Texture2D texture, int x, int y, int width, int height)
    {
        Texture     = texture;
        Bounds      = new Rectangle(x, y, width, height);
        TileSize    = new Size(width, height);
    }

    public override string ToString()
    {
        return $"{Name ?? string.Empty} {Bounds}";
    }
}

/*
public class TextureRegion : IInitalizable
{
    [JsonIgnore] public IServiceProvider Services { get; protected set; }

    //[JsonProperty(nameof(Texture))] protected Ref<Texture2D> TextureReference { get; set; }
    //[JsonIgnore] public Texture2D Texture => TextureReference.Get();

    public Texture2DAtlas Texture { get; }

    public Color Color { get; set; }

    [JsonIgnore] public Region2D CurrentRegion => GetSubRegion(Frame, FrameSize);

    [JsonIgnore] public Size TileCount => new ((int)Size.Width / (int)FrameSize.Width,((int)Size.Height / (int)FrameSize.Height));
    
    [JsonIgnore]
    public int FrameCount => Stride * (int)Size.Height / (int)FrameSize.Height;

    [JsonIgnore]
    public int Stride => (int)Size.Width / (int)FrameSize.Width;

    public TextureRegion()
        :this (null)
    {

    }

    public TextureRegion(IServiceProvider services)
        : this(services, null, 0, 0, 0, 0)
    {
    }

    public TextureRegion(IServiceProvider services, Texture2D texture, Rectangle region)
        : this(services, texture, region.X, region.Y, region.Width, region.Height)
    {
    }

    public TextureRegion(IServiceProvider services, Texture2D texture)
        : this(services, texture, 0, 0, texture.Width, texture.Height)
    {
    }

    public TextureRegion(IServiceProvider services, Texture2D texture, int x, int y, int width, int height)
    {
        Services = services;
        TextureReference = texture;
        Position = new Vector2(x, y);
        Size = FrameSize = new SizeF(width, height);
        OriginRelative   = Vector2.Zero;
    }

    public void SetFrame(int frame)
    {        
        Frame = frame;
    }

    public override string ToString()
    {
        return $"{string.Empty} {BoundingRectangle}";
    }

    public void Initialize()
    {
        if(ResourceType == ResourceType.Dynamic)
        {
            TextureReference = new Texture2D(Services.GetGraphicsDevice(), (int)Size.Width, (int)Size.Height);
            Texture.SetData<Color>(Enumerable.Repeat(Color, (int)(Size.Width * Size.Height)).ToArray());
        }

        if(Size == SizeF.Empty) Size = new SizeF(Texture.Width, Texture.Height);        
        if(FrameSize == SizeF.Empty) FrameSize = Size;
    }

    public void Reset()
    {
        Frame = 0;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
*/