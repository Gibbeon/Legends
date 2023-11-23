using Microsoft.Xna.Framework;

namespace LitEngine.Framework.Graphics
{
    public static class Utils
    {
        public static Color BlendMultiply(Color src, Color dest)
        {
            var result = new Color(src.R * dest.R / 255, src.G * dest.G / 255, src.B * dest.B / 255, src.A * dest.A / 255);
            return result;
        }
    }
}