using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Legends.Engine;
using Legends.Engine.Serialization;
using Legends.Engine.Graphics2D;

namespace Legends.Content.Pipline;

[ContentTypeWriter]

public class SpatialWriter : ContentTypeWriter<Spatial.SpatialDesc>
{
    protected override void Write(ContentWriter output, Spatial.SpatialDesc value)
    {          
        output.WriteRawObject<ActivatorDesc>(value);

        output.Write(value.Position);
        output.Write(value.Scale);
        output.Write(value.Origin);
        output.Write(value.Rotation);
        output.Write(value.Size.Width);
        output.Write(value.Size.Height);
    }

    public override string GetRuntimeType(TargetPlatform targetPlatform)
    {
        return typeof(Spatial.SpatialDesc).AssemblyQualifiedName;
    }

    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return typeof(SpatialReader).AssemblyQualifiedName;
    }
}
