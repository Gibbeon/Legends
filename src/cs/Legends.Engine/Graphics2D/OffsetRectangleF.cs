using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Legends.Engine;

public struct OffsetRectangleF : IBounds, ISizable, IRectangularF
{
    private SizeF       _size;
    private Vector2     _origin;

    public SizeF    Size    { get => _size;     set => Resize(value); }
    public Vector2  Origin  { get => _origin;   set => SetOrigin(value); }

    [JsonIgnore] public float Width => Size.Width;
    [JsonIgnore] public float Height => Size.Height;
    [JsonIgnore] public RectangleF BoundingRectangle        => new(Vector2.Zero - Origin, Size);
    [JsonIgnore] public Vector2 TopLeft                     => BoundingRectangle.TopLeft;
    [JsonIgnore] public Vector2 BottomRight                 => BoundingRectangle.BottomRight;   
    [JsonIgnore] public Vector2 TopRight                    => BoundingRectangle.TopRight;
    [JsonIgnore] public Vector2 BottomLeft                  => BoundingRectangle.BottomLeft;
    [JsonIgnore] public Vector2 Center                      => BoundingRectangle.Center;
    [JsonIgnore] public Vector2 HalfExtents                 => Size / 2;

    public static implicit operator RectangleF(OffsetRectangleF rect)   => rect.BoundingRectangle;
    public static implicit operator Rectangle(OffsetRectangleF rect)    => (Rectangle)rect.BoundingRectangle;
    public static implicit operator OffsetRectangleF(RectangleF rect)   => new OffsetRectangleF(rect);
    public static implicit operator OffsetRectangleF(Rectangle rect)    => new OffsetRectangleF(rect);

    public OffsetRectangleF() {}    
    public OffsetRectangleF(RectangleF rect): this(rect.Left, rect.Top, rect.Width, rect.Height)
    {     
        
    } 

    public OffsetRectangleF(float left, float top, float width, float height)
    {
        Origin = new Vector2(-left, -top);
        Size = new SizeF(width, height);
    }
   
    public void Resize(float width, float height) =>    Resize(new SizeF(width, height));    
    public void SetOrigin(float width, float height) =>  SetOrigin(new Vector2(width, height));
    public void Resize(SizeF size)
    {
        if(!Size.IsEmpty) 
        {
            SetOrigin(Origin.X / Width * size.Width, Origin.Y / Height * size.Height);
        }
        
        _size = size;
    }

    public void SetOrigin(Vector2 origin)         
    { 
        _origin = origin;
    }   

    public bool Contains(Vector2 point) => BoundingRectangle.Contains(point);
    
/*    public Matrix GetSpatialMatrix(Spatial parent) // rotate around the center point not the position
    {
        return  Matrix3x2.CreateTranslation(-(Position + Origin / parent.Scale))
            *   Matrix3x2.CreateRotationZ(parent.Rotation)
            *   Matrix3x2.CreateTranslation(Origin / parent.Scale)
            *   Matrix3x2.CreateScale(parent.Scale);
    }

    public Region2D GetSubRegion(int index, SizeF frameSize)
    {
        var stride = (int)Size.Width / (int)frameSize.Width;
        return new Region2D(new Vector2(Position.X + index % stride * frameSize.Width, Position.Y + index / stride * frameSize.Height), frameSize);
    }


    public bool Contains(Vector2 point, Spatial parent)
    {
        // rotate around rectangle center by -rectAngle
        if(parent.Rotation * parent.Rotation > float.Epsilon) {
            var sin = MathF.Sin(-parent.Rotation);
            var cos = MathF.Cos(-parent.Rotation);

            // set origin to rect center
            point -= parent.Position;
            // rotate
            point  = new Vector2(point.X * cos - point.Y * sin, point.X * sin + point.Y * cos);
            // put origin back
            point += parent.Position;
        }

        return new RectangleF(parent.Position - Origin * parent.Scale, (Vector2)Size * parent.Scale).Contains(point);
    }
*/
}