using Microsoft.Xna.Framework;
using Legends.Engine.Graphics2D;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework.Graphics;
using System.Security;

namespace Legends.Engine.Graphics2D;

public class SpriteRenderBehavior : BaseBehavior, ISpriteBatchDrawable
{
    public TextureRegion2D TextureRegion { get; set; }

    public Color Color { get; set; }

    public SpriteEffects Effect  { get; set; }

    public bool IsVisible => Parent.IsVisible;

    public Vector2 Position => Parent.Position;

    public float Rotation => Parent.Rotation;

    public Vector2 Scale => Parent.Scale;

    public Vector2 Origin => Parent.Origin;

    public Texture2D SourceData => TextureRegion.Texture;

    public RenderState? RenderState { get; set; }
    
    public IViewState? ViewState => Parent.ParentScene?.Camera;

    public Rectangle SourceBounds{ get => TextureRegion.Bounds; set => SetTextureRegionBounnds(value); }

    public Rectangle DestinationBounds => (Rectangle)Parent.BoundingRectangle; 

    public override void Draw(GameTime gameTime)
    {
        if(IsVisible)
        {
            Parent.Services.GetService<IRenderService>().DrawBatched(this);
        }
    }

    public override void Update(GameTime gameTime)
    {
        //base.Update(gameTime);
    }

    public SpriteRenderBehavior(SystemServices services, SceneObject parent) : base(services, parent)
    {
        Color = Color.White;
    }

    public void SetTextureRegionBounnds(Rectangle rect)
    {
        TextureRegion = new TextureRegion2D(TextureRegion.Texture, rect);
    }
}