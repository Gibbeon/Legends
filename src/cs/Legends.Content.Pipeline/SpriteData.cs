using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Framework; 
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Legends.Content.Pipline;

public class SpriteData
{
    public string Name;
    public SpatialData Spatial;
}

public class SpatialData
{
    public Vector2 Position;
    public Vector2 Scale;
    public float Rotation;
}

[ContentImporter(".sprite", DisplayName = "SpriteDataImporter", DefaultProcessor = "SpriteDataProcessor")]
public class SpriteDataImporter : ContentImporter<SpriteData>
{
    public override SpriteData Import(string filename, ContentImporterContext context)
    {
        context.Logger.LogMessage("Importing file: {0}", filename);

        //using(var reader = new JsonTextReader(File.OpenText(filename)))
        //{
        //    JsonSerializer serializer = new JsonSerializer();
        //    return serializer.Deserialize<SpriteData>(reader);        
        //}

        return JsonConvert.DeserializeObject<SpriteData>(File.ReadAllText(filename));
    }
}

[ContentProcessor(DisplayName = "SpriteDataProcessor")]
class SpriteDataProcessor : ContentProcessor<SpriteData, SpriteData>
{
    public override SpriteData Process(SpriteData input, ContentProcessorContext context)
    {
        return input;
    }
}

public class SpriteDataReader : ContentTypeReader<SpriteData>
{
    protected override SpriteData Read(ContentReader input, SpriteData existingInstance)
    {
        return new SpriteData() { 
            Name = input.ReadString(), 
            Spatial = new SpatialData() { Position = input.ReadVector2(), Scale = input.ReadVector2(), Rotation = input.ReadSingle() } 
        };
    }
}

[ContentTypeWriter]
public class SpriteDataWriter : ContentTypeWriter<SpriteData>
{
    protected override void Write(ContentWriter output, SpriteData value)
    {  
        output.Write(value.Name);        
        output.Write(value.Spatial.Position);
        output.Write(value.Spatial.Scale);        
        output.Write(value.Spatial.Rotation);
    }

    public override string GetRuntimeType(TargetPlatform targetPlatform)
    {
        return typeof(SpriteData).AssemblyQualifiedName;
    }

    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return typeof(SpriteDataReader).AssemblyQualifiedName;
    }
}


