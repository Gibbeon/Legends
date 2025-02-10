
using System;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Legends.Content.Pipline.JsonConverters;

public class PointJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Point size = (Point)value;
        writer.WriteValue($"{size.X} {size.Y}");
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
            return new Point(array[0], array[1]);
        }

        throw new FormatException("Invalid Point property value");
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Point);
    }
}