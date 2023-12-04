using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Newtonsoft.Json;
using Legends.Engine;
using Legends.Content.Pipline.JsonConverters;
using Legends.Engine.Serialization;

namespace Legends.Content.Pipline;

public static class StdStuff
{
    public static string Code = @"
        using Microsoft.Xna.Framework.Content.Pipeline;
        using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
        using Legends.Engine;
        using Legends.Engine.Graphics2D;
        using Legends.Engine.Serialization;

        namespace Legends.Content.Pipline;
    
        [ContentTypeWriter]
        public class {0}Writer : ContentTypeWriter<{0}> {{
            protected override void Write(ContentWriter output, {0} value)         {{ this.GenericWriteObject(output, value); }}
            public override string GetRuntimeType(TargetPlatform targetPlatform)   {{ return GetType().BaseType.GenericTypeArguments[0].AssemblyQualifiedName; }}
            public override string GetRuntimeReader(TargetPlatform targetPlatform) {{ return typeof(GenericReader<>).MakeGenericType(GetType().BaseType.GenericTypeArguments[0]).AssemblyQualifiedName; }}
        }}";
}

[ContentImporter(".json", DisplayName = "Legends Scene Importer", DefaultProcessor = "SceneProcessor")]
public class SceneImporter : ContentImporter<Scene>
{
    public override Scene Import(string filename, ContentImporterContext context)
    {
        context.Logger.LogMessage("Importing file: {0}", filename);
        try
        {            
            DynamicClassLoader.Compile("Scene",                 string.Format(StdStuff.Code, "Scene"));       
            DynamicClassLoader.Compile("SceneObject",           string.Format(StdStuff.Code, "SceneObject"));        
            DynamicClassLoader.Compile("Spatial",               string.Format(StdStuff.Code, "Spatial"));        
            DynamicClassLoader.Compile("Camera",                string.Format(StdStuff.Code, "Camera"));
            DynamicClassLoader.Compile("TextRenderBehavior",    string.Format(StdStuff.Code, "TextRenderBehavior"));
            
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
            };

            settings.Converters.Add(new AssetJsonConverter());
            
            var result = JsonConvert.DeserializeObject<Scene>(File.ReadAllText(filename), settings);

            context.Logger.LogMessage(JsonConvert.ToString(JsonConvert.SerializeObject(result, settings)).Replace("{", "{{").Replace("}", "}}"));

            return result ?? new Scene(null);
        } 
        catch(Exception err)
        {
            context.Logger.LogImportantMessage(err.Message);
            throw;
        }
    }
}

[ContentProcessor(DisplayName = "Legends Scene Processor")]
class SceneProcessor : ContentProcessor<Scene, Scene>
{
    public override Scene Process(Scene input, ContentProcessorContext context)
    {
        return input;
    }
}