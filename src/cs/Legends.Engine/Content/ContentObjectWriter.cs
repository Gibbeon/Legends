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
        return typeof(IAssetObjectReader).AssemblyQualifiedName;
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
        var instance = existingInstance == null ? null : existingInstance.Instance;
        var result = input.ReadComplexObject(instance, typeof(object));

        return ContentObject.Wrap(result);       
    }
}

public class IAssetObjectReader : ContentTypeReader<IAsset>
{
    protected override IAsset Read(ContentReader input, IAsset existingInstance)
    {
        var result = input.ReadComplexObject(existingInstance, typeof(IAsset));

        return result as IAsset;//ContentObject.Wrap(result);       
    }
}