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
                    SerializationBinder = new KnownTypesBinder("Legends.Engine")
                };
            
            settings.Converters.Add(new RefJsonConverter());
            settings.Converters.Add(new SizeFJsonConverter());
            settings.Converters.Add(new JsonConverters.SizeJsonConverter()); 
            settings.Converters.Add(new StringEnumConverter());    
                
            var result = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(filename), settings);
            string jsonOutput = JsonConvert.ToString(JsonConvert.SerializeObject(result, settings));

            context.Logger.LogMessage("{0}", jsonOutput.Substring(1, jsonOutput.Length - 2).Replace("\\\"", "\""));
            return result;
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
                    SerializationBinder = new KnownTypesBinder("Legends.Engine")
                };

                
            settings.Converters.Add(new RefJsonConverter());  
            settings.Converters.Add(new SizeFJsonConverter()); 
            settings.Converters.Add(new JsonConverters.SizeJsonConverter()); 
            settings.Converters.Add(new StringEnumConverter());  
                    
            return ContentObject.Wrap((object)input);
        }
        catch(Exception error)
        {
            Console.WriteLine("Import Error: {0}\n{1}", error.Message, error.StackTrace);
            throw;
        }
    }
}
