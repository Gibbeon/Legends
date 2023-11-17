using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Legends.App.Graphics
{
    public class Spatial2D
    {
        public Vector2 Origin       { get; set; }
        public Vector2 Position     { get => _position; set => SetPosition(value); }
        public Vector2 Scale        { get => _scale; set => SetScale(value); }
        public float Rotation       { get => _rotation; set => SetRotation(value); }        
        public Size2 OrigionalSize  { get; set; }
        //public Vector2 Center => Position + Origin;
        public Matrix LocalWorldMatrix
        {
            get => GetLocalMatrix();
            private set => _globalMatrix = value;
        }
        public Matrix GlobalWorldMatrix
        {
            get => GetGlobalMatrix();
            private set => _localMatrix = value;
        }

        protected float _rotation;        
        protected Vector2 _position;        
        protected Vector2 _scale;
        protected Matrix _localMatrix;
        protected Matrix _globalMatrix;

        public bool HasChanged
        {
            get;
            protected set;
        }
        public Size2 Size
        {
            get { return new Size2(OrigionalSize.Width * Scale.X, OrigionalSize.Height * Scale.Y); }
        }
        public Vector2 OriginNormalized
        {
            get
            {
                return new Vector2(Origin.X / (float)Size.Width, Origin.Y / (float)Size.Height);
            }
            set
            {
                Origin = new Vector2(value.X * (float)Size.Width, value.Y * (float)Size.Height);
            }
        }

        public Spatial2D()
        {
            Scale = Vector2.One;
            _localMatrix = Matrix.Identity;
        }
        public Spatial2D(Size2 size) : this()
        {            
            OrigionalSize = size;
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
        public virtual void SetRotation(float deltaRadians)
        {
            _rotation = deltaRadians;
            NeedToUpdate();
        }
        public virtual void Update(GameTime gameTime)
        {
            UpdateSpatial();
        }
        internal virtual void UpdateSpatial()
        {
            UpdateRelative(null); // nothing to do, there is no parent matrix
        }

        internal virtual void UpdateRelative(Spatial2D? parent, bool updateGlobal = false)
        {
            if (HasChanged)
            {
                _localMatrix =Matrix.CreateTranslation(new Vector3(-Position, 0f))
                    * Matrix.CreateTranslation(new Vector3(-Origin, 0f))
                    * Matrix.CreateRotationZ(Rotation)
                    * Matrix.CreateScale(new Vector3(Scale, 1f))
                    * Matrix.CreateTranslation(new Vector3(Origin, 0f));

                updateGlobal = true;
                HasChanged = false;
            }

            if (updateGlobal)
            {
                _globalMatrix = parent != null ?
                    Matrix.Multiply(parent.GlobalWorldMatrix, _localMatrix) :
                    _localMatrix;
            }
        }

        public Vector2 TransformLocalToWorld(Vector2 point)
        {
            Vector2.Transform(ref point, ref _globalMatrix, out point);
            return point;
        }

        public void TransformLocalToWorld(ref Vector2 localPoint, out Vector2 worldPoint)
        {
            Vector2.Transform(ref localPoint, ref _globalMatrix, out worldPoint);
        }

        public Vector2 TransformWorldToLocal(Vector2 point)
        {
            Matrix inverse = Matrix.Invert(_globalMatrix);
            Vector2.Transform(ref point, ref inverse, out point);
            return point;
        }

        public void TransformWorldToLocal(ref Vector2 worldPoint, out Vector2 localPoint)
        {
            Matrix inverse = Matrix.Invert(_globalMatrix);
            Vector2.Transform(ref worldPoint, ref inverse, out localPoint);
        }

        public void NeedToUpdate()
        {
            HasChanged = true;
        }

        protected virtual Matrix GetGlobalMatrix()
        {
            UpdateSpatial();
            return _globalMatrix;
        }

        protected virtual Matrix GetLocalMatrix()
        {
            UpdateSpatial();
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

            var rect = new RectangleF((Position - ((Vector2)OrigionalSize - Origin) * Scale), (Vector2)OrigionalSize * Scale);

            return  point.X     >= rect.Left
                    && point.X  <= rect.Right 
                    && point.Y  >= rect.Top 
                    && point.Y  <= rect.Bottom;
        }
    }
}    