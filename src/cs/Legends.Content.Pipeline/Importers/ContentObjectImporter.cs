using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Newtonsoft.Json;
using Legends.Content.Pipline.JsonConverters;
using Legends.Engine.Content;

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
                };
            
            settings.Converters.Add(new RefJsonConverter());
                
            var result = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(filename), settings);
            context.Logger.LogMessage("{0}", JsonConvert.ToString(JsonConvert.SerializeObject(result)));
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
                };
                
            settings.Converters.Add(new RefJsonConverter());     
            //context.BuildAssetDependencies((object)input, ((object)input).GetType());
                    
            return ContentObject.Wrap((object)input);
        }
        catch(Exception error)
        {
            Console.WriteLine("Import Error: {0}\n{1}", error.Message, error.StackTrace);
            throw;
        }
    }
}
