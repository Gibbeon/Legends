﻿using Microsoft.Xna.Framework;
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
    public Vector2 Position => Parent.Position;
    
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

public class Rectangle : Shape
{
    public Rectangle(IServiceProvider services, SceneObject parent) : base(services, parent) { }

    public override void DrawImmediate(GameTime gameTime, RenderSurface target)
    {
        var spriteBatch = target.SpriteBatch;
        spriteBatch.DrawRectangle(BoundingRectangle, this.Color, Thickness);
    }

    public static explicit operator Rectangle(RectangleF v)
    {
        throw new NotImplementedException();
    }
}

public class Polygon : Shape
{
    public Vector2[] Vertices { get; set; }
    
    public Polygon(IServiceProvider services, SceneObject parent) : base(services, parent) { }

    public override void DrawImmediate(GameTime gameTime, RenderSurface target)
    {
        var spriteBatch = target.SpriteBatch;
        spriteBatch.DrawPolygon(this.Position, Vertices, this.Color, Thickness);
    }
}

public class Circle : Shape
{
    public float Radius { get; set; } = 1.0f;
    public int Sides { get; set; }  = 1;
    
    public Circle(IServiceProvider services, SceneObject parent) : base(services, parent) { }

    public override void DrawImmediate(GameTime gameTime, RenderSurface target)
    {
        var spriteBatch = target.SpriteBatch;
        spriteBatch.DrawCircle(this.Position, Radius, Sides, this.Color, Thickness);
    }
}

public class Line : Shape
{
    public float Length { get; set; } = 1.0f;
    public float Angle { get; set; }
    
    public Line(IServiceProvider services, SceneObject parent) : base(services, parent) { }
    public override void DrawImmediate(GameTime gameTime, RenderSurface target)
    {
        var spriteBatch = target.SpriteBatch;
        spriteBatch.DrawLine(this.Position, Length, Angle, this.Color, Thickness);
    }
}
