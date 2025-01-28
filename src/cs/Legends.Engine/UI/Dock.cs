using System;
using System.ComponentModel.Design;
using System.Text.Json.Serialization;
using Legends.Engine.Graphics2D.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using MonoGame.Extended;

namespace Legends.Engine.UI;

[Flags]
public enum Anchor
{
    None = 0,
    Left = 1,
    Right = 2,
    Top = 4,
    Bottom = 8,
    Center = 16,
}

public static class RectangleFExtensions2
{
    public static RectangleF Transform(this RectangleF rect, Vector2 offset)
    {
        rect.Offset(offset);
        return rect;
    }
}

public class UIComponent2D : SceneObject
{
    public Vector2 Margin { get; set; }
    public Vector2 Padding { get; set; }
    public VerticalAlignment VerticalAlignment { get; set;}
    public HorizontalAlignment HorizontalAlignment { get; set;}
    [JsonIgnore] SizeF Size => new SizeF(0, 0); //Parent.Bounds.GetBoundingRectangle().Size;

    public UIComponent2D(IServiceContainer services, SceneObject sceneObject) 
        : base(services, sceneObject)
    {

    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

    }

    public float GetVerticalOffset(RectangleF bounds)
    {
        return bounds.Top + VerticalAlignment switch
        {
            VerticalAlignment.Top => 0,
            VerticalAlignment.Bottom => bounds.Height - Size.Height,
            VerticalAlignment.Middle => bounds.Center.Y - (Size.Height / 2),
            _ => 0,
        };
    }

    public float GetHorizontalOffset(RectangleF bounds)
    {
        return bounds.Left + HorizontalAlignment switch
        {
            HorizontalAlignment.Left => 0,
            HorizontalAlignment.Right => bounds.Width - Size.Width,
            HorizontalAlignment.Center =>  bounds.Center.Y - (Size.Width / 2),
            _ => 0,
        };
    }

    public override void Dispose()
    {
        
    }

    public override void Initialize()
    {

    }
}