using System;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Legends.Engine;

public class Spatial<TParent>: Spatial
    where TParent: Spatial
{
    [JsonIgnore] public TParent Parent => (TParent)_parent;

    public Spatial() : this(null)
    {     

    }

    public Spatial(Spatial parent): base(parent)
    {

    }
}

public abstract class Spatial : IMovable, IRotatable, IScalable, ISizable, IRectangularF
{
    static Vector2 default_origin_normalized = new (.5f, .5f);

    protected Spatial _parent;
    private float     _rotation;        
    private Vector2   _position;        
    private Vector2   _scale = Vector2.One;
    private Size2     _size;
    private Matrix    _localMatrix;
    private Matrix    _invLocalMatrix;
    private Vector2?  _originFixed;
    private Vector2?  _originNormalized;

    public Vector2 Position { get => _position;                set => SetPosition(value); }
    public Vector2 Scale    { get => _scale;                   set => SetScale(value); }
    public float   Rotation { get => _rotation;                set => SetRotation(value); }   
    public Size2   Size     { get => _size * Scale;            set => SetSize(value); }
    public Vector2? OriginFixed     { get => _originFixed;          set => SetOriginFixed(value); }
    public Vector2? OriginRelative  { get => _originNormalized;     set => SetOriginNormalized(value); }

    [JsonIgnore] public Vector2 Origin { get => OriginFixed ?? (OriginRelative ?? default_origin_normalized) * Size; }
    [JsonIgnore] public RectangleF BoundingRectangle        => new(TopLeft, Size);
    [JsonIgnore] public Vector2 TopLeft                     { get => Position - Origin; }
    [JsonIgnore] public Vector2 BottomRight                 { get => TopLeft  + (Vector2)Size; }
    [JsonIgnore] public Vector2 Center                      { get => Position + (Vector2)Size / 2; }
    [JsonIgnore] public Vector2 AbsolutePosition            { get => Position   + _parent?.AbsolutePosition ?? Vector2.Zero; }
    [JsonIgnore] public Vector2 AbsoluteScale               { get => Scale      * _parent?.AbsoluteScale    ?? Vector2.One; }
    [JsonIgnore] public float   AbsoluteRotation            { get => Rotation   + _parent?.AbsoluteRotation ?? 0.0f; }
    [JsonIgnore] public Size2   AbsoluteSize                { get => Size       * AbsoluteScale; }
    [JsonIgnore] public Vector2 AbsoluteOrigin              { get => OriginFixed ?? (OriginRelative ?? default_origin_normalized) * AbsoluteSize; }
    [JsonIgnore] public Matrix  AbsoluteMatrix              { get => _parent?.AbsoluteMatrix ?? Matrix2.Identity * LocalMatrix; }
    [JsonIgnore] public Vector2 AbsoluteTopLeft             { get => AbsolutePosition - AbsoluteOrigin; }
    [JsonIgnore] public Vector2 AbsoluteBottomRight         { get => AbsoluteTopLeft + (Vector2)AbsoluteSize; }
    [JsonIgnore] public Vector2 AbsoluteCenter              { get => AbsolutePosition + (Vector2)AbsoluteSize / 2; }
    [JsonIgnore] public RectangleF AbsoluteBoundingRectangle=> new(AbsoluteTopLeft, AbsoluteSize);
    [JsonIgnore] public Matrix  LocalMatrix                 => GetLocalMatrix();
    [JsonIgnore] public bool IsDirty                        { get; protected set; }
    
    public Spatial() : this(null)
    {     

    }

    public Spatial(Spatial parent)
    {
        _parent = parent;
        IsDirty = true;
    }

    public void SetParent(Spatial parent)
    {
        if(_parent != null)
        {
            _parent.DetatchChild(this);
        }
        _parent = parent;
    }

    protected virtual void DetatchChild(Spatial child)
    {

    }
    
