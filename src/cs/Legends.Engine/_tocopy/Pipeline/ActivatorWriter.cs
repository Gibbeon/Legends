using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Legends.Engine;
using Legends.Engine.Serialization;
using Legends.Engine.Graphics2D;

namespace Legends.Content.Pipline;

[ContentTypeWriter]
public class ActivatorWriter : ContentTypeWriter<ActivatorDesc>
{
    protected override void Write(ContentWriter output, ActivatorDesc value)
    {  
        output.Write(value.TypeOf);
    }

    public override string GetRuntimeType(TargetPlatform targetPlatform)
    {
        return typeof(ActivatorDesc).AssemblyQualifiedName;
    }

    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return typeof(ActivatorWriter).AssemblyQualifiedName;
    }
}
