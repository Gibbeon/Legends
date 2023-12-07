using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Newtonsoft.Json;
using Legends.Content.Pipline.JsonConverters;
using Legends.Engine.Content;
using System.Reflection;
using System.Linq;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using Legends.Engine.Runtime;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Legends.Content.Pipline;



[ContentImporter(".cs", DisplayName = "Legends Script Importer", DefaultProcessor = "Script Processor")]
public class ScriptImporter : ContentImporter<dynamic>
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

[ContentProcessor(DisplayName = "Legends Asset Processor")]
public class ScriptProcessor : ContentProcessor<dynamic, object>
{
    public override object Process(dynamic input, ContentProcessorContext context)
    {
        var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

        settings.Converters.Add(new AssetJsonConverter());

        context.BuildAssetDependencies((object)input, ((object)input).GetType());

        return (object)input;
    }
}
