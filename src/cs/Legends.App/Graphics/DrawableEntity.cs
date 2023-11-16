using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Legends.App.Graphics
{
    public interface ISpriteBatchDrawable2D
    {
        void DrawBatched(SpriteBatch batch, Camera2D camera, GameTime gameTime);

    }

    public class Camera2D
    {

    }

    public struct TextureView2D
    {
        public Texture2D Texture;

        public Rectangle Frame
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

    public class DrawableEntity : ISpriteBatchDrawable2D
    {
        private Game _game;

        public Spatial2D Spatial { get; protected set; }

        public Material2D Material { get; protected set; }

        public int Depth;

        public DrawableEntity(Game game)
        {
            _game = game; 
            Material = new Material2D();
            Spatial = new Spatial2D(new Size2(128, 128)); 
        }

        public void DrawBatched(SpriteBatch batch, Camera2D camera, GameTime gameTime)
        {           
            batch.Draw(
                Material.ColorMap.Texture,
                Spatial.Frame,
                Material.ColorMap.Frame,
                Material.AmbientColor,
                Spatial.Rotation,
                Spatial.Origin,
                Material.Effects,
                Depth);
        }
    }
}