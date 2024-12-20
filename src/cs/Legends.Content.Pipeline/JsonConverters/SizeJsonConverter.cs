
using System;
using Newtonsoft.Json;
using MonoGame.Extended;
using System.Linq;

namespace Legends.Content.Pipline.JsonConverters;

public class SizeJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Size size = (Size)value;
        writer.WriteValue($"{size.Width} {size.Height}");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        string src = reader.Value.ToString();

        Console.WriteLine(src);

        int[] array = 
            src
            .Split(',', ' ','\n')
            .Select(n => n.Trim())
            .Where(n => !string.IsNullOrEmpty(n))
            .Select(n => int.Parse(n))
            .ToArray();
        
        if (array.Length == 2)
        {
            return new Size(array[0], array[1]);
        }

        if (array.Length == 1)
        {
            return new Size(array[0], array[0]);
        }

        throw new FormatException("Invalid Size property value");
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Size);
    }
}