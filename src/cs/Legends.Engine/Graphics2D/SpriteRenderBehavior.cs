using Microsoft.Xna.Framework;
using Legends.Engine.Graphics2D;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework.Graphics;

namespace Legends.Engine.Graphics2D;

public class SpriteRenderBehavior : BaseBehavior, ISpriteBatchDrawable
{
    public TextureRegion2D TextureRegion { get; set; }

    public Color Color { get; set; }

    public SpriteEffects Effect  { get; set; }

    public bool IsVisible { get; set; }

    public Vector2 Position => Parent.Position;

    public float Rotation => Parent.Rotation;

    public Vector2 Scale => Parent.Scale;

    public Vector2 Origin => Parent.Origin;

    public Texture2D SourceData => TextureRegion.Texture;

    public Rectangle SourceBounds => TextureRegion.Bounds;

    public override void Update(GameTime gameTime)
    {
        Parent.Services.GetService<SpriteRenderService>().Current.Layers[0].Drawables.Add(this);        
    }
    
    public SpriteRenderBehavior(GameObject parent) : base(parent)
    {
        Color = Color.White;
    }
}