using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Newtonsoft.Json;
using Legends.Engine;

namespace Legends.Content.Pipline;

[ContentImporter(".json", DisplayName = "Legends Scene Importer", DefaultProcessor = "SceneProcessor")]
public class SceneImporter : ContentImporter<Scene>
{
    public override Scene Import(string filename, ContentImporterContext context)
    {
        context.Logger.LogMessage("Importing file: {0}", filename);
        try
        {
            var settings = new Newtonsoft.Json.JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                //TypeNameAssemblyFormatHandling = Newtonsoft.Json.TypeNameAssemblyFormatHandling.Simple,
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto
            };

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