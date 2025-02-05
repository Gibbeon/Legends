
using System;
using Newtonsoft.Json;
using MonoGame.Extended;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Legends.Content.Pipline.JsonConverters;

public class RectangleJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Rectangle rect = (Rectangle)value;
        writer.WriteValue($"{rect.X} {rect.Y} {rect.Width} {rect.Height}");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if(reader.TokenType == JsonToken.String)
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
            
            if (array.Length == 4)
            {
                return new Rectangle(array[0], array[1], array[2], array[3]);
            }
            throw new FormatException("Rectangle property value invalid - requires 4 parameters");
        }
        else
        {
            var result = new Rectangle();
            serializer.Populate(reader, result);
            return result;
        }
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Rectangle);
    }
}