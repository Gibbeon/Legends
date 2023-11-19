using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;

// 32 * (64 / 1024)

namespace Legends.App.Global
{
    public static class Defaults
    {
        public static Texture2D? Texture { get; set; }

        public static void LoadContent(ContentManager manager)
        {
            //Texture = manager.Load<Texture2D>("npc1");
            
            Texture = new Texture2D(manager.GetGraphicsDevice(), 64, 64);
            uint[] data = new uint[Texture.Width * Texture.Height];
            Color[] rainbow = GetGradients(Color.White, Color.Black, Texture.Height).ToArray();
            
            for(uint i = 0; i < Texture.Height; i++)
            {

                    var array = Enumerable.Repeat(rainbow[i].ToArgb(), Texture.Width).ToArray();

                    Array.Copy(array, 0, data,  i * Texture.Width, array.Length);
                
            }
            
            Texture.SetData<uint>(data);
            
        }

        public static IEnumerable<Color> GetGradients(Color start, Color end, int steps)
        {
            int stepA = ((end.A - start.A) / (steps - 1));
            int stepR = ((end.R - start.R) / (steps - 1));
            int stepG = ((end.G - start.G) / (steps - 1));
            int stepB = ((end.B - start.B) / (steps - 1));

            for (int i = 0; i < steps; i++)
            {
                yield return Color.FromArgb(start.A + (stepA * i),
                                            start.R + (stepR * i),
                                            start.G + (stepG * i),
                                            start.B + (stepB * i));
            }
        }
    }
}