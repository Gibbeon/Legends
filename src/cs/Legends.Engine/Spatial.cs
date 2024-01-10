using System;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Legends.Engine;

public class Spatial : IMovable, IRotatable, IScalable, ISizable, IRectangularF
{
    private float     _rotation;        
    private Vector2   _position;        
    private Vector2   _scale = Vector2.One;
    private float     _offsetRotation;        
    private Vector2   _offsetPosition;        
    private Vector2   _offsetScale;
    private Size2     _size;
    private Matrix    _localMatrix;
    private Vector2   _originNormalized;

    public Vector2 Position             { get => (OffsetPosition + _position) * OffsetScale; set => SetPosition(value); }
    public Vector2 Scale                { get => OffsetScale    * _scale; set => SetScale(value); }
    public float   Rotation             { get => OffsetRotation + _rotation; set => SetRotation(value); }   
    public Size2   Size                 { get => _size * Scale; set => SetSize(value); }
    public Vector2 Origin               { get => _originNormalized * Size; set => SetOrigin(value); }

    [JsonIgnore]
    public Vector2 TopLeft              { get => Position - Origin; set => Position = value + Origin; }
    
    [JsonIgnore]
    public Vector2 BottomRight          { get => TopLeft + (Vector2)Size; set=> Size = value - TopLeft; }

    [JsonIgnore]
    internal float OffsetRotation       
    { 
        get => _offsetRotation; 
        set { _offsetRotation = value; IsDirty = true; }
    }      

    [JsonIgnore]
    internal Vector2 OffsetPosition     
    { 
        get => _offsetPosition; 
        set { _offsetPosition = value; IsDirty = true; }
    }

    [JsonIgnore]
    internal Vector2 OffsetScale        
    { 
        get => _offsetScale; 
        set { _offsetScale = value; IsDirty = true; }
    }     

    [JsonIgnore]
    public Vector2 Center               { get => Position + (Vector2)Size / 2; }
    [JsonIgnore]
    public Vector2 OriginNormalized     { get => _originNormalized; set => SetOriginNormalized(value); }
    [JsonIgnore]
    public RectangleF BoundingRectangle => GetBoundingRectangle();
    [JsonIgnore]
    public Matrix LocalMatrix           => GetLocalMatrix();
    [JsonIgnore]
    public bool IsDirty              { get; protected set; }
    
    public Spatial()
    {     
        _offsetScale = Vector2.One;
        IsDirty = true;
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
        IsDirty = true;
    }

    public void SetScale(float scale)
    {
        SetScale(new Vector2(scale, scale));
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

    public virtual void SetSize(float width, float height)
    {
        SetSize(new Size2(width, height));
    }

    public virtual void SetOriginNormalized(Vector2 origin)
    {
        _originNormalized = origin;
        IsDirty = true;
    }

    public virtual void SetOrigin(Vector2 origin)
    {
        SetOriginNormalized(Size != Size2.Empty ? origin / Size : Vector2.Zero);
    }

    public virtual void SetSize(Size2 size)
    {
        var origin = OriginNormalized;
        _size = size;
        OriginNormalized = origin;
        IsDirty = true;
    }

    protected virtual void OnChanged()
    {
        if (IsDirty)
        {
            _localMatrix = Matrix2.CreateTranslation(-Position)
                * Matrix2.CreateTranslation(-(Origin / Scale))
                * Matrix2.CreateRotationZ(Rotation)
                * Matrix2.CreateScale(Scale)
                * Matrix2.CreateTranslation(Origin); // for the camera I removed the scale; should make sure that' correct for all use cases

            IsDirty = false;
        }
    }

    /*public Vector2 TransformLocalToWorld(Vector2 point)
    {
        Vector2.Transform(ref point, ref _localMatrix, out point);
        return point;
    }

    public void TransformLocalToWorld(ref Vector2 localPoint, out Vector2 worldPoint)
    {
        Vector2.Transform(ref localPoint, ref _localMatrix, out worldPoint);
    }

    public virtual Vector2 TransformWorldToLocal(Vector2 point)
    {
        Matrix inverse = Matrix.Invert(LocalMatrix);
        Vector2.Transform(ref point, ref inverse, out point);
        return point;
    }*/

    protected virtual Matrix GetLocalMatrix()
    {
        OnChanged();
        return _localMatrix;
    }

    public virtual RectangleF GetBoundingRectangle()
    {
        return new RectangleF(Position - Origin, Size);
    }

    public bool Contains(Point2 point)
    {
        // rotate around rectangle center by -rectAngle
        if(Rotation * Rotation > float.Epsilon)
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

        var rect = new RectangleF(Position - ((Vector2)Origin) * Scale, (Vector2)Size);

        return  point.X     >= rect.Left
                && point.X  <= rect.Right 
                && point.Y  >= rect.Top 
                && point.Y  <= rect.Bottom;
    }
}