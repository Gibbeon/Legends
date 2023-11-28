using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Legends.Engine;
using Legends.Engine.Serialization;
using Legends.Engine.Graphics2D;

namespace Legends.Content.Pipline;

[ContentTypeWriter]

public class SceneWriter : ContentTypeWriter<Scene.SceneDesc>
{
    protected override void Write(ContentWriter output, Scene.SceneDesc value)
    {  
        output.WriteRawObject<SceneObject.SceneObjectDesc>(value);
        output.WriteObject<Camera.CameraDesc>(value.Camera);
    }

    public override string GetRuntimeType(TargetPlatform targetPlatform)
    {
        return typeof(Scene.SceneDesc).AssemblyQualifiedName;
    }

    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return typeof(SceneWriter).AssemblyQualifiedName;
    }
}
