using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System;
using Microsoft.Xna.Framework.Content;
using Legends.Engine.Content;

namespace Legends.Engine.Graphics2D;

public enum HorizontalAlignment
{
    Left,
    Center,
    Right
}

public enum VerticalAlignment
{
    Top,
    Middle,
    Bottom
}
public class Label : Component<Sprite>, IBitmapFontBatchDrawable
{
    public Color Color { get; set; }

    public SpriteEffects Effect  { get; set; }

    public string Text {get; set; }

    public Ref<BitmapFont> Font { get; set; }

    public RenderState RenderState { get; set; }
    
    [JsonProperty("halign")]
    public HorizontalAlignment HorizontalAlignment { get; set; }

    [JsonProperty("valign")]
    public VerticalAlignment VerticalAlignment { get; set; }
    
    [JsonIgnore]
    public bool Visible   => Parent != null && (Parent).Visible;

    [JsonIgnore]
    public Vector2 Position => Parent != null ? (Parent).Position + new Vector2(GetHorizontalOffset(), GetVerticalOffset()) : Vector2.Zero;

    [JsonIgnore]
    public float Rotation   => Parent != null ? (Parent).Rotation : 0.0f;
    
    [JsonIgnore]
    public Vector2 Scale    => Parent != null ? (Parent).Scale : Vector2.One;
    
    [JsonIgnore]
    public Vector2 Origin   => Parent != null ? (Parent).Origin : Vector2.Zero;
    
    [JsonIgnore]
    public IViewState ViewState => (Parent).GetParentScene().Camera;
    
    [JsonIgnore]
    public Rectangle SourceBounds => new(Position.ToPoint(), SourceData == null ? Point.Zero : (Point)SourceData.MeasureString(Text));

    [JsonIgnore]
    public BitmapFont SourceData => (BitmapFont)Font;

    public Label() : this(null, null)
    {

    }

    public Label(IServiceProvider services, SceneObject parent) : base(services, parent)
    {
        Color   = Color.White;
        Text    = string.Empty;
    }

    public float GetVerticalOffset()
    {
        return VerticalAlignment switch
        {
            VerticalAlignment.Top => 0,
            VerticalAlignment.Bottom => -((BitmapFont)Font).MeasureString(Text).Height * Scale.Y,
            VerticalAlignment.Middle => -((BitmapFont)Font).MeasureString(Text).Height * Scale.Y / 2,
            _ => 0,
        };
    }

        public float GetHorizontalOffset()
    {
        return HorizontalAlignment switch
        {
            HorizontalAlignment.Left => 0,
            HorizontalAlignment.Right => -((BitmapFont)Font).MeasureString(Text).Width * Scale.X,
            HorizontalAlignment.Center => -((BitmapFont)Font).MeasureString(Text).Width * Scale.Y / 2,
            _ => 0,
        };
    }

    public override void Update(GameTime gameTime)
    {

    }

    public override void Draw(GameTime gameTime)
    {   
        base.Draw(gameTime);

        if(Visible && Parent != null)
        {
            Services.Get<IRenderService>().DrawBatched(this);  
        }      
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}