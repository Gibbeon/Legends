using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Legends.Engine.Graphics2D.Primitives;

public abstract class Drawable : Asset, IBounds
{   
    private Color                       _color = Color.White;
    public Color                        Color { get => _color; set => _color = value; }
    public RenderState                  RenderState { get; set; }
    public SizeF                        Size { get; set; }
    public Vector2                      Origin { get; set; }
    [JsonIgnore] public abstract RectangleF          BoundingRectangle { get; }

    public Drawable(IServiceProvider services): base(services)
    {

    }

    public abstract void DrawTo(RenderSurface target, Vector2 drawTo, float rotation = 0.0f);

    public bool Contains(Vector2 point)
    {
        throw new NotImplementedException();
    }
}

public class MultiDrawable : Drawable
{   
    public List<Drawable> Drawables { get; set; }

    [JsonIgnore] public override RectangleF BoundingRectangle => new RectangleF(0, 0, Size.Width, Size.Height);

    public MultiDrawable(IServiceProvider services): base(services)
    {
        Drawables = new List<Drawable>();
    }

    public override void DrawTo(RenderSurface target, Vector2 drawTo, float rotation = 0.0f)
    {
        foreach(var drawable in Drawables)
        {
            drawable.DrawTo(target, drawTo);
        }
    }

    public override void Initialize()
    {
    }

    public override void Reset()
    {
    }

    public override void Dispose()
    {
    }
}