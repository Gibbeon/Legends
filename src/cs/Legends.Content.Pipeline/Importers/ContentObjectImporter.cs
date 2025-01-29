using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Newtonsoft.Json;
using Legends.Content.Pipline.JsonConverters;
using Legends.Engine.Content;
using MonoGame.Extended.Serialization;
using Legends.Engine.Graphics2D;
using Newtonsoft.Json.Converters;
using MonoGame.Extended.Serialization.Json;

namespace Legends.Content.Pipline;

[ContentImporter(".json", DisplayName = "Legends ContentObject Importer", DefaultProcessor = "ContentObjectProcessor")]
public class ContentObjectImporter : ContentImporter<dynamic>
{
    public override dynamic Import(string filename, ContentImporterContext context)
    {
        try
        {
            ContentLogger.Enabled = true;
            var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    SerializationBinder = new KnownTypesBinder("Legends.Engine"),
                    MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
                };
            
            settings.Converters.Add(new AssetJsonConverter());
            settings.Converters.Add(new SizeFJsonConverter());
            settings.Converters.Add(new JsonConverters.SizeJsonConverter()); 
            settings.Converters.Add(new StringEnumConverter());    
                
            context.Logger.LogMessage("File Json:\n{0}", File.ReadAllText(filename));
            var result          = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(filename), settings);
            context.Logger.LogMessage("result.GetType().IsAssignableTo(typeof(IAsset)) = {0}", result.GetType().IsAssignableTo(typeof(Engine.IAsset)));

            
            var result2          = JsonConvert.DeserializeObject(File.ReadAllText(filename), result.GetType(), settings);

            string jsonOutput   = JsonConvert.ToString(JsonConvert.SerializeObject(result, settings));
            context.Logger.LogMessage("Import Json:\n{0}", jsonOutput.Substring(1, jsonOutput.Length - 2).Replace("\\\"", "\""));

            return result2;
        }
        catch(Exception error)
        {
            Console.WriteLine("Import Error: {0}\n{1}", error.Message, error.StackTrace);
            throw;
        }
    }
}

[ContentProcessor(DisplayName = "Legends ContentObject Processor")]
public class ContentObjectProcessor : ContentProcessor<dynamic, ContentObject>
{
    public override ContentObject Process(dynamic input, ContentProcessorContext context)
    {      
        try
        {  
            ContentLogger.Enabled = true;
            var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    SerializationBinder = new KnownTypesBinder("Legends.Engine"),
                    MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
                };

                
            settings.Converters.Add(new AssetJsonConverter());  
            settings.Converters.Add(new SizeFJsonConverter()); 
            settings.Converters.Add(new JsonConverters.SizeJsonConverter()); 
            settings.Converters.Add(new StringEnumConverter());  

            context.Logger.LogMessage("Process");
                    
            return ContentObject.Wrap((object)input);
        }
        catch(Exception error)
        {
            Console.WriteLine("Import Error: {0}\n{1}", error.Message, error.StackTrace);
            throw;
        }
    }
}
