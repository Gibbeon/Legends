using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System;
using MonoGame.Extended;
using System.Net.Sockets;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

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

public class TextLabel : Component, ISpriteRenderable
{
    private string _text;
    private HorizontalAlignment _halign;
    private VerticalAlignment _valign;
    private float _verticalOffset;
    private float _horizontalOffset;
    private Size2 _textSize;

    public Color Color { get; set; }

    public SpriteEffects SpriteEffects  { get; set; }

    public string Text {get => _text;  set => SetText(value); }

    [JsonProperty(nameof(Font))]
    public Ref<BitmapFont> FontReference { get; set; }

    [JsonIgnore]
    public BitmapFont Font => FontReference.Get();

    public RenderState RenderState { get; set; }
    
    [JsonProperty("halign")]
    public HorizontalAlignment HorizontalAlignment { get => _halign; set => SetAlignment(value, _valign); }

    [JsonProperty("valign")]
    public VerticalAlignment VerticalAlignment { get => _valign; set => SetAlignment(_halign, value); }
    
    [JsonIgnore]
    public bool Visible => Parent.Visible;

    [JsonIgnore]
    public Vector2 Position => Parent.Position;

    [JsonIgnore]
    public IViewState ViewState => Parent.Scene.Camera;
    
    [JsonIgnore]
    public Rectangle DestinationBounds => (Rectangle)Parent.BoundingRectangle;

    [JsonIgnore]
    protected bool IsDirty { get; set; }

    public TextLabel() : this(null, null)
    {

    }

    public TextLabel(IServiceProvider services, SceneObject parent) : base(services, parent)
    {
        Color   = Color.White;
        Text    = string.Empty;
    }

    public void SetAlignment(HorizontalAlignment halign, VerticalAlignment valign)
    {
        _halign = halign;
        _valign = valign;
        IsDirty = true;
    }

    public void SetText(string text)
    {
        _text = text;
        IsDirty = true;
    }

    public void Resize()
    {
        if(IsDirty) {
            _textSize = Font.MeasureString(Text);
            _horizontalOffset = GetVerticalOffset();
            _verticalOffset = GetHorizontalOffset();

            Parent.SetSize(_textSize);
            Parent.Origin = new Vector2(-_verticalOffset, -_horizontalOffset);
            IsDirty = false;
        }
    }

    public float GetVerticalOffset()
    {
        return VerticalAlignment switch
        {
            VerticalAlignment.Top => 0,
            VerticalAlignment.Bottom => -_textSize.Height * Parent.Scale.Y,
            VerticalAlignment.Middle => -_textSize.Height * Parent.Scale.Y / 2,
            _ => 0,
        };
    }

    public float GetHorizontalOffset()
    {
        return HorizontalAlignment switch
        {
            HorizontalAlignment.Left => 0,
            HorizontalAlignment.Right => -_textSize.Width * Parent.Scale.X,
            HorizontalAlignment.Center => -_textSize.Width * Parent.Scale.X / 2,
            _ => 0,
        };
    }

    public override void Draw(GameTime gameTime)
    {   
        base.Draw(gameTime);

        if(Visible && Parent != null)
        {
            Services.Get<IRenderService>().DrawBatched(this);  
        }      
    }

    public void DrawImmediate(GameTime gameTime, GraphicsResource target = null)
    {
        if (string.IsNullOrEmpty(Text))
        {
            return;
        }

        var spriteBatch = this.GetSpriteBatch(target);

        if (Parent.Rotation > 0 || Parent.Scale != Vector2.One)
        {
            spriteBatch.DrawString(
                Font,
                Text,
                Vector2.Zero,
                Color,
                Parent.Rotation,
                -(Position / Parent.Scale),
                Parent.Scale,
                SpriteEffects,
                0,
                null);//fontDrawable.DestinationBounds);
        }
        else
        {
            spriteBatch.DrawString(
                Font,
                Text,
                Position,
                Color,
                null); // DestinationBounds
        }

        if(target is not SpriteBatch)
            spriteBatch?.End();
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public override void Initialize()
    { 
        Resize();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        Resize();
    }

    public override void Reset()
    { 

    }

    public override string ToString()
    {
        return  $"Component: {GetType().Name}\n" + 
        $"\tColor: {Color} Font: {FontReference}\n" + 
        $"\tHAlign: {HorizontalAlignment} VAlign: {VerticalAlignment}\n" + 
        $"\tSpriteEffects: {SpriteEffects} RenderState: {RenderState}";
    }
}