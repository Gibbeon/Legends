using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LitEngine.Framework.Graphics
{
    public class Camera : Spatial
    {
        public Vector3 Up
        {
            get => LocalWorldMatrix.Up;
        }
        public Matrix View
        {
            get => GetViewMatrix();
        }
        public Matrix Projection
        {
            get => GetProjectionMatrix();
        }
        public Matrix ModelViewProjection
        {
            get => GetModelViewProjectionMatrix();
        }
        public BoundingFrustum Frustum
        {
            get;
            protected set;
        }
        private Matrix _modelViewProjectionMatrix;
        private Matrix _viewMatrix;
        private Matrix _projectionMatrix;
        public Camera(Viewport viewport)
        {
            Frustum = new BoundingFrustum(Matrix.Identity);

            SetOrthogonal(viewport);
            LookAt(Vector3.Forward);
        }
        public void SetPerspective(Viewport viewport, float fieldOfView)
        {
            _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(fieldOfView, viewport.AspectRatio, viewport.MinDepth, viewport.MaxDepth);
            NeedToUpdate();
        }
        public void SetOrthogonal(Viewport viewport)
        {
            _projectionMatrix = Matrix.CreateOrthographicOffCenter(viewport.X, viewport.X + viewport.Width, viewport.Y + viewport.Height, viewport.Y, viewport.MinDepth, viewport.MaxDepth);
            NeedToUpdate();
        }
        public void LookAt(Vector3 target)
        {
            _viewMatrix = Matrix.CreateLookAt(Position, target + Position, Vector3.Up);
            NeedToUpdate();
        }
        public override void Update(GameTime gameTime)
        {
            if(!Enabled) return;
            
            UpdateSpatial();
        }

        internal override void UpdateSpatial()
        {
            if (HasChanged)
            {
                base.UpdateSpatial();

                _modelViewProjectionMatrix = GlobalWorldMatrix * _viewMatrix * _projectionMatrix;
                Frustum.Matrix = _modelViewProjectionMatrix;
            }
        }
        protected virtual Matrix GetViewMatrix()
        {
            UpdateSpatial();
            return _viewMatrix;
        }
        protected virtual Matrix GetProjectionMatrix()
        {
            UpdateSpatial();
            return _projectionMatrix;
        }
        protected virtual Matrix GetModelViewProjectionMatrix()
        {
            UpdateSpatial();
            return _modelViewProjectionMatrix;
        }
    }
}
