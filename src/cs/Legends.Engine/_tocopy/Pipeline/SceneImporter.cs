using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Newtonsoft.Json;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Legends.Engine;
using Legends.Engine.Serialization;
using System;
using Legends.Engine.Graphics2D;
using System.Linq;
using System.Reflection;
using System.Collections;
using MonoGame.Extended;

namespace Legends.Content.Pipline;


[ContentImporter(".json", DisplayName = "Legends Scene Importer", DefaultProcessor = "SceneProcessor")]
public class SceneImporter : ContentImporter<Scene.SceneDesc>
{
    public override Scene.SceneDesc Import(string filename, ContentImporterContext context)
    {
        context.Logger.LogMessage("Importing file: {0}", filename);
        try
        {
            var settings = new Newtonsoft.Json.JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                //TypeNameAssemblyFormatHandling = Newtonsoft.Json.TypeNameAssemblyFormatHandling.Simple,
                    SerializationBinder = new KnownTypesBinder("Legends.Engine"),
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto
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

[ContentProcessor(DisplayName = "Legends Scene Processor")]
class SceneProcessor : ContentProcessor<Scene.SceneDesc, Scene.SceneDesc>
{
    public override Scene.SceneDesc Process(Scene.SceneDesc input, ContentProcessorContext context)
    {
        return input;
    }
}