using System;
using Microsoft.Xna.Framework;

namespace LitEngine.Framework.Graphics
{
    public class Spatial : IUpdateable
    {
        public event EventHandler<EventArgs>? EnabledChanged;
        public event EventHandler<EventArgs>? UpdateOrderChanged;

        public bool Enabled
        {
            get => _enabled;
            set => SetEnabled(value);
        }
        public int UpdateOrder
        {
            get => _updateOrder;
            set => SetUpdateOrder(value);
        }

        public Vector3 Position
        {
            get;
            private set;
        }
        public Vector3 Scale
        {
            get;
            private set;
        }
        public Quaternion Rotation
        {
            get;
            private set;
        }
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
        public bool HasChanged
        {
            get;
            protected set;
        }
        protected bool _enabled;
        protected int _updateOrder;
        protected Matrix _localMatrix;
        protected Matrix _globalMatrix;

        public Spatial()
        {
            Scale = Vector3.One;
            Position = Vector3.Zero;
            Rotation = Quaternion.Identity;
            Enabled = true;
            _localMatrix = Matrix.Identity;
            _globalMatrix = Matrix.Identity;
        }

        public virtual void SetPosition(Vector3 position)
        {
            Position = position;
            NeedToUpdate();
        }

        public virtual void SetScale(Vector3 scale)
        {
            Scale = scale;
            NeedToUpdate();
        }

        public virtual void SetRotation(Quaternion quaternion)
        {
            Rotation = quaternion;
            NeedToUpdate();
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!Enabled) return;

            UpdateSpatial();
        }

        internal virtual void UpdateSpatial()
        {
            UpdateRelative(null); // nothing to do, there is no parent matrix
        }

        internal virtual void UpdateRelative(Spatial? parent, bool updateGlobal = false)
        {
            if (HasChanged)
            {
                _localMatrix =
                    Matrix.CreateScale(Scale)
                    * Matrix.CreateFromQuaternion(Rotation)
                    * Matrix.CreateTranslation(Position);
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

        public Vector3 TransformLocalToWorld(Vector3 point)
        {
            Vector3.Transform(ref point, ref _globalMatrix, out point);
            return point;
        }

        public void TransformLocalToWorld(ref Vector3 localPoint, out Vector3 worldPoint)
        {
            Vector3.Transform(ref localPoint, ref _globalMatrix, out worldPoint);
        }

        public Vector3 TransformWorldToLocal(Vector3 point)
        {
            Matrix inverse = Matrix.Invert(_globalMatrix);
            Vector3.Transform(ref point, ref inverse, out point);
            return point;
        }

        public void TransformWorldToLocal(ref Vector3 worldPoint, out Vector3 localPoint)
        {
            Matrix inverse = Matrix.Invert(_globalMatrix);
            Vector3.Transform(ref worldPoint, ref inverse, out localPoint);
        }

        protected void NeedToUpdate()
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

        public void SetEnabled(bool value)
        {
            if (_enabled != value)
            {
                _enabled = value;
                if (EnabledChanged != null)
                {
                    EnabledChanged(this, EventArgs.Empty);
                }
            }
        }
        public void SetUpdateOrder(int value)
        {
            if (_updateOrder != value)
            {
                _updateOrder = value;
                if (UpdateOrderChanged != null)
                {
                    UpdateOrderChanged(this, EventArgs.Empty);
                }
            }
        }
    }
}

