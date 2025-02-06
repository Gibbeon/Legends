using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Newtonsoft.Json;
using System;

namespace Legends.Engine.Graphics2D.Components;

public class Sprite: Component, ISpriteRenderable
{
    [JsonIgnore] public int             RenderLayerID => 1;
    [JsonIgnore] public Vector2         Position => Parent.Position;
    [JsonIgnore] public bool            Visible => Parent.Visible;
    [JsonIgnore] public IViewState      ViewState => Parent.Scene.Camera;
    public Texture2DRegion              TextureRegion { get; set; }
    public RectangleF                   Bounds { get; set; }
    public int                          FrameIndex { get; set;}
    public bool                         FlipHorizontally { get; set; }
    public bool                         FlipVertically { get; set;}
    public Color                        Color { get; set; }
    public RenderState                  RenderState { get; set; }

    public Sprite()
    {
        
    }

    public Sprite(IServiceProvider services, SceneObject parent, string assetName = null) : base(services, parent, assetName)
    {

    }

    public override void Initialize()
    {        
        if(Bounds.Size.IsEmpty) 
            Bounds = new RectangleF(Bounds.Position, new SizeF(TextureRegion.TileSize.Width, TextureRegion.TileSize.Height));

        Reset();        
    }

    public override void Reset()
    {
        Color = Color.White; 
        TextureRegion.Texture.Reset();
    }

    public override void Draw(GameTime gameTime)
    { 
        Services.Get<IRenderService>().DrawItem(this);
    }

    public void DrawImmediate(GameTime gameTime, RenderSurface target)
    {
        var spriteBounds = (Microsoft.Xna.Framework.Rectangle)Parent.LocalToWorld(Bounds);

//Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth
        target.SpriteBatch.Draw(
            TextureRegion.Texture,
            spriteBounds,
            TextureRegion[FrameIndex],
            Color,
            Parent.Rotation,
            Vector2.Zero,
            (FlipHorizontally ? SpriteEffects.FlipHorizontally : SpriteEffects.None) | 
            (FlipVertically   ? SpriteEffects.FlipVertically   : SpriteEffects.None),
            0
        );
            
            //TextureRegion.Texture,
            //spriteBounds,// - Origin * Parent.Scale,
            //TextureRegion.Bounds,
            //Color,
            //Parent.Rotation,
            //Vector2.Zero,//Origin,
            //Vector2.One,//Parent.Scale,
            //(FlipHorizontally ? SpriteEffects.FlipHorizontally : SpriteEffects.None) | 
            //(FlipVertically   ? SpriteEffects.FlipVertically   : SpriteEffects.None),
            //0);
    }
  
    
    public override void Dispose()
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