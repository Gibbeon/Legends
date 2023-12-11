using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Newtonsoft.Json;
using Legends.Content.Pipline.JsonConverters;
using Legends.Content.Pipeline;
using Legends.Engine.Content;

namespace Legends.Content.Pipline;

[ContentImporter(".json", DisplayName = "Legends ContentObject Importer", DefaultProcessor = "ContentObjectProcessor")]
public class ContentObjectImporter : ContentImporter<dynamic>
{
    public override dynamic Import(string filename, ContentImporterContext context)
    {
        var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
            };
        
        settings.Converters.Add(new AssetJsonConverter());
            
        return JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(filename), settings);
    }
}

[ContentProcessor(DisplayName = "Legends ContentObject Processor")]
public class ContentObjectProcessor : ContentProcessor<dynamic, ContentObject>
{
    public override ContentObject Process(dynamic input, ContentProcessorContext context)
    {
        foreach(var item in context.Parameters)
        {
            Console.Write("{0}={1}", item.Key, item.Value);
        }
        
        var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
            };
            
        settings.Converters.Add(new AssetJsonConverter());     
        //context.BuildAssetDependencies((object)input, ((object)input).GetType());
                
        return ContentObject.Wrap((object)input);
    }
}
