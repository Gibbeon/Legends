using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System;

namespace Legends.Engine.Graphics2D;

public class TextRenderBehavior : BaseBehavior, IBitmapFontBatchDrawable
{
    public Color Color { get; set; }

    public SpriteEffects Effect  { get; set; }

    public string Text {get; set; }

    public BitmapFont Font { get; set; }

    public RenderState? RenderState { get; set; }
    
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
    public IViewState? ViewState => Parent.GetParentScene()?.Camera;
    
    [JsonIgnore]
    public Rectangle SourceBounds => new Rectangle(Position.ToPoint(), (Point)SourceData.MeasureString(Text));

    [JsonIgnore]
    public BitmapFont SourceData => Font;
    public TextRenderBehavior(IServiceProvider? services, SceneObject? parent) : base(services, parent)
    {
        Color = Color.White;
    }

    public override void Update(GameTime gameTime)
    {
        //base.Update(gameTime);
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
        
    }
}