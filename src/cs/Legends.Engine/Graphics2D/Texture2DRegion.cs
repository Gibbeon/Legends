
using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Legends.Engine.Graphics2D;

//
// Summary:
//     Represents a region of a texture.
public class Texture2DRegion
{
    //
    // Summary:
    //     Gets the name assigned to this texture region when it was created.
    public string Name { get; set; }

    //
    // Summary:
    //     Gets the texture associated with this texture region.
    public Texture2D Texture { get; set;  }

    //
    // Summary:
    //     Gets the top-left x-coordinate of the texture region within the texture.
    public int X { get;  set; }

    //
    // Summary:
    //     Gets the top-left y-coordinate of the texture region within the texture.
    public int Y { get; set;  }

    //
    // Summary:
    //     Gets the width, in pixels, of the texture region.
    public int Width { get; set;  }

    //
    // Summary:
    //     Gets the height, in pixels, of the texture region.
    public int Height { get;  set; }

    //
    // Summary:
    //     Gets the size of the texture region.
    public Size Size { get;  set; }

    //
    // Summary:
    //     Gets or sets the user-defined data associated with this texture region.
    public object Tag { get; set; }

    //
    // Summary:
    //     Gets the bounds of the texture region within the texture.
    public Rectangle Bounds { get;  set; }

    //
    // Summary:
    //     Gets the top UV coordinate of the texture region.
    public float TopUV { get;  set; }

    //
    // Summary:
    //     Gets the right UV coordinate of the texture region.
    public float RightUV { get;  set; }

    //
    // Summary:
    //     Gets the bottom UV coordinate of the texture region.
    public float BottomUV { get;  set; }

    //
    // Summary:
    //     Gets the left UV coordinate of the texture region.
    public float LeftUV { get;  set; }

    //
    // Summary:
    //     Initializes a new instance of the MonoGame.Extended.Graphics.Texture2DRegion
    //     class representing the entire texture.
    //
    // Parameters:
    //   texture:
    //     The texture to create the region from.
    //
    // Exceptions:
    //   T:System.ArgumentNullException:
    //     Thrown if texture is null.
    //
    //   T:System.ObjectDisposedException:
    //     Thrown if texture has been disposed prior.

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

    //
    // Summary:
    //     Initializes a new instance of the MonoGame.Extended.Graphics.Texture2DRegion
    //     class with the specified region of the texture and name.
    //
    // Parameters:
    //   texture:
    //     The texture to create the region from.
    //
    //   x:
    //     The top-left x-coordinate of the region within the texture.
    //
    //   y:
    //     The top-left y-coordinate of the region within the texture.
    //
    //   width:
    //     The width, in pixels, of the region.
    //
    //   height:
    //     The height, in pixels, of the region.
    //
    //   name:
    //     The name of the texture region.
    //
    // Exceptions:
    //   T:System.ArgumentNullException:
    //     Thrown if texture is null.
    //
    //   T:System.ObjectDisposedException:
    //     Thrown if texture has been disposed prior.
    public Texture2DRegion(Texture2D texture, int x, int y, int width, int height)
    {
        //ArgumentNullException.ThrowIfNull(texture, "texture");
        //if (texture.IsDisposed)
        //{
        //    throw new ObjectDisposedException("texture");
        //}


        Texture = texture;
        X = x;
        Y = y;
        //Width = width;
        //Height = height;
        Bounds = new Rectangle(x, y, width, height);
        Size = new Size(width, height);
        //TopUV = (float)Bounds.Top / (float)texture.Height;
        //RightUV = (float)Bounds.Right / (float)texture.Width;
        //BottomUV = (float)Bounds.Bottom / (float)texture.Height;
        //LeftUV = (float)Bounds.Left / (float)texture.Width;
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