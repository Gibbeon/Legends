using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Legends.Engine;
using Legends.Engine.Serialization;
using Legends.Engine.Graphics2D;

namespace Legends.Content.Pipline;

[ContentTypeWriter]

public class TextBehaviorWriter : ContentTypeWriter<TextRenderBehavior.TextRenderBehaviorDesc>
{
    protected override void Write(ContentWriter output, TextRenderBehavior.TextRenderBehaviorDesc value)
    {  
        output.WriteRawObject<ActivatorDesc>(value);

        output.Write(value.Text);
        output.Write(value.Color);
        output.Write(value.Font);
    }

    public override string GetRuntimeType(TargetPlatform targetPlatform)
    {
        return typeof(TextRenderBehavior.TextRenderBehaviorDesc).AssemblyQualifiedName;
    }

    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return typeof(TextBehaviorWriter).AssemblyQualifiedName;
    }
}





