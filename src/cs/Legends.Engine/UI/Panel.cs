using System;
using Microsoft.Xna.Framework;

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

public class Panel : BaseUIObject
{
    public Panel(): base(null, null)
    {

    }

    public Panel(IServiceProvider services, SceneObject sceneObject) 
        : base(services, sceneObject)
    {

    }

    public override void Update(GameTime gameTime)
    {
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
}