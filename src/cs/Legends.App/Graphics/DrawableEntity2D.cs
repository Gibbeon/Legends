using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace Legends.App.Graphics
{
    public interface ISpriteBatchDrawable2D : IDrawable
    {
        void DrawBatched(SpriteBatch batch, GameTime gameTime);
    }
    
    public struct TextureView2D
    {
        public Texture2D Texture;

        public Rectangle Bounds;

        public TextureView2D(Texture2D texture)
        {
            Texture = texture;
            Bounds = new Rectangle(0, 0, texture.Width, texture.Height);
        }
    }

    public class Material2D
    {
        public TextureView2D ColorMap;
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
            Spatial = new Spatial2D(); 
        }

        public void DrawBatched(SpriteBatch batch, GameTime gameTime)
        { 
            //BUG: When Source(texture) Size is Different than Destination Size, The Origin translates things oddly
           // var ratioX = Material.ColorMap.Bounds.Width     / Spatial.Size.Width;
            //var ratioY = Material.ColorMap.Bounds.Height    / Spatial.Size.Height;

            //Vector2 posAdj = new Vector2(Spatial.Origin.X / ratioX, Spatial.Origin.Y / ratioY);
            //new Rectangle((Spatial.Position + posAdj - Spatial.Origin).ToPoint(), (Point)Spatial.Size) <-- this undoes it but then doesn't work with rotation axis
               
            // so instead I bounded the texture map to the size. I don't love this

            batch.Draw(
                Material.ColorMap.Texture,
                new Rectangle((Spatial.Position).ToPoint(), (Point)Spatial.Size),
                new Rectangle(Material.ColorMap.Bounds.Location, 
                    new Point((int)MathF.Min(Material.ColorMap.Bounds.Size.X, Spatial.Size.Width),(int)MathF.Min(Material.ColorMap.Bounds.Size.Y, Spatial.Size.Height))),
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