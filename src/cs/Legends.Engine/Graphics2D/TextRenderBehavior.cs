using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace Legends.Engine.Graphics2D;

public class TextRenderBehavior : BaseBehavior, IBitmapFontBatchDrawable
{
    public class TextRenderBehaviorDesc : IBehavior.BehaviorDesc
    {
        public string Text;
        public Color Color;
        public string Font;
    }

    public Color Color { get; set; }

    public SpriteEffects Effect  { get; set; }

    public bool IsVisible => Parent.IsVisible;

    public Vector2 Position => Parent.Position;

    public float Rotation => Parent.Rotation;

    public Vector2 Scale => Parent.Scale;

    public Vector2 Origin => Parent.Origin;
    public string Text {get; set; }
    
    public RenderState? RenderState { get; set; }
    public IViewState? ViewState => Parent.ParentScene?.Camera;
    public BitmapFont SourceData => Font;
    public BitmapFont Font { get; set; }
    public Rectangle SourceBounds => new Rectangle(Position.ToPoint(), (Point)SourceData.MeasureString(Text));

    public override void Draw(GameTime gameTime)
    {   
        base.Draw(gameTime);

        if(IsVisible)
        {
            Parent.Services.GetService<IRenderService>().DrawBatched(this);  
        }      
    }

    public override void Update(GameTime gameTime)
    {
        //base.Update(gameTime);
    }

    public TextRenderBehavior(SystemServices services, SceneObject parent) : base(services, parent)
    {
        Color = Color.White;
    }
}