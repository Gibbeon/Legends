using Microsoft.Xna.Framework;
using Legends.Engine.Graphics2D;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework.Graphics;
using System.Security;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Legends.Engine.Graphics2D;

public class SpriteRenderBehavior : BaseBehavior, ISpriteBatchDrawable
{
    public TextureRegion2D? TextureRegion { get; set; }

    public Color Color { get; set; }

    public RenderState? RenderState { get; set; }

    [DefaultValue(SpriteEffects.None)]
    public SpriteEffects Effect  { get; set; }

    [JsonIgnore]
    public bool IsVisible => Parent.IsVisible;

    [JsonIgnore]
    public Vector2 Position => Parent.Position;

    [JsonIgnore]
    public float Rotation => Parent.Rotation;

    [JsonIgnore]
    public Vector2 Scale => Parent.Scale;

    [JsonIgnore]
    public Vector2 Origin => Parent.Origin;

    [JsonIgnore]
    public Texture2D SourceData => TextureRegion.Texture;
    
    [JsonIgnore]
    public IViewState? ViewState => Parent?.GetParentScene().Camera;

    [JsonIgnore]
    public Rectangle SourceBounds{ get => TextureRegion.Bounds; set => SetTextureRegionBounnds(value); }

    [JsonIgnore]
    public Rectangle DestinationBounds => (Rectangle)Parent?.BoundingRectangle; 

    public SpriteRenderBehavior(): this (null, null)
    {

    }
    public SpriteRenderBehavior(SystemServices? services, SceneObject? parent) : base(services, parent)
    {
        Color = Color.White;
    }

    public override void Draw(GameTime gameTime)
    {
        if(IsVisible)
        {
            Parent?.Services?.GetService<IRenderService>().DrawBatched(this);
        }
    }

    public override void Update(GameTime gameTime)
    {
        //base.Update(gameTime);
    }


    public void SetTextureRegionBounnds(Rectangle rect)
    {
        TextureRegion = new TextureRegion2D(TextureRegion?.Texture, rect);
    }

    public override void Dispose()
    {
        
    }
}