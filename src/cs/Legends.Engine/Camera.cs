using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.IO.Pipes;

namespace Legends.Engine;

public class Camera : SceneObject, IViewState
{
    protected BoundedValue<float> _zoomBounds;
    protected Matrix _projection;
    protected Matrix _world;
    
    [JsonIgnore]
    public BoundedValue<float> ZoomBounds => _zoomBounds;
    
    [JsonIgnore]
    public Matrix View => _world;
    
    [JsonIgnore]
    public Matrix Projection => _projection;
    
    [JsonIgnore]
    public Matrix World => LocalMatrix;
    
    [JsonIgnore]
    public IViewState ViewState => new ViewState() { View = View, Projection = Projection, World = World };

    public Camera() : base(null, null)
    {

    }

    public Camera(IServiceProvider? services, Scene? scene) : base(services, scene)
    {
        _zoomBounds = new BoundedValue<float>(float.Epsilon, float.MaxValue);
    }

    public override void Initialize()
    {
        base.Initialize();

        OriginNormalized = new Vector2(.5f, .5f);
        SetSize(0, 0);
    }

    public override void SetSize(Size2 size)
    {
        if(size == Size2.Empty)
        {
            if(Services != null)
            {
                size = new Vector2(Services.GetGraphicsDevice().Viewport.Width, Services.GetGraphicsDevice().Viewport.Height);
            }
            else
            {
                return;
            }
        }

        base.SetSize(size);
        
        _world      = Matrix.CreateTranslation(Size.Width / 2, Size.Height / 2, 0.0f);
        _projection = Matrix.CreateOrthographicOffCenter(0f, Size.Width, Size.Height, 0f, -1f, 0f);
        HasChanged = true;
    }

    public override void SetScale(Vector2 scale)
    {
        // apply bounds
        base.SetScale(new Vector2(
            _zoomBounds.GetValue(scale.X),
            _zoomBounds.GetValue(scale.Y)
        ));

        var adjustedSize = (Size2)(this.Size / (Scale));

        _world = Matrix.CreateTranslation(adjustedSize.Width / 2, adjustedSize.Height / 2, 0.0f);
        _projection = Matrix.CreateOrthographicOffCenter(0f, adjustedSize.Width, adjustedSize.Height, 0f, -1f, 0f);
    }

    public void LookAt(Vector2 vector)
    {
        Position = vector;
    }

    public override RectangleF GetBoundingRectangle()
    {
        return new RectangleF(Position -(Origin / (Scale * Scale)), Size / (Scale * Scale));
    }
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