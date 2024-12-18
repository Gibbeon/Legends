using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.ComponentModel;
using System;
using MonoGame.Extended;

namespace Legends.Engine.Graphics2D.Components;

public abstract class Shape : Component2D, ISpriteRenderable
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
            Services.Get<IRenderService>().DrawBatched(this);
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

    public abstract void DrawImmediate(GameTime gameTime, GraphicsResource target = null);
}

public class Point : Shape
{
    public Point(IServiceProvider services, SceneObject parent) : base(services, parent) { }

    public override void DrawImmediate(GameTime gameTime, GraphicsResource target = null)
    {
        var spriteBatch = this.GetSpriteBatch(target);
        spriteBatch.DrawPoint(Position, this.Color, Thickness);
        if(target is not SpriteBatch)
            spriteBatch?.End();
    }
}

public class Rectangle : Shape
{
    public Rectangle(IServiceProvider services, SceneObject parent) : base(services, parent) { }

    public override void DrawImmediate(GameTime gameTime, GraphicsResource target = null)
    {
        var spriteBatch = this.GetSpriteBatch(target);
        spriteBatch.DrawRectangle(BoundingRectangle, this.Color, Thickness);
        if(target is not SpriteBatch)
            spriteBatch?.End();
    }
}

public class Polygon : Shape
{
    public Vector2[] Vertices { get; set; }
    
    public Polygon(IServiceProvider services, SceneObject parent) : base(services, parent) { }

    public override void DrawImmediate(GameTime gameTime, GraphicsResource target = null)
    {
        var spriteBatch = this.GetSpriteBatch(target);
        spriteBatch.DrawPolygon(this.Position, Vertices, this.Color, Thickness);
        if(target is not SpriteBatch)
            spriteBatch?.End();
    }
}

public class Circle : Shape
{
    public float Radius { get; set; } = 1.0f;
    public int Sides { get; set; }  = 1;
    
    public Circle(IServiceProvider services, SceneObject parent) : base(services, parent) { }

    public override void DrawImmediate(GameTime gameTime, GraphicsResource target = null)
    {
        var spriteBatch = this.GetSpriteBatch(target);
        spriteBatch.DrawCircle(this.Position, Radius, Sides, this.Color, Thickness);
        if(target is not SpriteBatch)
            spriteBatch?.End();
    }
}

public class Line : Shape
{
    public float Length { get; set; } = 1.0f;
    public float Angle { get; set; }
    
    public Line(IServiceProvider services, SceneObject parent) : base(services, parent) { }

    public override void DrawImmediate(GameTime gameTime, GraphicsResource target = null)
    {
        var spriteBatch = this.GetSpriteBatch(target);
        spriteBatch.DrawLine(this.Position, Length, Angle, this.Color, Thickness);
        if(target is not SpriteBatch)
            spriteBatch?.End();
    }
}
