using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended;
using Legends.Engine.Graphics2D;

namespace Legends.Content.Pipline;

public static class ContentWriterPrimitivesExtensions
{
    public static void Write(this ContentWriter output, Size2 size2)
    {
        output.Write(size2.Width);
        output.Write(size2.Height);
    }

    /*public static void Write(this ContentWriter output, Asset asset)
    {
        output.Write(asset.Name);
    }*/
}