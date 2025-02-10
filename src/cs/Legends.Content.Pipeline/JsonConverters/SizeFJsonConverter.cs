
using System;
using Newtonsoft.Json;
using MonoGame.Extended;
using System.Linq;

namespace Legends.Content.Pipline.JsonConverters;

public class SizeFJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        SizeF size = (SizeF)value;
        writer.WriteValue($"{size.Width} {size.Height}");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        string src = reader.Value.ToString();

        Console.WriteLine(src);

        float[] array = 
            src
            .Split(',', ' ','\n')
            .Select(n => n.Trim())
            .Where(n => !string.IsNullOrEmpty(n))
            .Select(n => float.Parse(n))
            .ToArray();
        
        if (array.Length == 2)
        {
            return new SizeF(array[0], array[1]);
        }

        if (array.Length == 1)
        {
            return new SizeF(array[0], array[0]);
        }

        throw new FormatException("Invalid Size property value");
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(SizeF);
    }
}