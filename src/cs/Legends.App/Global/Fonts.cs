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
        public static BitmapFont? Menu;

        public static void LoadContent(ContentManager manager)
        {
            //Menu = manager.Load<BitmapFont>("Sensation");
        }
    }
}