using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System;
using Microsoft.Xna.Framework.Content;

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
public class TextRenderBehavior : BaseBehavior, IBitmapFontBatchDrawable
{
    public Color Color { get; set; }

    public SpriteEffects Effect  { get; set; }

    public string Text {get; set; }

    public BitmapFont Font { get; set; }

    public RenderState RenderState { get; set; }
    
[JsonProperty("halign")]
    public HorizontalAlignment HorizontalAlignment { get; set; }

    [JsonProperty("valign")]
    public VerticalAlignment VerticalAlignment { get; set; }
    
    [JsonIgnore]
    public bool IsVisible   => Parent != null && Parent.IsVisible;

    [JsonIgnore]
    public Vector2 Position => Parent != null ? Parent.Position + new Vector2(GetHorizontalOffset(), GetVerticalOffset()) : Vector2.Zero;

    [JsonIgnore]
    public float Rotation   => Parent != null ? Parent.Rotation : 0.0f;
    
    [JsonIgnore]
    public Vector2 Scale    => Parent != null ? Parent.Scale : Vector2.One;
    
    [JsonIgnore]
    public Vector2 Origin   => Parent != null ? Parent.Origin : Vector2.Zero;
    
    [JsonIgnore]
    public IViewState ViewState => Parent?.GetParentScene()?.Camera;
    
    [JsonIgnore]
    public Rectangle SourceBounds => new(Position.ToPoint(), SourceData == null ? Point.Zero : (Point)SourceData.MeasureString(Text));

    [JsonIgnore]
    public BitmapFont SourceData => Font;

    public TextRenderBehavior() : this(null, null)
    {

    }

    public TextRenderBehavior(IServiceProvider services, SceneObject parent) : base(services, parent)
    {
        Color   = Color.White;
        Text    = string.Empty;
    }

    public float GetVerticalOffset()
    {
        switch (VerticalAlignment)
        {
            case VerticalAlignment.Top:     return 0;
            case VerticalAlignment.Bottom:  return -((BitmapFont)Font).MeasureString(Text).Height * Scale.Y;
            case VerticalAlignment.Middle:  return -((BitmapFont)Font).MeasureString(Text).Height * Scale.Y / 2;
            default:
                return 0;
        }
    }

        public float GetHorizontalOffset()
    {
        switch (HorizontalAlignment)
        {
            case HorizontalAlignment.Left:      return 0;
            case HorizontalAlignment.Right:     return -((BitmapFont)Font).MeasureString(Text).Width * Scale.X;
            case HorizontalAlignment.Center:    return -((BitmapFont)Font).MeasureString(Text).Width * Scale.Y/ 2;
            default:
                return 0;
        }
    }

    public override void Update(GameTime gameTime)
    {

    }

    public override void Draw(GameTime gameTime)
    {   
        base.Draw(gameTime);

        if(IsVisible && Parent != null)
        {
            Parent.Services?.Get<IRenderService>().DrawBatched(this);  
        }      
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}