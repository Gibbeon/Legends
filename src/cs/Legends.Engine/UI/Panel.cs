using System;
using Legends.Engine.Graphics2D.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Legends.Engine.UI;

public class Panel : UISceneObject
{
    public VerticalAlignment        VerticalAlignment   { get; set;}
    public HorizontalAlignment      HorizontalAlignment { get; set;}

    public Panel(): this (null, null)
    {

    }

    public Panel(IServiceProvider services, SceneObject sceneObject) 
        : base(services, sceneObject)
    {

    }
}

public class UISceneObject : SceneObject, IMovable
{
    public Vector2                  Margin              { get; set; }
    public Vector2                  Padding             { get; set; }

    public UISceneObject(): this (null, null)
    {

    }

    public UISceneObject(IServiceProvider services, SceneObject sceneObject) 
        : base(services, sceneObject)
    {

    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        /*var children = Parent.Children.Where(n=> n.GetComponent<BaseUIComponent>() != null && n.Visible);
   
        var width   = children.Sum(n => n.Bounds.BoundingRectangle.Width + n.GetComponent<BaseUIComponent>().Margin.X);
        var height  = children.Max(n => n.Bounds.BoundingRectangle.Height);

        float x = Padding.X;
        float y = Padding.Y;

        switch(HorizontalAlignment)
        {
            case HorizontalAlignment.Justified:
            case HorizontalAlignment.Center:
            case HorizontalAlignment.Left:  
                x = Padding.X + (HorizontalAlignment == HorizontalAlignment.Center      ? (BoundingRectangle.Width - width / 2) : 0); 
                var xPadding =   HorizontalAlignment == HorizontalAlignment.Justified   ? (BoundingRectangle.Width - width / children.Count()) : Padding.X;             
                foreach(var child in children)
                {
                    child.Position = new (x, child.Position.Y);
                    x += child.Bounds.BoundingRectangle.Width
                      +  child.GetComponent<BaseUIComponent>().Margin.X
                      +  xPadding;
                }
            break;
            
            case HorizontalAlignment.Right:   
                x = width - Padding.X;             
                foreach(var child in children)
                {                    
                    child.Position = new (x - child.Bounds.BoundingRectangle.Width - child.GetComponent<BaseUIComponent>().Margin.X, child.Position.Y);
                    x -= child.Bounds.BoundingRectangle.Width
                      -  child.GetComponent<BaseUIComponent>().Margin.X
                      -  Padding.X;
                }
            break;

            case HorizontalAlignment.Fixed:
            break;
        }

        foreach(var child in children)
        {
            switch(VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    child.Position = new (child.Position.X, y + child.GetComponent<BaseUIComponent>().Margin.Y);
                    break;
                case VerticalAlignment.Bottom:
                    y = BoundingRectangle.Height - Padding.Y;
                    child.Position = new (child.Position.X, y - child.GetComponent<BaseUIComponent>().Margin.Y - child.Bounds.BoundingRectangle.Height);
                    break;
                case VerticalAlignment.Middle:
                    child.Position = new (child.Position.X, (BoundingRectangle.Height - child.Bounds.BoundingRectangle.Height) / 2);
                    break;
                 case VerticalAlignment.Fixed:
                    break;
            }
        }*/
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