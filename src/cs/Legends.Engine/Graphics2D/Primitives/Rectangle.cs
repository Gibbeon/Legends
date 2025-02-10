using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Legends.Engine.Graphics2D.Primitives;

public class Quad : Drawable
{    
    [DefaultValue(1)]
    public int                          Thickness   { get; set;}
    public bool                         Solid       { get; set;}

    
    [JsonIgnore] public override RectangleF  BoundingRectangle => new(Vector2.Zero - Origin, Size);

    public Quad(IServiceProvider services): base(services)
    {
        Thickness = 1;
    }

    public override void DrawTo(RenderSurface target, Vector2 drawTo, float rotation = 0.0f)
    {           
        if(Solid)
        {
            target.SpriteBatch.FillRectangle(new RectangleF(drawTo - Origin, Size), this.Color); 
        }
        else
        {    
            target.SpriteBatch.DrawRectangle(new RectangleF(drawTo - Origin, Size), this.Color, Thickness);
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