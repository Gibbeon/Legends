using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Newtonsoft.Json;
using Legends.Engine;
using Legends.Engine.Graphics2D;

namespace Legends.Content.Pipline.JsonConverters;

public class AssetJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType.IsAssignableTo(typeof(Asset));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object value, JsonSerializer serializer)
    {
        return Activator.CreateInstance(objectType, reader.Value);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if(value is Asset assetValue)
        {
            writer.WriteValue(assetValue.Name);
        }
        else
        {
            throw new NotSupportedException();
        }
    }
}
