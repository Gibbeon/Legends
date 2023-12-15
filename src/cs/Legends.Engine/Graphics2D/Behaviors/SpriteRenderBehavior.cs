using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.ComponentModel;
using System;
using MonoGame.Extended;

namespace Legends.Engine.Graphics2D;

public class TextureRegion : Spatial
{
    public Ref<Texture2D> Texture { get; set; }

    public TextureRegion(Texture2D texture, Rectangle region)
        : this(texture, region.X, region.Y, region.Width, region.Height)
    {
    }

    public  TextureRegion()
        : this(null, 0, 0, 0, 0)
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

    public override string ToString()
    {
        return $"{string.Empty} {BoundingRectangle}";
    }
}

public class SpriteRenderBehavior : BaseBehavior, ISpriteBatchDrawable
{
    public Ref<TextureRegion> TextureRegion { get; set; }

    public Color Color { get; set; }

    public RenderState RenderState { get; set; }

    [DefaultValue(SpriteEffects.None)]
    public SpriteEffects Effect  { get; set; }

    [JsonIgnore]
    public Texture2D SourceData => (~TextureRegion).Texture;

    [JsonIgnore]
    public bool Visible   => Parent != null && (~Parent).Visible;

    [JsonIgnore]
    public Vector2 Position => Parent != null ? (~Parent).Position : Vector2.Zero;

    [JsonIgnore]
    public float Rotation   => Parent != null ? (~Parent).Rotation : 0.0f;
    
    [JsonIgnore]
    public Vector2 Scale    => Parent != null ? (~Parent).Scale : Vector2.One;
    
    [JsonIgnore]
    public Vector2 Origin   => Parent != null ? (~Parent).Origin : Vector2.Zero;
    
    [JsonIgnore]
    public IViewState ViewState => (~Parent).GetParentScene().Camera.Get();

    [JsonIgnore]
    public Rectangle SourceBounds{ get => TextureRegion == null ? Rectangle.Empty : (~TextureRegion).BoundingRectangle.ToRectangle(); set => SetTextureRegionBounnds(value); }

    [JsonIgnore]
    public Rectangle DestinationBounds => Parent == null ? Rectangle.Empty : (Rectangle)(~Parent).BoundingRectangle; 

    public SpriteRenderBehavior(): this (null, null)
    {

    }
    public SpriteRenderBehavior(IServiceProvider services, SceneObject parent) : base(services, parent)
    {
        Color = Color.White;
    }

    public override void Draw(GameTime gameTime)
    {
        if(Visible)
        {
            (~Parent).Services.Get<IRenderService>().DrawBatched(this);
        }
    }

    public override void Update(GameTime gameTime)
    {
        //base.Update(gameTime);
    }


    public void SetTextureRegionBounnds(Rectangle rect)
    {
        TextureRegion = new TextureRegion((~TextureRegion).Texture, rect);
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}