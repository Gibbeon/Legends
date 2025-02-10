using System;
using Legends.Engine.Graphics2D.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Legends.Engine.UI;

public abstract class BaseUIObject : SceneObject, IMovable
{
    public Vector2                  Margin              { get; set; }
    public Vector2                  Padding             { get; set; }
    public VerticalAlignment        VerticalAlignment   { get; set;}
    public HorizontalAlignment      HorizontalAlignment { get; set;}

    public BaseUIObject(IServiceProvider services, SceneObject sceneObject) 
        : base(services, sceneObject)
    {

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