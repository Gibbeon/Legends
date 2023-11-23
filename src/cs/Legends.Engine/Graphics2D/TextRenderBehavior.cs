using Microsoft.Xna.Framework;
using Legends.Engine.Graphics2D;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace Legends.Engine.Graphics2D;

public class TextRenderBehavior : BaseBehavior, IBitmapFontBatchDrawable
{
    public Color Color { get; set; }

    public SpriteEffects Effect  { get; set; }

    public bool IsVisible { get; set; }

    public Vector2 Position => Parent.Position;

    public float Rotation => Parent.Rotation;

    public Vector2 Scale => Parent.Scale;

    public Vector2 Origin => Parent.Origin;

    public string Text {get; set; }

    public BitmapFont SourceData {get; set; }

    public Rectangle SourceBounds => new Rectangle(Position.ToPoint(), (Point)SourceData.MeasureString(Text));

    public override void Update(GameTime gameTime)
    {
        Parent.Services.GetService<SpriteRenderService>().Current.Layers[0].Drawables.Add(this);        
    }
    
    public TextRenderBehavior(GameObject parent) : base(parent)
    {
        Color = Color.White;
    }
}