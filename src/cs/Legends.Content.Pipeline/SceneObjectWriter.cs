using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Legends.Engine;
using Legends.Engine.Graphics2D;

namespace Legends.Content.Pipline;

[ContentTypeWriter]
public class SceneWriter : ContentTypeWriter<Scene>
{
    protected override void Write(ContentWriter output, Scene value)
    {  
        output.WriteAll(value);
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

[ContentTypeWriter]
public class SceneObjectWriter : ContentTypeWriter<SceneObject>
{
    protected override void Write(ContentWriter output, SceneObject value)
    {  
        output.WriteAll(value);
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

[ContentTypeWriter]
public class SpatialWriter : ContentTypeWriter<Spatial>
{
    protected override void Write(ContentWriter output, Spatial value)
    {  
        output.WriteAll(value);
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

[ContentTypeWriter]
public class CameraWriter : ContentTypeWriter<Camera>
{
    protected override void Write(ContentWriter output, Camera value)
    {  
        output.WriteAll(value);
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

[ContentTypeWriter]
public class TextRenderBehaviorWriter : ContentTypeWriter<TextRenderBehavior>
{
    protected override void Write(ContentWriter output, TextRenderBehavior value)
    {  
        output.WriteAll(value);
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


