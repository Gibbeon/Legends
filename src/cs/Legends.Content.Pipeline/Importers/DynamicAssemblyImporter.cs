using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Legends.Engine.Serialization;
using System;

namespace Legends.Content.Pipline;


[ContentImporter(".cs", DisplayName = "Legends Script Importer", DefaultProcessor = "DynamicAssemblyProcessor")]
public class DynamicAssemblyImporter : ContentImporter<DynamicAssembly>
{
    public override DynamicAssembly Import(string filename, ContentImporterContext context)
    {
        Console.WriteLine("Filename {0}", filename);

        return DynamicClassLoader.Compile(filename, File.ReadAllText(filename));
    }
}

[ContentProcessor(DisplayName = "Legends Script Processor")]
public class DynamicAssemblyProcessor : ContentProcessor<DynamicAssembly, DynamicAssembly>
{
    public override DynamicAssembly Process(DynamicAssembly input, ContentProcessorContext context)
    {
        return (DynamicAssembly)input;
    }
}
