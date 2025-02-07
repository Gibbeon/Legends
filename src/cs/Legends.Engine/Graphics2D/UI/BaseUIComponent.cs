using System;
using System.ComponentModel.Design;
using Legends.Engine.Graphics2D.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Newtonsoft.Json;

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

public abstract class BaseUIComponent : Component, IRectangularF, ISizable, IMovable, IBounds
{
    [JsonIgnore] public Vector2     Position { get => Parent.Position; set => Parent.Position = value; }
    [JsonIgnore] public RectangleF  BoundingRectangle => new(Position - Vector2.Zero * Parent.Scale, Size * Parent.Scale);
    public Vector2                  Margin              { get; set; }
    public Vector2                  Padding             { get; set; }
    public VerticalAlignment        VerticalAlignment   { get; set;}
    public HorizontalAlignment      HorizontalAlignment { get; set;}
    public SizeF                    Size                { get; set; }

    public BaseUIComponent(IServiceContainer services, SceneObject sceneObject) 
        : base(services, sceneObject)
    {

    }

    public virtual RectangleF Arrange(Rectangle bounds)
    {
        return new RectangleF(Padding.X + GetHorizontalOffset(bounds), Padding.Y + GetVerticalOffset(bounds), Size.Width, Size.Height);
    }

    public override void Update(GameTime gameTime)
    {
        Arrange((Parent.Bounds as IRectangular).BoundingRectangle);
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

    public bool Contains(Vector2 point)
    {
        throw new NotImplementedException();
    }
}