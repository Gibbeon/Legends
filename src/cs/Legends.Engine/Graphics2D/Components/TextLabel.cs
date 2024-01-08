using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System;
using Microsoft.Xna.Framework.Content;
using Legends.Engine.Content;

namespace Legends.Engine.Graphics2D.Components;

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
public class TextLabel : Component, IBitmapFontBatchRenderable
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
    public bool Visible   => Parent.Visible;

    [JsonIgnore]
    public Vector2 Position => Parent.Position + new Vector2(GetHorizontalOffset(), GetVerticalOffset());

    [JsonIgnore]
    public float Rotation   => Parent.Rotation;
    
    [JsonIgnore]
    public Vector2 Scale    => Parent.Scale;
    
    [JsonIgnore]
    public Vector2 Origin   => Parent.Origin + new Vector2(GetHorizontalOffset(), GetVerticalOffset());
    
    [JsonIgnore]
    public IViewState ViewState => Parent.Scene.Camera;

    [JsonIgnore]
    public BitmapFont SourceData => (BitmapFont)Font;

    public Rectangle DestinationBounds => new(Position.ToPoint(), SourceData == null ? Point.Zero : (Point)SourceData.MeasureString(Text));

    public TextLabel() : this(null, null)
    {

    }

    public TextLabel(IServiceProvider services, SceneObject parent) : base(services, parent)
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

    public override void Initialize()
    {
        throw new NotImplementedException();
    }

    public override void Reset()
    {
        throw new NotImplementedException();
    }
}