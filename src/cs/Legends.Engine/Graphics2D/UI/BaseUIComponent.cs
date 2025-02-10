using System;
using System.ComponentModel.Design;
using Legends.Engine.Graphics2D.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Newtonsoft.Json;

namespace Legends.Engine.UI;



public abstract class BaseUIComponent : Component, IRectangularF, ISizable, IMovable, IBounds
{
    [JsonIgnore] public Vector2     Position { get => Parent.Position; set => Parent.Position = value; }
    [JsonIgnore] public RectangleF  BoundingRectangle => GetBoundingRectangle();
    public Vector2                  Margin              { get; set; }
    public Vector2                  Padding             { get; set; }
    public VerticalAlignment        VerticalAlignment   { get; set;}
    public HorizontalAlignment      HorizontalAlignment { get; set;}
    public SizeF                    Size                { get; set; }

    public BaseUIComponent(IServiceProvider services, SceneObject sceneObject) 
        : base(services, sceneObject)
    {

    }

    public virtual RectangleF GetBoundingRectangle()
    {
        return new(Position - Vector2.Zero * Parent.Scale, Size * Parent.Scale);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public static float GetVerticalOffset(VerticalAlignment vAlign, RectangleF component, SizeF containerSize)
    {
        return component.Top + vAlign switch
        {
            VerticalAlignment.Top => 0,
            VerticalAlignment.Bottom => component.Height - containerSize.Height,
            VerticalAlignment.Middle => component.Center.Y - (containerSize.Height / 2),
            _ => 0,
        };
    }

    public float GetHorizontalOffset(HorizontalAlignment hAlign, RectangleF component, SizeF containerSize)
    {
        return component.Left + hAlign switch
        {
            HorizontalAlignment.Left => 0,
            HorizontalAlignment.Right => component.Width - containerSize.Width,
            HorizontalAlignment.Center =>  component.Center.Y - (containerSize.Width / 2),
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