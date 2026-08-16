using System;
using System.Linq;
using Legends.Engine.Graphics2D.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Legends.Engine.UI;

public class Panel : UISceneObject
{
    public VerticalAlignment        VerticalAlignment   { get; set;}
    public HorizontalAlignment      HorizontalAlignment { get; set;}

    public Panel(IServiceProvider services, SceneObject sceneObject = default) 
        : base(services, sceneObject)
    {

    }

    public override void Initialize()
    {
        // An empty rectangle here made every alignment calculation below resolve against a 0x0
        // container. Default to the camera's visible region, evaluated lazily on each access.
        Bounds ??= new BoundsFunction(() =>
        {
            var camera = Scene?.Camera;
            if(camera == null) return RectangleF.Empty;

            var viewport = camera.Viewport;
            return new RectangleF(
                viewport.X - viewport.Width  / 2f,
                viewport.Y - viewport.Height / 2f,
                viewport.Width,
                viewport.Height);
        });

        base.Initialize();
    }

    public override void Update(GameTime gameTime)
    {
        AutoArrange();

        base.Update(gameTime);
    }

    public void AutoArrange()
    {
        // Materialised once: this used to be a lazy query re-evaluated five times per frame,
        // and Max() on it threw as soon as the panel was empty or every child was hidden.
        var children = GetChildren<UISceneObject>().Where(n => n.Visible).ToList();

        if(children.Count == 0) return;

        var bounds  = BoundingRectangle;
        var width   = children.Sum(n => n.Bounds.BoundingRectangle.Width + n.Margin.X);

        float x;

        switch(HorizontalAlignment)
        {
            case HorizontalAlignment.Justified:
            case HorizontalAlignment.Center:
            case HorizontalAlignment.Left:
                // (Width - width) / 2 centres the run of children; 'Width - width / 2' did not.
                x = HorizontalAlignment == HorizontalAlignment.Center
                        ? bounds.Left + (bounds.Width - width) / 2
                        : bounds.Left + Padding.X;

                var xPadding = HorizontalAlignment == HorizontalAlignment.Justified && children.Count > 1
                        ? (bounds.Width - width) / (children.Count - 1)
                        : Padding.X;

                foreach(var child in children)
                {
                    child.Position = new (x, child.Position.Y);
                    x += child.Bounds.BoundingRectangle.Width
                      +  child.Margin.X
                      +  xPadding;
                }
            break;

            case HorizontalAlignment.Right:
                // Start far enough left that the run finishes flush against the right edge.
                x = bounds.Right - Padding.X - (width + Padding.X * (children.Count - 1));
                foreach(var child in children)
                {
                    child.Position = new (x, child.Position.Y);
                    x += child.Bounds.BoundingRectangle.Width
                      +  child.Margin.X
                      +  Padding.X;
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
                    child.Position = new (child.Position.X, bounds.Top + Padding.Y + child.Margin.Y);
                    break;
                case VerticalAlignment.Bottom:
                    child.Position = new (child.Position.X, bounds.Bottom - Padding.Y - child.Margin.Y - child.Bounds.BoundingRectangle.Height);
                    break;
                case VerticalAlignment.Middle:
                    child.Position = new (child.Position.X, bounds.Top + (bounds.Height - child.Bounds.BoundingRectangle.Height) / 2);
                    break;
                 case VerticalAlignment.Fixed:
                    break;
            }
        }
    }
}

public class UISceneObject : SceneObject, IMovable
{
    public Vector2                  Margin              { get; set; }
    public Vector2                  Padding             { get; set; }

    public UISceneObject(IServiceProvider services, SceneObject sceneObject = default) 
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
            HorizontalAlignment.Center =>  component.Center.X - (containerSize.Width / 2),
            _ => 0,
        };
    }

    public override void Dispose()
    {
        // Was empty, which severed teardown for every UI subtree. It is safe to chain again now
        // that MultiStateComponent.Dispose() no longer throws.
        base.Dispose();
    }

    public override void Initialize()
    {
        base.Initialize();
    }
}