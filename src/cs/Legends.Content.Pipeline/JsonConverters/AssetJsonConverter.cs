using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Legends.Engine.Content;
using Legends.Engine.Serialization;
using System.IO;
using System.Data;
using System.Dynamic;
using Legends.Engine.Runtime;
using System.Reflection;
using SharpDX.Direct3D;

namespace Legends.Content.Pipline.JsonConverters;

public class AssetJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType.IsAssignableTo(typeof(Asset));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object value, JsonSerializer serializer)
    {   
        try
        {
            if(reader.ValueType == typeof(string))
            {
                return Activator.CreateInstance(objectType, reader.Value, value);
            }
            
            value = JObject.Load(reader).ToObject(objectType);

            if(value is IScriptable scriptable)
            {
                var derivedType = DynamicClassLoader.CompileCodeAndExtractClass(scriptable.Source, File.ReadAllText(scriptable.Source), scriptable.TypeName);
                scriptable.Set(serializer.Deserialize(new StringReader(scriptable.Properties.ToString()), derivedType));
            }
        }
        catch(Exception error)
        {
            Console.WriteLine("AssetJsonConverter.ReadJson Error:" + error.Message);
            throw;
        }

        return value;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {       
        try
        {
            if(value is IScriptable scriptable)
            {
                serializer.Serialize(writer, scriptable);
            }
            else if(value is Asset asset)
            {
                writer.WriteValue(asset.Source);
            }
        }
        catch(Exception error)
        {
            Console.WriteLine("AssetJsonConverter.WriteJson Error:" + error.Message);
            throw;
        }
    }
}
