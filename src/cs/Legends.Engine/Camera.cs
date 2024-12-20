using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using Legends.Engine.Graphics2D;
using Legends.Engine.Resolvers;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ViewportAdapters;

namespace Legends.Engine;

public class Camera : SceneObject, IViewState
{    
    protected BoundedValue<float> _zoomBounds;
    protected Matrix _projection;
    protected Matrix _view;
    protected Matrix _modelView;
    protected Matrix _invModelView; 
    protected Vector2 _lookAtRelative = new Vector2(.5f, .5f);
    protected Viewport _viewport;

    public BoundedValue<float> ZoomBounds { get => _zoomBounds; set => _zoomBounds = value; }
    public Viewport Viewport { get => _viewport; set { _viewport = value; IsDirty = true; } }
    public Vector2 LookAtRelative  { get => _lookAtRelative; set { _lookAtRelative = value; IsDirty = true; } }

    [JsonIgnore] public Matrix View =>       _view;
    [JsonIgnore] public Matrix Projection => _projection;
    [JsonIgnore] public Matrix World =>      LocalMatrix;

    public Camera() : this(null, null)
    {

    }

    public Camera(IServiceProvider services, Scene scene) : base(services, scene)
    {
        _zoomBounds = new(float.Epsilon, float.MaxValue);        
    }

    public override void Initialize()
    {
        base.Initialize();

        if(Viewport.Bounds == Rectangle.Empty)
        {
            Viewport = Services.GetGraphicsDevice().Viewport; 
        }
    }

    protected override void UpdateMatricies()
    { 
        if(IsDirty) {
            base.UpdateMatricies();
            _projection = Matrix.CreateOrthographicOffCenter(-Viewport.Width*LookAtRelative.X, Viewport.Width*(1 - LookAtRelative.X), Viewport.Height*(1 - LookAtRelative.Y), -Viewport.Height*LookAtRelative.Y, Viewport.MinDepth, Viewport.MaxDepth); // this is the same as create orthographic
            _view           = Matrix3x2.Identity; //CreateTranslation(Origin); // why double scaling
            _modelView      = _view * World;
            _invModelView   = Matrix.Invert(_modelView);
        }
    }

    public override void SetScale(Vector2 scale)
    {
        // apply bounds
        base.SetScale(new Vector2(
            _zoomBounds.GetValue(scale.X),
            _zoomBounds.GetValue(scale.Y)
        ));
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        bool bounded = false;
        if(bounded)
        {
            //var ofs_x = Position.X % Size.Width;
            //var ofs_y = Position.Y % Size.Height;

            //if(ofs_x < 0) ofs_x += Size.Width;
            //if(ofs_y < 0) ofs_y += Size.Height;

            //SetPosition( ofs_x, ofs_y);
        }
    }

    public override void WorldToLocal(ref Vector2 point)
    {
        Vector2.Transform(ref point, ref _invModelView, out point);
    }

    public override void LocalToWorld(ref Vector2 point)
    {
        Vector2.Transform(ref point, ref _modelView, out point);
    }

