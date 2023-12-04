﻿using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Legends.Engine;
using Legends.Engine.Graphics2D;
using Legends.Engine.Serialization;

namespace Legends.Content.Pipline;

/*

[ContentTypeWriter]
public class SceneWriter : ContentTypeWriter<Scene>
{
    protected override void Write(ContentWriter output, Scene value)
    {  
        this.GenericWriteObject(output, value);
    }

    public override string GetRuntimeType(TargetPlatform targetPlatform)
    {
        return GetType().BaseType.GenericTypeArguments[0].AssemblyQualifiedName;
    }

    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return typeof(GenericReader<>).MakeGenericType(GetType().BaseType.GenericTypeArguments[0]).AssemblyQualifiedName;
    }   
}



[ContentTypeWriter]
public class SceneObjectWriter : ContentTypeWriter<SceneObject>
{
    protected override void Write(ContentWriter output, SceneObject value)
    {  
        this.GenericWriteObject(output, value);
    }

    public override string GetRuntimeType(TargetPlatform targetPlatform)
    {
        return GetType().BaseType.GenericTypeArguments[0].AssemblyQualifiedName;
    }

    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return typeof(GenericReader<>).MakeGenericType(GetType().BaseType.GenericTypeArguments[0]).AssemblyQualifiedName;
    }
}

[ContentTypeWriter]
public class SpatialWriter : ContentTypeWriter<Spatial>
{
    protected override void Write(ContentWriter output, Spatial value)
    {  
        this.GenericWriteObject(output, value);
    }

    public override string GetRuntimeType(TargetPlatform targetPlatform)
    {
        return GetType().BaseType.GenericTypeArguments[0].AssemblyQualifiedName;
    }

    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return typeof(GenericReader<>).MakeGenericType(GetType().BaseType.GenericTypeArguments[0]).AssemblyQualifiedName;
    }
}

[ContentTypeWriter]
public class CameraWriter : ContentTypeWriter<Camera>
{
    protected override void Write(ContentWriter output, Camera value)
    {  
        this.GenericWriteObject(output, value);
    }

    public override string GetRuntimeType(TargetPlatform targetPlatform)
    {
        return GetType().BaseType.GenericTypeArguments[0].AssemblyQualifiedName;
    }

    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return typeof(GenericReader<>).MakeGenericType(GetType().BaseType.GenericTypeArguments[0]).AssemblyQualifiedName;
    }
}

[ContentTypeWriter]
public class TextRenderBehaviorWriter : ContentTypeWriter<TextRenderBehavior>
{
    protected override void Write(ContentWriter output, TextRenderBehavior value)
    {  
        this.GenericWriteObject(output, value);
    }

    public override string GetRuntimeType(TargetPlatform targetPlatform)
    {
        return GetType().BaseType.GenericTypeArguments[0].AssemblyQualifiedName;
    }

    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return typeof(GenericReader<>).MakeGenericType(GetType().BaseType.GenericTypeArguments[0]).AssemblyQualifiedName;
    }
}

*/
