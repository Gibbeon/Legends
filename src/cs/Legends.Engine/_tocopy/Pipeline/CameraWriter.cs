using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Legends.Engine;
using Legends.Engine.Serialization;
using Legends.Engine.Graphics2D;

namespace Legends.Content.Pipline;

[ContentTypeWriter]
public class CameraWriter : ContentTypeWriter<Camera.CameraDesc>
{
    protected override void Write(ContentWriter output, Camera.CameraDesc value)
    {  
        output.WriteRawObject<SceneObject.SceneObjectDesc>(value);
    }

    public override string GetRuntimeType(TargetPlatform targetPlatform)
    {
        return typeof(Camera.CameraDesc).AssemblyQualifiedName;
    }

    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return typeof(CameraWriter).AssemblyQualifiedName;
    }
}
