using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Newtonsoft.Json;
using Legends.Content.Pipline.JsonConverters;
using Legends.Engine.Content;
using Newtonsoft.Json.Converters;
using Legends.Engine;

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
                    TypeNameHandling = TypeNameHandling.All,
                    SerializationBinder = new KnownTypesBinder("Legends.Engine"),
                    MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
                    Formatting = Formatting.Indented
                };
            
            settings.Converters.Add(new AssetJsonConverter());
            settings.Converters.Add(new SizeFJsonConverter());
            settings.Converters.Add(new PointJsonConverter());
            settings.Converters.Add(new JsonConverters.SizeJsonConverter()); 
            settings.Converters.Add(new JsonConverters.RectangleFJsonConverter()); 
            settings.Converters.Add(new JsonConverters.RectangleJsonConverter()); 
            settings.Converters.Add(new StringEnumConverter());    

            var jsonFileContents = File.ReadAllText(filename);
                
            //context.Logger.LogMessage("File Json:\n{0}\n", jsonFileContents);
            var result          = (dynamic)JsonConvert.DeserializeObject<dynamic>(jsonFileContents, settings);
            //context.Logger.LogMessage("result.GetType().IsAssignableTo(typeof(IAsset)) = {0}", result.GetType().IsAssignableTo(typeof(Engine.IAsset)));

            //string jsonOutput2   = JsonConvert.ToString(JsonConvert.SerializeObject(result, settings));

            //context.Logger.LogMessage("Pass 1 Json:\n{0}\n", jsonOutput2.Substring(1, jsonOutput2.Length - 2).Replace("\\\"", "\"").Replace("\\n", "\n"));

            
            var result2         = JsonConvert.DeserializeObject(jsonFileContents, result.GetType(), settings);
            string jsonOutput   = JsonConvert.ToString(JsonConvert.SerializeObject(result2, settings));

            context.Logger.LogMessage("Processed Json:\n{0}\n", jsonOutput.Substring(1, jsonOutput.Length - 2).Replace("\\\"", "\"").Replace("\\n", "\n"));

            return result2 as IAsset;

        }
        catch(Exception error)
        {
            Console.WriteLine("Import Error: {0}\n{1}", error.Message, error.StackTrace);
            throw;
        }
    }
}

[ContentProcessor(DisplayName = "Legends ContentObject Processor")]
public class ContentObjectProcessor : ContentProcessor<IAsset, ContentObject>
{
    public override ContentObject Process(IAsset input, ContentProcessorContext context)
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
            settings.Converters.Add(new PointJsonConverter());            
            settings.Converters.Add(new JsonConverters.SizeJsonConverter()); 
            settings.Converters.Add(new JsonConverters.RectangleFJsonConverter()); 
            settings.Converters.Add(new JsonConverters.RectangleJsonConverter()); 
            settings.Converters.Add(new StringEnumConverter());  

            //context.Logger.LogMessage("Process");

            //return input;      
            //throw new Exception("");
            return ContentObject.Wrap((object)input);
        }
        catch(Exception error)
        {
            Console.WriteLine("Import Error: {0}\n{1}", error.Message, error.StackTrace);
            throw;
        }
    }
}
