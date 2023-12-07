using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Legends.Engine.Content;

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
            
            //Console.WriteLine("AssetJsonConverter.ReadJson {0}[{1}]='{2}'", 
            //    objectType.Name, 
            //    objectType.GetGenericArguments().SingleOrDefault()?.Name, 
            //    value);
            
            value = JObject.Load(reader).ToObject(objectType);

            //Console.WriteLine("AssetJsonConverter.ReadJson value='{0}'", value);
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
