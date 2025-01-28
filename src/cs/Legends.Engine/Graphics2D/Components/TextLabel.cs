using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using MonoGame.Extended;

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
    private SizeF _textSize;

    [JsonIgnore] public int RenderLayerID => 1;

    public Color Color { get; set; }

    public SpriteEffects SpriteEffects  { get; set; }

    public string Text {get => _text;  set => SetText(value); }
    private SpriteFont _spriteFont;

    [JsonIgnore]
    public SpriteFont Font => _spriteFont;

    public RenderState RenderState { get; set; }
    
    [JsonProperty("halign")]
    public HorizontalAlignment HorizontalAlignment { get => _halign; set => SetAlignment(value, _valign); }

    [JsonProperty("valign")]
    public VerticalAlignment VerticalAlignment { get => _valign; set => SetAlignment(_halign, value); }
    
    [JsonIgnore]
    public bool Visible => Parent.Visible;

    [JsonIgnore] public SizeF Size => (Parent.Bounds as ISizable).Size;
    [JsonIgnore] public Vector2 Origin => Vector2.Zero;

    [JsonIgnore]
    public Vector2 Position => Parent.AbsolutePosition - Origin * Parent.Scale;

    [JsonIgnore]
    public IViewState ViewState => Parent.Scene.Camera;
    
    [JsonIgnore]
    public Microsoft.Xna.Framework.Rectangle DestinationBounds => (Microsoft.Xna.Framework.Rectangle)new RectangleF(Parent.Position, Size * Parent.Scale);

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
            _textSize           = Font.MeasureString(Text);
            _horizontalOffset   = GetVerticalOffset();
            _verticalOffset     = GetHorizontalOffset();

            //SetSize(_textSize);
            //Parent.Position = new Vector2(-_verticalOffset, -_horizontalOffset);
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

        if (Parent.AbsoluteRotation > 0 || Parent.AbsoluteScale != Vector2.One)
        {
            spriteBatch.DrawString(
                Font,
                Text,
                Vector2.Zero,
                Color,
                Parent.AbsoluteRotation,
                -(Position / Parent.AbsoluteScale),
                Parent.AbsoluteScale,
                SpriteEffects,
                0,
                false);//fontDrawable.DestinationBounds);
        }
        else
        {
            spriteBatch.DrawString(
                Font,
                Text,
                Position,
                Color); // DestinationBounds
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
        $"\tColor: {Color} Font: {Font}\n" + 
        $"\tHAlign: {HorizontalAlignment} VAlign: {VerticalAlignment}\n" + 
        $"\tSpriteEffects: {SpriteEffects} RenderState: {RenderState}";
    }
}