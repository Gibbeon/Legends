using System;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Legends.Engine;

public interface IBounds
{
    bool Contains(Vector2 point);
}

public class Box2D : IBounds, ISizable, IRectangularF
{
    private SizeF     _size;
    private Vector2?  _originFixed;
    private Vector2   _originRelative = new (.5f, .5f);

    public SizeF    Size             { get => _size;             set => SetSize(value); }
    public Vector2? OriginFixed      { get => _originFixed;      set => SetOriginFixed(value); }
    public Vector2  OriginRelative   { get => _originRelative; set => SetOriginRelative(value); }

    [JsonIgnore] public Vector2 Origin                      => OriginFixed.GetValueOrDefault(OriginRelative * Size);
    [JsonIgnore] public RectangleF BoundingRectangle        => new(Vector2.Zero - Origin, Size);
    [JsonIgnore] public Vector2 TopLeft                     => BoundingRectangle.TopLeft;
    [JsonIgnore] public Vector2 BottomRight                 => BoundingRectangle.BottomRight;   
    [JsonIgnore] public Vector2 TopRight                    => BoundingRectangle.TopRight;
    [JsonIgnore] public Vector2 BottomLeft                  => BoundingRectangle.BottomLeft;
    [JsonIgnore] public Vector2 Center                      => BoundingRectangle.Center;
    
    public Box2D()
    {     

    }


    public void SetSize(float width, float height) =>  SetSize(new Vector2(width, height));
    public virtual void SetSize(SizeF size)
    {
        _size = size;
    }
    public void SetOriginFixed(float width, float height) =>  SetOriginFixed(new Vector2(width, height));
    public void SetOriginFixed(Vector2? origin)         
    { 
        _originFixed = origin;
    }   
    public void SetOriginRelative(float widthFactor, float heighFactort) =>  SetOriginRelative(new Vector2(widthFactor, heighFactort));
    public void SetOriginRelative(Vector2 origin)     
    { 
        _originRelative = origin; 
    }
    
    public Matrix GetSpatialMatrix(Spatial spatial) // rotate around the center point not the position
    {
        return  Matrix3x2.CreateTranslation(-(spatial.Position + Origin / spatial.Scale))
            *   Matrix3x2.CreateRotationZ(spatial.Rotation)
            *   Matrix3x2.CreateTranslation(Origin / spatial.Scale)
            *   Matrix3x2.CreateScale(spatial.Scale);
    }

    public bool Contains(Vector2 point) => BoundingRectangle.Contains(point);

    public bool Contains(Vector2 point, Spatial spatial)
    {
        // rotate around rectangle center by -rectAngle
        if(spatial.Rotation * spatial.Rotation > float.Epsilon) {
            var sin = MathF.Sin(-spatial.Rotation);
            var cos = MathF.Cos(-spatial.Rotation);

            // set origin to rect center
            point -= spatial.Position;
            // rotate
            point  = new Vector2(point.X * cos - point.Y * sin, point.X * sin + point.Y * cos);
            // put origin back
            point += spatial.Position;
        }

        return new RectangleF(spatial.Position - Origin * spatial.Scale, (Vector2)Size * spatial.Scale).Contains(point);
    }
}