using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Newtonsoft.Json;
using Legends.Engine;
using Legends.Engine.Graphics2D;

namespace Legends.Content.Pipline;

public class AssetJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType.IsAssignableTo(typeof(Asset));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object value, JsonSerializer serializer)
    {
        return Activator.CreateInstance(objectType, reader.Value);
        ///return new Asset(reader.Value.ToString());
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteValue((value as Asset).Name);
    }
}

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
                //Formatting = Formatting.Indented,
                //ObjectCreationHandling = ObjectCreationHandling.Auto,
                //NullValueHandling = NullValueHandling.Ignore,
                //TypeNameAssemblyFormatHandling = Newtonsoft.Json.TypeNameAssemblyFormatHandling.Simple,
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
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