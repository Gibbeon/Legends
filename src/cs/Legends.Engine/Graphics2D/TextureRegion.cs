using System;
using System.Linq;
using Autofac;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Newtonsoft.Json;

namespace Legends.Engine.Graphics2D;

public enum ResourceType
{
    Static,
    Dynamic
}

public class TextureRegion : Box2D, IInitalizable
{
    [JsonIgnore]
    public IServiceProvider Services { get; protected set; }

    [JsonProperty(nameof(Texture))]
    protected Ref<Texture2D> TextureReference { get; set; }

    [JsonIgnore]
    public Texture2D Texture => TextureReference.Get();

    public SizeF Slice { get; set; }
    public int Frame { get; set; }
    public ResourceType ResourceType { get; set; }
    public Color Color { get; set; }

    public Vector2 Position { get; set; }

    [JsonIgnore]
    public Size TileCount => new ((int)Size.Width / (int)Slice.Width,((int)Size.Height / (int)Slice.Height));

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

    public TextureRegion(IServiceProvider services, Texture2D texture, int x, int y, int width, int height) : base()
    {
        Services = services;
        TextureReference = texture;
        Position = new Vector2(x, y);
        Size = new SizeF(width, height);
    }

    public void SetFrame(int frame)
    {        
        Frame = frame;

        //OffsetPosition = new Vector2(
        //    (int)(Frame * Slice.Width) % (int)Size.Width, 
        //    (int)(Frame * Slice.Width / (int)Size.Width) * Slice.Height
        //);
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
        if(Slice == SizeF.Empty) Slice = Size;

        //OffsetPosition = new Vector2(
        //    (int)(Frame * Slice.Width) % (int)Size.Width, 
        //    (int)(Frame * Slice.Width / (int)Size.Width) * Slice.Height
        //);
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