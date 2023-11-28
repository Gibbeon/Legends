using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Legends.Engine;

namespace Legends.Content.Pipline;

[ContentTypeWriter]
public class SceneObjectWriter : ContentTypeWriter<SceneObject.SceneObjectDesc>
{
    protected override void Write(ContentWriter output, SceneObject.SceneObjectDesc value)
    {  
        output.WriteAll<SceneObject.SceneObjectDesc>(value);
    }

    public override string GetRuntimeType(TargetPlatform targetPlatform)
    {
        return GetType().BaseType.GenericTypeArguments[0].AssemblyQualifiedName;
    }

    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return GetType().AssemblyQualifiedName;
    }
}
