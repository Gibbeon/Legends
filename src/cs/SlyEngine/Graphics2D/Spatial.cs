using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace SlyEngine.Graphics2D;

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

    protected float     _rotation;        
    protected Vector2   _position;        
    protected Vector2   _scale;
    protected Size2     _size;
    protected Matrix    _localMatrix;
    protected Vector2   _originNormalized;
    public Vector2 Position     { get => _position; set => SetPosition(value); }
    public Vector2 Scale        { get => _scale; set => SetScale(value); }
    public float   Rotation     { get => _rotation; set => SetRotation(value); }   
    public Size2   Size         { get => _size * _scale; set => SetSize(value); }
    public Vector2 Origin       { get => _originNormalized * Size; set => _originNormalized = Size != Size2.Empty ? value / Size : Vector2.Zero; }
    public Vector2 Center       { get => _position + Origin; }
    public Vector2 OriginNormalized { get => _originNormalized; set => _originNormalized = value; }
    public RectangleF BoundingRectangle  { get => new RectangleF(Position, Size); }

    public Matrix LocalMatrix
    {
        get => GetLocalMatrix();
        private set => _localMatrix = value;
    }

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
        Position = data.Position;
        Rotation = data.Rotation;
        Scale    = data.Scale;
        Origin   = data.Origin;
        Size     = data.Size;

        UpdateLocalMatrix();
    }
    public void Move(float x, float y)
    {
        Move(new Vector2(x, y));
    }     
    public void Move(Vector2 direction)
    {
        Position += direction;//Vector2.Transform(direction, Matrix.CreateRotationZ(0f - Rotation));
    }
    public void Rotate(float deltaRadians)
    {
        Rotation += deltaRadians;
    }
    public void Zoom(float value)
    {
        Zoom(new Vector2(value, value));
    }

    public void Zoom(Vector2 zoom)
    {
        Scale += zoom;
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

    public virtual void SetSize(Size2 size)
    {
        var origin = OriginNormalized;
        _size = size;
        OriginNormalized = origin;
    }
    internal virtual void UpdateLocalMatrix()
    {
        if (HasChanged)
        {
            _localMatrix =Matrix.CreateTranslation(new Vector3(-Position, 0f))
                * Matrix.CreateTranslation(new Vector3(-(Origin / Scale), 0f))
                * Matrix.CreateRotationZ(Rotation)
                * Matrix.CreateScale(new Vector3(Scale, 1f))
                * Matrix.CreateTranslation(new Vector3((Origin / Scale), 0f));

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
        Matrix inverse = Matrix.Invert(_localMatrix);
        Vector2.Transform(ref point, ref inverse, out point);
        return point;
    }

    public void TransformWorldToLocal(ref Vector2 worldPoint, out Vector2 localPoint)
    {
        Matrix inverse = Matrix.Invert(_localMatrix);
        Vector2.Transform(ref worldPoint, ref inverse, out localPoint);
    }

    public void NeedToUpdate()
    {
        HasChanged = true;
    }

    protected virtual Matrix GetLocalMatrix()
    {
        UpdateLocalMatrix();
        return _localMatrix;
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