    /*public virtual Vector2 TransformWorldToLocal(Vector2 point)
    {
        Matrix inverse = Matrix.Invert(_world * LocalMatrix);
        Vector2.Transform(ref point, ref inverse, out point);
        return point;
    }

    public virtual IEnumerable<Vector2> TransformWorldToLocal(params Vector2[] points)
    {
        Matrix inverse = Matrix.Invert(_world * LocalMatrix);
        return points.Select(n => Vector2.Transform(n, inverse));
    }

    public virtual RectangleF TransformWorldToLocal(RectangleF rectangleF)
    {
        var topLeft = TransformWorldToLocal(rectangleF.TopLeft);
        var bottomRight = TransformWorldToLocal(rectangleF.BottomRight);
        return new RectangleF(topLeft, bottomRight - topLeft);
    }

    public virtual Vector2 TransformLocalToWorld(Vector2 point)
    {
        Matrix inverse = _world * LocalMatrix;
        Vector2.Transform(ref point, ref inverse, out point);
        return point;
    }

    public virtual IEnumerable<Vector2> TransformLocalToWorld(params Vector2[] points)
    {
        Matrix inverse = _world * LocalMatrix;        
        return points.Select(n => Vector2.Transform(n, inverse));
    }

    public virtual RectangleF TransformLocalToWorld(RectangleF rectangleF)
    {
        var topLeft = TransformLocalToWorld(rectangleF.TopLeft);
        var bottomRight = TransformLocalToWorld(rectangleF.BottomRight);
        return new RectangleF(topLeft, bottomRight - topLeft);
    }
    */
}

    /*public class Camera2D : Camera<Vector2>, IMovable, IRotatable
    {
        private readonly ViewportAdapter _viewportAdapter;
        private Spatial2D _spatial;
        public Spatial2D Spatial => _spatial;
        public override Vector2 Position { get => _spatial.Position - Origin; set => _spatial.Position = value - Origin; }
        public override Vector2 Origin { get => _spatial.Origin; set => _spatial.Origin = value; }
        public override Vector2 Center { get => _spatial.Position; }
        public override float Rotation { get => _spatial.Rotation; set => _spatial.Rotation = value; }
        public override float MinimumZoom { get; set; }
        public override float MaximumZoom { get; set; }
        public BoundingFrustum BoundingFrustum => GetBoundingFrustum();
        public override float Zoom  { get => _spatial.Scale.X; set => _spatial.Scale = new Vector2(value, value); }

        public override RectangleF BoundingRectangle
        {
            get
            {
                Vector3[] corners = GetBoundingFrustum().GetCorners();
                Vector3 vector = corners[0];
                Vector3 vector2 = corners[2];
                float width = vector2.X - vector.X;
                float height = vector2.Y - vector.Y;
                return new RectangleF(vector.X, vector.Y, width, height);
            }
        }

        public Camera2D(GraphicsDevice graphicsDevice)
            : this(new DefaultViewportAdapter(graphicsDevice))
        {
        }

        public Camera2D(ViewportAdapter viewportAdapter)
        {
            _viewportAdapter = viewportAdapter;
            _spatial = new Spatial2D(new Vector2(viewportAdapter.VirtualWidth, viewportAdapter.VirtualHeight))
            {
                OriginNormalized = new Vector2(.5f, .5f)
            };
            MinimumZoom = .1f;            
            MaximumZoom = 100;
            LookAt(Vector2.Zero);
        }

        public override void Move(Vector2 direction)
        {
            Spatial.Move(direction);            
        }
        public override void Rotate(float deltaRadians)
        {
            Spatial.Rotate(deltaRadians);
        }
        public override void ZoomIn(float deltaZoom)
        {
            ClampZoom(Zoom + deltaZoom);
        }

        public override void ZoomOut(float deltaZoom)
        {
            ClampZoom(Zoom - deltaZoom);
        }

        private void ClampZoom(float value)
        {
            if (value < MinimumZoom)
            {
                Spatial.SetScale(MinimumZoom);
            }
            else
            {
                Spatial.SetScale((value > MaximumZoom) ? MaximumZoom : value);
            }
        }

        public override void LookAt(Vector2 position)
        {
            Position = position;//new Vector2((float)_viewportAdapter.VirtualWidth / 2f, (float)_viewportAdapter.VirtualHeight / 2f);
        }

        public Vector2 WorldToScreen(float x, float y)
        {
            return WorldToScreen(new Vector2(x, y));
        }

        public override Vector2 WorldToScreen(Vector2 worldPosition)
        {
            Viewport viewport = _viewportAdapter.Viewport;
            return Vector2.Transform(worldPosition + new Vector2(viewport.X, viewport.Y), GetViewMatrix());
        }

        public Vector2 ScreenToWorld(float x, float y)
        {
            return ScreenToWorld(new Vector2(x, y));
        }

        public override Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            Viewport viewport = _viewportAdapter.Viewport;
            return Vector2.Transform(screenPosition - new Vector2(viewport.X, viewport.Y), Matrix.Invert(GetViewMatrix()));
        }

        public Matrix GetViewMatrix(Vector2 parallaxFactor)
        {
            return GetVirtualViewMatrix(parallaxFactor) * _viewportAdapter.GetScaleMatrix();
        }

        private Matrix GetVirtualViewMatrix(Vector2 parallaxFactor)
        {
            return Spatial.LocalWorldMatrix; // remove position, adjust origion, rotate aroung origin, add origion, add position == 0
        }

        private Matrix GetVirtualViewMatrix()
        {
            return GetVirtualViewMatrix(Vector2.One);
        }

        public override Matrix GetViewMatrix()
        {
            return GetViewMatrix(Vector2.One);
        }

        public override Matrix GetInverseViewMatrix()
        {
            return Matrix.Invert(GetViewMatrix());
        }

        private Matrix GetProjectionMatrix(Matrix viewMatrix)
        {
            Matrix matrix = Matrix.CreateOrthographicOffCenter(0f, _viewportAdapter.VirtualWidth, _viewportAdapter.VirtualHeight, 0f, -1f, 0f);
            Matrix.Multiply(ref viewMatrix, ref matrix, out matrix);
            return matrix;
        }

        public override BoundingFrustum GetBoundingFrustum()
        {
            Matrix virtualViewMatrix = GetVirtualViewMatrix();
            return new BoundingFrustum(GetProjectionMatrix(virtualViewMatrix));
        }

        public ContainmentType Contains(Point point)
        {
            return Contains(point.ToVector2());
        }

        public override ContainmentType Contains(Vector2 vector2)
        {
            return GetBoundingFrustum().Contains(new Vector3(vector2.X, vector2.Y, 0f));
        }

        public override ContainmentType Contains(Rectangle rectangle)
        {
            Vector3 max = new Vector3(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, 0.5f);
            Vector3 min = new Vector3(rectangle.X, rectangle.Y, 0.5f);
            BoundingBox box = new BoundingBox(min, max);
            return GetBoundingFrustum().Contains(box);
        }

        public Matrix View => GetViewMatrix();
        public Matrix Projection => GetProjectionMatrix(Matrix.Identity);
        public Matrix World => Matrix.Identity;
    }
}*/