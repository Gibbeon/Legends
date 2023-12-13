using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended.Content;

namespace Legends.Engine.Content;

[ContentTypeWriter]
public class ContentObjectWriter : ContentTypeWriter<ContentObject>
{
    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return typeof(ContentObjectReader).AssemblyQualifiedName;
    }

    protected override void Write(ContentWriter output, ContentObject value)
    {
        output.WriteObject(value.Instance, value.Instance.GetType());
    }
}

public class ContentObjectReader : ContentTypeReader<ContentObject>
{
    protected override ContentObject Read(ContentReader input, ContentObject existingInstance)
    {
        var result = input.ReadComplexObject(existingInstance, typeof(object));

        return ContentObject.Wrap(result);       
    }
}