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
    public class Material2D
    {
        public SubTexture2D SubTexture2D;
        public float Alpha;
        Vector3 DiffuseColor;
        Vector3 EmissiveColor;
        bool FogEnabled;
        Vector3 FogColor;
        float FogEnd;
        float FogStart;
        bool TextureEnabled;

        public Material2D(SubTexture2D subTexture2D) {
            Alpha = 1.0f;
            SubTexture2D = subTexture2D;
            FogEnabled = false;
            TextureEnabled = true;
        }

        public Material2D(Texture2D texture2D): this (new SubTexture2D(texture2D)) {

        }
        

        //public Effect ToEffect() {
// 
        //}
    }
}