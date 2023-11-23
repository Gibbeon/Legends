using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Legends.Engine.Graphics2D;

public class Spatial : IMovable, IRotatable, IScalable, ISizable, IRectangularF
{
    public class SpatialDesc
    {
        public float Rotation;        
        public Vector2 Position;      
        public Vector2 Scale = Vector2.One;
        public Vector2 Origin;
        public Size2 Size;
    }

    private float     _rotation;        
    private Vector2   _position;        
    private Vector2   _scale;

    private float     _offsetRotation;        
    private Vector2   _offsetPosition;        
    private Vector2   _offsetScale;

    private Size2     _size;
    private Matrix    _localMatrix;
    private Vector2   _originNormalized;

    internal float     OffsetRotation   { get => _offsetRotation; set => _offsetRotation = value;}     
    internal Vector2   OffsetPosition   { get => _offsetPosition; set => _offsetPosition = value;}             
    internal Vector2   OffsetScale      { get => _offsetScale; set => _offsetScale = value; }     

    public Vector2 Position             { get => OffsetPosition + _position; set => SetPosition(value); }
    public Vector2 Scale                { get => OffsetScale * _scale; set => SetScale(value); }
    public float   Rotation             { get => OffsetRotation + _rotation; set => SetRotation(value); }   
    public Size2   Size                 { get => _size * Scale; set => SetSize(value); }
    public Vector2 Origin               { get => _originNormalized * Size; set => _originNormalized = Size != Size2.Empty ? value / Size : Vector2.Zero; }
    public Vector2 Center               { get => Position + (Vector2)Size / 2; }
    public Vector2 OriginNormalized     { get => _originNormalized; set => _originNormalized = value; }
    public RectangleF BoundingRectangle { get => GetBoundingRectangle(); }
    public Matrix LocalMatrix           => GetLocalMatrix();

    public bool HasChanged
    {
        get;
        protected set;
    }

    public Spatial() : this(new SpatialDesc())
    {
        
    }
    public Spatial(SpatialDesc data)
    {            
        _offsetScale = Vector2.One;

        Position = data.Position;
        Rotation = data.Rotation;
        Scale    = data.Scale;
        Origin   = data.Origin;
        Size     = data.Size;

        UpdateMatrix();
    }
    public void Move(float x, float y)
    {
        Move(new Vector2(x, y));
    }     
    public virtual void Move(Vector2 direction)
    {
        Position += direction;//Vector2.Transform(direction, Matrix.CreateRotationZ(0f - Rotation));
    }
    public virtual void Rotate(float deltaRadians)
    {
        Rotation += deltaRadians;
    }
    public void Zoom(float value)
    {
        Zoom(new Vector2(value, value));
    }

    public void Zoom(Vector2 zoom)
    {
        Scale *= zoom;
    }

    public virtual void SetPosition(Vector2 position)
    {
        _position = position;
        NeedToUpdate();
    }
    public void SetScale(float scale)
    {
        SetScale(new Vector2(scale, scale));
    }
    public virtual void SetScale(Vector2 scale)
    {
        _scale = scale;
        NeedToUpdate();
    }
    public virtual void SetRotation(float radians)
    {
        _rotation = radians;
        NeedToUpdate();
    }

    public virtual void SetSize(float width, float height)
    {
        SetSize(new Size2(width, height));
    }

    public virtual void SetSize(Size2 size)
    {
        var origin = OriginNormalized;
        _size = size;
        OriginNormalized = origin;
    }
    internal virtual void UpdateMatrix()
    {
        if (HasChanged)
        {
            _localMatrix = Matrix2.CreateTranslation(-Position)
                * Matrix2.CreateTranslation(-(Origin / Scale))
                * Matrix2.CreateRotationZ(Rotation)
                * Matrix2.CreateScale(Scale)
                * Matrix2.CreateTranslation((Origin / Scale));

            HasChanged = false;
        }
    }

    public Vector2 TransformLocalToWorld(Vector2 point)
    {
        Vector2.Transform(ref point, ref _localMatrix, out point);
        return point;
    }

    public void TransformLocalToWorld(ref Vector2 localPoint, out Vector2 worldPoint)
    {
        Vector2.Transform(ref localPoint, ref _localMatrix, out worldPoint);
    }

    public Vector2 TransformWorldToLocal(Vector2 point)
    {
        Matrix inverse = Matrix.Invert(LocalMatrix);
        Vector2.Transform(ref point, ref inverse, out point);
        return point;
    }

    public void TransformWorldToLocal(ref Vector2 worldPoint, out Vector2 localPoint)
    {
        Matrix inverse = Matrix.Invert(LocalMatrix);
        Vector2.Transform(ref worldPoint, ref inverse, out localPoint);
    }

    public void NeedToUpdate()
    {
        HasChanged = true;
    }

    protected virtual Matrix GetLocalMatrix()
    {
        UpdateMatrix();
        return _localMatrix;
    }

    public virtual RectangleF GetBoundingRectangle()
    {
        return new RectangleF(Position - Origin, Size);
    }

    public bool Contains(Point2 point)
    {
        // rotate around rectangle center by -rectAngle
        if(_rotation * _rotation > float.Epsilon)
        {
            var sin = MathF.Sin(-_rotation);
            var cos = MathF.Cos(-_rotation);

            // set origin to rect center
            point -= Position;
            // rotate
            point  = new Point2(point.X * cos - point.Y * sin, point.X * sin + point.Y * cos);
            // put origin back
            point += Position;
        }

        var rect = new RectangleF((Position - ((Vector2)Origin) * Scale), (Vector2)Size);

        return  point.X     >= rect.Left
                && point.X  <= rect.Right 
                && point.Y  >= rect.Top 
                && point.Y  <= rect.Bottom;
    }
}