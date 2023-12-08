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
using Legends.Engine.Serialization;

namespace Legends.Content.Pipline;



[ContentImporter(".cs", DisplayName = "Legends Script Importer", DefaultProcessor = "ScriptProcessor")]
public class ScriptImporter : ContentImporter<Assembly>
{
    public override Assembly Import(string filename, ContentImporterContext context)
    {
        
        return DynamicClassLoader.Compile(filename, File.ReadAllText(filename));
    }
}

[ContentProcessor(DisplayName = "Legends Script Processor")]
public class ScriptProcessor : ContentProcessor<Assembly, Assembly>
{
    public override Assembly Process(Assembly input, ContentProcessorContext context)
    {
        return (Assembly)input;
    }
}
