
using System;
using Newtonsoft.Json;
using MonoGame.Extended;
using System.Linq;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;

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
            var jsonObject = JObject.Load(reader);

            var xVal   = jsonObject.Property("x");
            var yVal   = jsonObject.Property("y");
            var wVal   = jsonObject.Property("width");
            var hVal   = jsonObject.Property("height");
            var oVal   = jsonObject.Property("origin");
            var posVal = jsonObject.Property("position");
            var szVal  = jsonObject.Property("size");

            if(xVal != null) result.X           = xVal.Value.ToObject<int>();
            if(yVal != null) result.Y           = yVal.Value.ToObject<int>();
            if(wVal != null) result.Width       = wVal.Value.ToObject<int>();
            if(hVal != null) result.Height      = hVal.Value.ToObject<int>();
            if(posVal != null) result.Location  = posVal.Value.ToObject<Point>(serializer);
            if(szVal != null) result.Size       = szVal.Value.ToObject<Size>(serializer);
            if(oVal != null) result.Location    = Invert(oVal.Value.ToObject<Point>(serializer));
            
            return result;
        }
    }

    private static Point Invert(Point point)
    {
        return new Point(-point.X, -point.Y);
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Rectangle);
    }
}