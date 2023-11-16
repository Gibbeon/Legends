using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Legends.App.Graphics
{
    public class Spatial2D
    {
        public Transform2 Transform;
        public Size2 Size {
            get { return new Size2(OrigionalSize.Width * Scale.X, OrigionalSize.Height * Scale.Y); }
        }

        public Vector2 Origin { get; set; }
        public Vector2 Position
        {
            get { return Transform.Position; }
            set { Transform.Position = value; }
        }

        public Vector2 Scale
        {
            get { return Transform.Scale; }
            set { Transform.Scale = value; }
        }

        public float Rotation
        {
            get { return Transform.Rotation; }
            set { Transform.Rotation = value; }
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

        public Size2 OrigionalSize { get; set; }

        public Rectangle Frame
        {
            get 
            {
                return new Rectangle(Position.ToPoint(), (Point)Size);
            }

        }

        public Spatial2D()
        {
            Transform = new Transform2();
        }

        public Spatial2D(Size2 size)
        {
            Transform = new Transform2();
            OrigionalSize = size;
        }
    }
}    