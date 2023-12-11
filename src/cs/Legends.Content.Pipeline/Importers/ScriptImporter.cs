using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using System.Reflection;
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
