using Autofac;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Newtonsoft.Json;
using System;

namespace Legends.Engine.Graphics2D.Components;

public class Sprite : Asset
{
    public Texture2DRegion              TextureRegion { get; set; }
    public int                          FrameIndex { get; set;}
    public bool                         FlipHorizontally { get; set; }
    public bool                         FlipVertically { get; set;}
    private Color                       _color = Color.White;
    public Color                        Color { get => _color; set => _color = value; }
    public RenderState                  RenderState { get; set; }
    public SizeF                        Size { get; set; }
    public Vector2                      Origin { get; set; }

    public Sprite(IServiceProvider services, string assetName): base(services, assetName)
    {

    }

    public void DrawTo(RenderSurface target, Rectangle drawAt, float rotation = 0.0f)
    {
        target.SpriteBatch.Draw(
            TextureRegion.Texture,
            drawAt,
            TextureRegion[FrameIndex],
            Color,
            rotation,
            Vector2.Zero,
            (FlipHorizontally ? SpriteEffects.FlipHorizontally : SpriteEffects.None) | 
            (FlipVertically   ? SpriteEffects.FlipVertically   : SpriteEffects.None),
            0
        );
    }

    public override void Initialize()
    {
    }

    public override void Reset()
    {
    }

    public override void Dispose()
    {
    }
}
public class SpriteRenderable: Sprite, IComponent, ISpriteRenderable, IRectangularF, ISizable, IBounds
{
    [JsonIgnore] public int             RenderLayerID => 1;
    [JsonIgnore] public Vector2         Position { get => Parent.Position; set => Parent.Position = value; }
    [JsonIgnore] public bool            Visible => Parent.Visible;
    [JsonIgnore] public IViewState      ViewState => Parent.Scene.Camera;
    [JsonIgnore] public RectangleF      BoundingRectangle => new(Position - Origin * Parent.Scale, Size * Parent.Scale);
    [JsonIgnore] public SceneObject     Parent { get; private set; }

    public SpriteRenderable(): base(null, null) // don't do this, should use the better constructor model
    {

    }

    public SpriteRenderable(IServiceProvider services, SceneObject parent, string assetName = null) : base(services, assetName)
    {
        Parent = parent;
    }

    public override void Initialize()
    {        
        if(Size.IsEmpty) 
        {
            Size = new SizeF(TextureRegion.TileSize.Width, TextureRegion.TileSize.Height);
        }

        Reset();        
    }

    public override void Reset()
    {
        TextureRegion.Texture.Reset();
    }

    public void Draw(GameTime gameTime)
    { 
        Services.Get<IRenderService>().DrawItem(this);
    }

    public void DrawImmediate(GameTime gameTime, RenderSurface target)
    {
        DrawTo(target, (Rectangle)BoundingRectangle, Parent.Rotation);
    }
  
    
    public override void Dispose()
    {

    }

    public bool Contains(Vector2 point)
    {
        throw new NotImplementedException();
    }

    public void Update(GameTime gameTime)
    {
        
    }
}


/******************************************************************************

public class Sprite : Component, ISpriteRenderable
{
    [JsonIgnore]
    public int RenderLayerID => 1;

    [JsonProperty(nameof(TextureRegion))]
    protected Ref<TextureRegion> TextureRegionReference { get; set; }
    public Color Color { get; set; }
    public RenderState RenderState { get; set; }
    public SpriteEffects SpriteEffect  { get; set; }

    [JsonIgnore] public bool            Visible  => Parent.Visible;
    [JsonIgnore] public Vector2         Position => Parent.Position;
    [JsonIgnore] public TextureRegion   TextureRegion => TextureRegionReference.Get();
    [JsonIgnore] public IViewState      ViewState => Parent.Scene.Camera;
    [JsonIgnore] public Region2D        BoundingRegion => TextureRegion.CurrentRegion;

    public Sprite(): this (null, null)
    {

    }
    public Sprite(IServiceProvider services, SceneObject parent) : base(services, parent)
    {
        Color = Color.White;
    }

    public override void Draw(GameTime gameTime)
    {
        if(Visible)
        {
            Services.Get<IRenderService>().DrawBatched(this);
        }
    }

     public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public override void Initialize()
    {
        if(Parent.Bounds == null) 
        {
            Parent.Bounds = new SpriteBounds(this);
        }
    }

    public override void Reset()
    {
        
    }

    public void DrawImmediate(GameTime gameTime, GraphicsResource target = null)
    {
        var spriteBatch = this.GetSpriteBatch(target);

        spriteBatch.Draw(
            TextureRegion.Texture,
            Parent.Position - Origin * Parent.Scale,
            (Microsoft.Xna.Framework.Rectangle)TextureRegion.CurrentRegion.BoundingRectangle,
            Color,
            Parent.Rotation,
            Vector2.Zero,//Origin,
            Vector2.One,//Parent.Scale,
            SpriteEffect,
            0);

        if(target is not SpriteBatch)
            spriteBatch?.End();
    }
}
*/