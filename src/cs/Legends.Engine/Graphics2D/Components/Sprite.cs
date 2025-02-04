using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using Legends.Engine.Graphics2D;

namespace Legends.Engine.Graphics2D.Components;

public class Sprite: Component, ISpriteRenderable
{
    [JsonIgnore] public int             RenderLayerID => 1;
    [JsonIgnore] public Vector2         Position => Parent.Position;
    [JsonIgnore] public bool            Visible => Parent.Visible;
    [JsonIgnore] public IViewState      ViewState => Parent.Scene.Camera;
    public Texture2DRegion              TextureRegion2 { get; set; }
    public OffsetRectangleF             Bounds { get; set; }
    public int                          FrameIndex { get; set;}
    public bool                         FlipHorizontally { get; set; }
    public bool                         FlipVertically { get; set;}
    public Color                        Color { get; set; }
    public RenderState                  RenderState { get; set; }

    public Sprite() : base(AssetType.Dynamic, "") {}
    public Sprite(IServiceProvider services, SceneObject parent) : base(services, parent)
    {
    }

    public override void Initialize()
    {
 
    }

    public override void Reset()
    {

    }

    public override void Draw(GameTime gameTime)
    { 
        Services.Get<IRenderService>().DrawBatched(this);
    }

    public void DrawImmediate(GameTime gameTime, GraphicsResource target = null)
    {
        var spriteBatch = this.GetSpriteBatch(target);

        spriteBatch.Draw(
            ColorMap.Texture,
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