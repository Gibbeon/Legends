using System;
using System.IO;
using LitEngine.Framework;
using LitEngine.Framework.Graphics;
using LitEngine.Framework.Graphics.Animation;
using LitEngine.Framework.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;

namespace LitEngine.Framework.Graphics
{   
    public struct SubTexture2D
    {
        public int TextureID;
        public Texture2D Texture2D;
        public Rectangle Source;

        public SubTexture2D(Texture2D texture) {
            TextureID = 0;
            Texture2D = texture;
            Source = texture.Bounds;
        }
    }
}