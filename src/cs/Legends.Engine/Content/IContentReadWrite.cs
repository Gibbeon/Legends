using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Legends.Engine.Content;

public interface IContentReadWrite
{
    public void Read(ContentReader reader);
    public void Write(ContentWriter writer);
}