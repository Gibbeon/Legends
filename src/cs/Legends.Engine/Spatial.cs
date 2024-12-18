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

public abstract class Spatial : IMovable, IRotatable, IScalable
{
    protected Spatial   _parent;
    protected float     _rotation;        
    protected Vector2   _position;        
    protected Vector2   _scale = Vector2.One;
    protected Matrix    _localMatrix;
    protected Matrix    _invLocalMatrix;

    public Vector2 Position { get => _position;             set => SetPosition(value); }
    public Vector2 Scale    { get => _scale;                set => SetScale(value); }
    public float   Rotation { get => _rotation;             set => SetRotation(value); }

    [JsonIgnore] public Matrix  LocalMatrix                 => GetLocalMatrix(); 
    [JsonIgnore] public Vector2 AbsolutePosition            { get => Position   + _parent?.AbsolutePosition ?? Vector2.Zero; }
    [JsonIgnore] public Vector2 AbsoluteScale               { get => Scale      * _parent?.AbsoluteScale    ?? Vector2.One; }
    [JsonIgnore] public float   AbsoluteRotation            { get => Rotation   + _parent?.AbsoluteRotation ?? 0.0f; }
    [JsonIgnore] public Matrix  GlobalMatrix                { get => (_parent?.GlobalMatrix ?? Matrix3x2.Identity) * LocalMatrix; }
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

    protected virtual void UpdateMatricies()
    {
        if (IsDirty) {
            _localMatrix = 
                    Matrix3x2.CreateTranslation(-Position)
                *   Matrix3x2.CreateRotationZ(Rotation)
                *   Matrix3x2.CreateScale(Scale);

            _invLocalMatrix = Matrix.Invert(_localMatrix);

            IsDirty = false;
        }
    }

    protected virtual Matrix GetLocalMatrix()
    {
        UpdateMatricies();
        return _localMatrix;
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