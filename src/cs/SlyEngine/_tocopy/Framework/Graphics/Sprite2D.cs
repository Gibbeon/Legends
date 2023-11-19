using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LitEngine.Framework.Graphics
{
    public class Sprite2D : SpatialNode, IDrawable2D
    {
        public event EventHandler<EventArgs>? VisibleChanged;
        public event EventHandler<EventArgs>? DrawOrderChanged;
        public Material2D Material { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        protected Vector3 WorldSize { get; set; }
        public SpriteEffects SpriteEffects { get; set; }
        public Color AmbientColor { get; set; }

        public bool Visible
        {
            get => _visible;
            set => SetVisible(value);
        }

        public int DrawOrder
        {
            get => _drawOrder;
            set => SetDrawOrder(value);
        }

        public float Depth
        {
            get;
            set;
        }

        protected int _drawOrder;
        protected bool _visible;

        public Sprite2D(Rectangle rectangle, Material2D material) :
            this(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, material)
        {

        }

        public Sprite2D(float x, float y, float width, float height, Material2D material)
        {
            SetPosition(new Vector3(x, y, 0));

            Width = width;
            Height = height;
            Material = material;
            _visible = true;

            WorldSize = Vector3.Transform(new Vector3(Width, Height, 0), GlobalWorldMatrix) - Vector3.Transform(Position, GlobalWorldMatrix);
        }

        public virtual void DrawBatched(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if(!Visible) return;

            var color = new Color(Color.Red, (int)(255 * Material.Alpha));

            spriteBatch.Draw(
                Material.SubTexture2D.Texture2D,
                    new Vector2(GlobalWorldMatrix.Translation.X, GlobalWorldMatrix.Translation.Y),
                    //new Rectangle(0, 0, TileSet.Width, TileSet.Height),
                    new Rectangle(0, 0, (int)Width, (int)Height), //Material.SubTexture2D.Source
                    color,
                    0,
                    Vector2.Zero,
                    Vector2.One,
                    SpriteEffects,
                    Depth);
        }

        public virtual void Draw(GameTime gameTime)
        {
            if(!Visible) return;

            //var vertexBuffer = new VertexBuffer(GameEngine.Instance.GraphicsDevice, new VertexDeclaration[] {
            //  new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0)
            // }, 2, BufferUsage.None);
            //vertexBuffer.SetData<Vector3>(-, new Vector3[] { GlobalWorldMatrix.Translation, GlobalWorldMatrix.} )
            //GameEngine.Instance.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            //GameEngine.Instance.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
        }
        public override void Update(GameTime gameTime)
        {
            if(!Enabled) return;

            if (HasChanged)
            {
                base.Update(gameTime);
                WorldSize = Vector3.Transform(new Vector3(Width, Height, 0), GlobalWorldMatrix) - GlobalWorldMatrix.Translation;
            }
            else
            {
                base.Update(gameTime);
            }
        }

        protected void SetVisible(bool value)
        {
            if (_visible != value)
            {
                _visible = value;
                if (VisibleChanged != null)
                {
                    VisibleChanged(this, EventArgs.Empty);
                }
            }
        }
        protected void SetDrawOrder(int value)
        {
            if (_drawOrder != value)
            {
                _drawOrder = value;
                if (DrawOrderChanged != null)
                {
                    DrawOrderChanged(this, EventArgs.Empty);
                }
            }
        }
    }
}
