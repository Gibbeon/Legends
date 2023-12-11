using Legends.Engine.Runtime;
using Legends.Engine.Serialization;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Legends.Engine.Content;

[ContentTypeWriter]
public class DynamicAssemblyWriter : ContentTypeWriter<DynamicAssembly>
{
    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return typeof(DynamicAssemblyReader).AssemblyQualifiedName;
    }

    protected override void Write(ContentWriter output, DynamicAssembly value)
    {
        value.Write(output);
    }
}

public class DynamicAssemblyReader : ContentTypeReader<DynamicAssembly>
{
    protected override DynamicAssembly Read(ContentReader input, DynamicAssembly existingInstance)
    {
        existingInstance ??= (DynamicAssembly)typeof(DynamicAssembly).Create();
        existingInstance.Read(input);
        return existingInstance;
    }
}