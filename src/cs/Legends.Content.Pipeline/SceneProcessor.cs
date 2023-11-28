using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Legends.Engine;
using Legends.Engine.Serialization;
using System;
using Legends.Engine.Graphics2D;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace Legends.Content.Pipline;


[ContentImporter(".json", DisplayName = "SceneImporter", DefaultProcessor = "SceneImporter")]
public class SceneImporter : ContentImporter<Scene.SceneDesc>
{
    public override Scene.SceneDesc Import(string filename, ContentImporterContext context)
    {
        context.Logger.LogMessage("Importing file: {0}", filename);
        try
        {
            var settings = new Newtonsoft.Json.JsonSerializerSettings
            {
                TypeNameAssemblyFormatHandling = Newtonsoft.Json.TypeNameAssemblyFormatHandling.Simple,
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects,
            };

            var result = JsonConvert.DeserializeObject<Scene.SceneDesc>(File.ReadAllText(filename), settings);

            context.Logger.LogMessage(JsonConvert.ToString(JsonConvert.SerializeObject(result, settings)).Replace("{", "{{").Replace("}", "}}"));

            return result;
        } 
        catch(Exception err)
        {
            context.Logger.LogImportantMessage(err.Message);
            throw;
        }
    }
}

[ContentProcessor(DisplayName = "SceneDataProcessor")]
class SceneProcessor : ContentProcessor<Scene.SceneDesc, Scene.SceneDesc>
{
    public override Scene.SceneDesc Process(Scene.SceneDesc input, ContentProcessorContext context)
    {
        return input;
    }
}

[ContentTypeWriter]
public class SceneObjectWriter : ContentTypeWriter<SceneObject.SceneObjectDesc>
{
    protected override void Write(ContentWriter output, SceneObject.SceneObjectDesc value)
    {  
        output.WriteRawObject<Spatial.SpatialDesc>(value);

        output.Write(value.Name);
        output.Write(value.Enabled);
        output.Write(value.IsVisible);
        output.Write(value.Tags.Count);
        for(int i = 0; i < value.Tags.Count; i++)
        {
            output.Write(value.Tags[i]);
        }

        output.Write(value.Children.Count);
        for(int i = 0; i < value.Children.Count; i++)
        {
            output.WriteObject(value.Children[i]);
        }

        output.Write(value.Behaviors.Count);
        for(int i = 0; i < value.Behaviors.Count; i++)
        {
            output.WriteObject(value.Behaviors[i]);
        }
    }

    public override string GetRuntimeType(TargetPlatform targetPlatform)
    {
        return typeof(SceneObject.SceneObjectDesc).AssemblyQualifiedName;
    }

    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return typeof(SceneObjectReader).AssemblyQualifiedName;
    }
}

public class ActivatorWriter : ContentTypeWriter<ActivatorDesc>
{
    protected override void Write(ContentWriter output, ActivatorDesc value)
    {  
        output.Write(value.TypeOf);
    }

    public override string GetRuntimeType(TargetPlatform targetPlatform)
    {
        return typeof(ActivatorDesc).AssemblyQualifiedName;
    }

    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return typeof(ActivatorWriter).AssemblyQualifiedName;
    }
}

public class SpatialWriter : ContentTypeWriter<Spatial.SpatialDesc>
{
    protected override void Write(ContentWriter output, Spatial.SpatialDesc value)
    {          
        output.WriteRawObject<ActivatorDesc>(value);

        output.Write(value.Position);
        output.Write(value.Scale);
        output.Write(value.Origin);
        output.Write(value.Rotation);
        output.Write(value.Size.Width);
        output.Write(value.Size.Height);
    }

    public override string GetRuntimeType(TargetPlatform targetPlatform)
    {
        return typeof(Spatial.SpatialDesc).AssemblyQualifiedName;
    }

    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return typeof(SpatialReader).AssemblyQualifiedName;
    }
}

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





