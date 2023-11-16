using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Legends.App.Graphics
{
    public interface ISpriteBatchDrawable2D : IDrawable
    {
        void DrawBatched(SpriteBatch batch, GameTime gameTime);
    }
    
    public struct TextureView2D
    {
        public Texture2D Texture;

        public Rectangle Bounds
        {
            get 
            {
                return new Rectangle(Offset.ToPoint(), (Point)FrameSize);
            }
        }

        public int FrameCount
        {
            get; private set;
        }

        public Vector2 Offset
        {
            get; private set;
        }

        public Size2 FrameSize
        {
            get; private set;
        }

        public TextureView2D(Texture2D texture)
        {
            Texture = texture;
            FrameSize = new Size2(texture.Width, texture.Height);
            FrameCount = 0;
            Offset = Vector2.Zero;
        }
    }

    public class Material2D
    {
        public TextureView2D ColorMap { get; set; }
        public SpriteEffects Effects { get; set; }
        public Color AmbientColor { get; set; }

        public Material2D(Texture2D texture = null)
        {
            ColorMap = new TextureView2D(texture ?? Global.Defaults.Texture);  
            AmbientColor = Color.White;
        }
    }

    public class DrawableEntity2D : ISpriteBatchDrawable2D
    {
        private Game _game;

        public Spatial2D Spatial { get; protected set; }

        public Material2D Material { get; protected set; }

        public int DrawOrder => throw new NotImplementedException();

        public bool Visible => throw new NotImplementedException();

        public int Depth;

        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;

        public DrawableEntity2D(Game game)
        {
            _game = game; 
            Material = new Material2D();
            Spatial = new Spatial2D(new Size2(128, 128)); 
        }

        public void DrawBatched(SpriteBatch batch, GameTime gameTime)
        {           
            batch.Draw(
                Material.ColorMap.Texture,
                Spatial.Bounds,
                Material.ColorMap.Bounds,
                Material.AmbientColor,
                Spatial.Rotation,
                Spatial.Origin,
                Material.Effects,
                Depth);
        }

        public void Draw(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}