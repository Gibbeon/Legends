using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Newtonsoft.Json;
using Legends.Engine;
using Legends.Engine.Graphics2D;
using Newtonsoft.Json.Linq;
using Legends.Engine.Runtime;

namespace Legends.Content.Pipline.JsonConverters;

public class AssetJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType.IsAssignableTo(typeof(Asset));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object value, JsonSerializer serializer)
    {   
        Console.WriteLine("AssetJsonConverter.ReadJson {0}", objectType.GetGenericArguments()[0].FullName);
        

        if(reader.ValueType == typeof(string))
        {
            return Activator.CreateInstance(objectType, reader.Value);
        }
        else
        {
            reader.SupportMultipleContent = true;

            JObject parsedValue = JObject.Load(reader);
            Console.WriteLine(parsedValue.ToString());
            
            Console.WriteLine(parsedValue.ToObject<Scriptable>());
            return parsedValue.ToObject<Scriptable>();
        }

        throw new NotSupportedException();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {       
        Console.WriteLine("AssetJsonConverter.WriteJson {0}", value.GetType());

        if(value is Scriptable scriptable)
        {
            writer.WriteStartObject();        
            writer.WritePropertyName("Source");    
            writer.WriteValue(scriptable.Source); 
            writer.WritePropertyName("TypeName");           
            writer.WriteValue(scriptable.TypeName);
            writer.WriteEndObject();
        }
        else if(value is Asset asset)
        {
            writer.WriteValue(asset.Source);
        }
    }
}
