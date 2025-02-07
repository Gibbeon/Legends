using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.ComponentModel;
using System;
using MonoGame.Extended;

namespace Legends.Engine.Graphics2D.Components;

public abstract class Shape : Component, ISpriteRenderable
{
    [JsonIgnore]
    public int RenderLayerID => 1;

    public Color Color { get; set; }

    public int Thickness { get; set; } = 1;

    public RenderState RenderState { get; set; }

    public SpriteEffects SpriteEffect  { get; set; }

    [JsonIgnore]
    public bool Visible   => Parent.Visible;

    [JsonIgnore]
    public Vector2 Position { get => Parent.Position; set => Parent.Position = value; }
    
    [JsonIgnore]
    public IViewState ViewState => Parent.Scene.Camera;

    [JsonIgnore]
    public BoundingRectangle BoundingRectangle => BoundingRectangle.Empty;

    public Shape(): this (null, null)
    {

    }
    public Shape(IServiceProvider services, SceneObject parent) : base(services, parent)
    {
        Color = Color.White;
    }

    public override void Draw(GameTime gameTime)
    {
        if(Visible)
        {
            Services.Get<IRenderService>().DrawItem(this);
        }
    }

     public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public override void Initialize()
    {

    }

    public override void Reset()
    {
        
    }

    public abstract void DrawImmediate(GameTime gameTime, RenderSurface target);
}

public class Point : Shape
{
    public Point(IServiceProvider services, SceneObject parent) : base(services, parent) { }

    public override void DrawImmediate(GameTime gameTime, RenderSurface target)
    {
        var spriteBatch = target.SpriteBatch;
        spriteBatch.DrawPoint(Position, this.Color, Thickness);
    }
}

public class RectangleShape : Shape
{
    public RectangleShape(IServiceProvider services, SceneObject parent) : base(services, parent) { }

    public override void DrawImmediate(GameTime gameTime, RenderSurface target)
    {
        var spriteBatch = target.SpriteBatch;
        spriteBatch.DrawRectangle(BoundingRectangle, this.Color, Thickness);
    }

    public static explicit operator RectangleShape(RectangleF v)
    {
        throw new NotImplementedException();
    }
}

public class PolygonShape : Shape
{
    public Vector2[] Vertices { get; set; }
    
    public PolygonShape(IServiceProvider services, SceneObject parent) : base(services, parent) { }

    public override void DrawImmediate(GameTime gameTime, RenderSurface target)
    {
        var spriteBatch = target.SpriteBatch;
        spriteBatch.DrawPolygon(this.Position, Vertices, this.Color, Thickness);
    }
}

public class CircleShape : Shape
{
    public float Radius { get; set; } = 1.0f;
    public int Sides { get; set; }  = 1;
    
    public CircleShape(IServiceProvider services, SceneObject parent) : base(services, parent) { }

    public override void DrawImmediate(GameTime gameTime, RenderSurface target)
    {
        var spriteBatch = target.SpriteBatch;
        spriteBatch.DrawCircle(this.Position, Radius, Sides, this.Color, Thickness);
    }
}

public class LineShape : Shape
{
    public float Length { get; set; } = 1.0f;
    public float Angle { get; set; }
    
    public LineShape(IServiceProvider services, SceneObject parent) : base(services, parent) { }
    public override void DrawImmediate(GameTime gameTime, RenderSurface target)
    {
        var spriteBatch = target.SpriteBatch;
        spriteBatch.DrawLine(this.Position, Length, Angle, this.Color, Thickness);
    }
}
