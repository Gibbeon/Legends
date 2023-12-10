using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended;
using Legends.Engine.Graphics2D;
using Microsoft.Xna.Framework.Content;
using SharpDX.D3DCompiler;

namespace Legends.Content.Pipline;

public static class ContentPrimitivesExtensions
{
    public static void Write(this ContentWriter output, Size2 size2)
    {
        output.Write(size2.Width);
        output.Write(size2.Height);
    }

    public static Size2 ReadSize2(this ContentReader input)
    {
        return new Size2(input.ReadSingle(), input.ReadSingle());
    }
}