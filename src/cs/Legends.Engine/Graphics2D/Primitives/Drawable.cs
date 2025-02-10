using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;

namespace Legends.Engine.Graphics2D.Primitives;

public abstract class Drawable : Asset, IBounds
{   
    private Color                       _color = Color.White;
    public Color                        Color { get => _color; set => _color = value; }
    public RenderState                  RenderState { get; set; }
    public SizeF                        Size { get; set; }
    public Vector2                      Origin { get; set; }

    public abstract RectangleF          BoundingRectangle { get; }

    public Drawable(IServiceProvider services, string assetName): base(services, assetName)
    {

    }

    public abstract void DrawTo(RenderSurface target, Vector2 drawTo, float rotation = 0.0f);

    public bool Contains(Vector2 point)
    {
        throw new NotImplementedException();
    }
}