    public void Move(float x, float y) => Move(new Vector2(x, y));
    public void Move(Vector2 delta) => Position += Vector2.Transform(delta, Matrix.CreateRotationZ(0f - Rotation));
    public void RotateByDegrees(float degrees) => Rotate(MathF.PI/180 * degrees);
    public void Zoom(float value) => Zoom(new Vector2(value, value));
    public void Zoom(Vector2 zoom) => Scale *= zoom;
    public void Rotate(float deltaRadians) => Rotation += deltaRadians;
    public void SetPosition(float x, float y) => SetPosition(new Vector2(x, y));
    public void SetScale(float scale) => SetScale(new Vector2(scale, scale));
    public void SetSize(float width, float height) =>  SetSize(new Size2(width, height));
    public void SetOriginFixed(Vector2? origin) { _originFixed = origin; IsDirty = true; }
    public void SetOriginNormalized(Vector2? origin) { _originNormalized = origin; IsDirty = true; }
    
    public virtual void SetPosition(Vector2 position)
    {
        _position = position;
        IsDirty = true;
    }
    
    public virtual void SetScale(Vector2 scale)
    {
        _scale = scale;
        IsDirty = true;
    }
    
    public virtual void SetRotation(float radians)
    {
        _rotation = radians;
        IsDirty = true;
    }
    public virtual void SetSize(Size2 size)
    {
        _size = size / Scale;
        IsDirty = true;
    }

    protected virtual void UpdateMatricies()
    {
        if (IsDirty) {
            _localMatrix = 
                    Matrix2.CreateTranslation(-(Position + Origin / Scale))
                *   Matrix2.CreateRotationZ(Rotation)
                *   Matrix2.CreateTranslation(Origin / Scale)
                *   Matrix2.CreateScale(Scale);

            _invLocalMatrix = Matrix.Invert(_localMatrix);

            IsDirty = false;
        }
    }

    protected virtual Matrix GetLocalMatrix()
    {
        UpdateMatricies();
        return _localMatrix;
    }

    public bool Contains(Point2 point)
    {
        // rotate around rectangle center by -rectAngle
        if(Rotation * Rotation > float.Epsilon) {
            var sin = MathF.Sin(-_rotation);
            var cos = MathF.Cos(-_rotation);

            // set origin to rect center
            point -= Position;
            // rotate
            point  = new Point2(point.X * cos - point.Y * sin, point.X * sin + point.Y * cos);
            // put origin back
            point += Position;
        }

        var rect = new RectangleF(Position - ((Vector2)Origin) * Scale, (Vector2)Size);

        return  point.X     >= rect.Left
                && point.X  <= rect.Right 
                && point.Y  >= rect.Top 
                && point.Y  <= rect.Bottom;
    }

    public virtual void WorldToLocal(ref Vector2 point)
    {
        Vector2.Transform(ref point, ref _invLocalMatrix, out point);
    }

    public virtual void LocalToWorld(ref Vector2 point)
    {
        Vector2.Transform(ref point, ref _localMatrix, out point);
    }
}

public static class SpatialExtensions
{
    public static Vector2 WorldToLocal(this Spatial spatial, Vector2 point)
    {
        spatial.WorldToLocal(ref point);
        return point;
    }

    public static Vector2 LocalToWorld(this Spatial spatial, Vector2 point)
    {
        spatial.LocalToWorld(ref point);
        return point;
    }
    public static RectangleF WorldToLocal(this Spatial spatial, RectangleF rectangle)
    {
        Vector2 topLeft = rectangle.TopLeft;
        Vector2 bottomRight = rectangle.BottomRight;

        spatial.WorldToLocal(ref topLeft);
        spatial.WorldToLocal(ref bottomRight);

        return RectangleF.CreateFrom(topLeft, bottomRight);
    }

    public static RectangleF LocalToWorld(this Spatial spatial, RectangleF rectangle)
    {
        Vector2 topLeft     = rectangle.TopLeft;
        Vector2 bottomRight = rectangle.BottomRight;

        spatial.LocalToWorld(ref topLeft);
        spatial.LocalToWorld(ref bottomRight);

        return RectangleF.CreateFrom(topLeft, bottomRight);
    }
}