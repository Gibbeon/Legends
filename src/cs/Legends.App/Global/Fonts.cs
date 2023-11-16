using System.Drawing;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Content;
using MonoGame.Extended.Sprites;

namespace Legends.App.Global
{
    public static class Fonts
    {
        public static BitmapFont Menu;

        public static void LoadContent(ContentManager manager)
        {
            Menu = manager.Load<BitmapFont>("Sensation");
        }
    }

    public static class Defaults
    {
        public static Texture2D Texture;

        public static void LoadContent(ContentManager manager)
        {
            Texture = new Texture2D(manager.GetGraphicsDevice(), 128, 128);
            Texture.SetData<int>(Enumerable.Repeat(Color.Green.ToArgb(), Texture.Width * Texture.Height).ToArray());
        }
        
    }
}