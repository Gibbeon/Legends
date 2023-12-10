using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Legends.Engine.Content;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended.Content;

namespace Legends.Engine.Content;

[ContentTypeWriter]
public class ScriptWriter : ContentTypeWriter<Assembly>
{
    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return typeof(ScriptReader).AssemblyQualifiedName;
    }

    protected override void Write(ContentWriter output, Assembly value)
    {
        output.Write(value.FullName);

    }
}

public class ScriptReader : ContentTypeReader<Assembly>
{
    protected override Assembly Read(ContentReader input, Assembly existingInstance)
    {
        Console.WriteLine(input.ReadString());

        return null;
    }
}