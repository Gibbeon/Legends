
using System;
using Newtonsoft.Json;
using MonoGame.Extended;
using System.Linq;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Xml.Schema;

namespace Legends.Content.Pipline.JsonConverters;

public class RectangleFJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        RectangleF rect = (RectangleF)value;
        writer.WriteValue($"{rect.X} {rect.Y} {rect.Width} {rect.Height}");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if(reader.TokenType == JsonToken.String)
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
            
            if (array.Length == 4)
            {
                return new RectangleF(array[0], array[1], array[2], array[3]);
            }
            throw new FormatException("RectangleF property value invalid - requires 4 parameters");
        }
        else
        {
            var result = new RectangleF();
            var jsonObject = JObject.Load(reader);

            var xVal   = jsonObject.Property("x");
            var yVal   = jsonObject.Property("y");
            var wVal   = jsonObject.Property("width");
            var hVal   = jsonObject.Property("height");
            var posVal = jsonObject.Property("position");
            var oVal   = jsonObject.Property("origin");
            var szVal  = jsonObject.Property("size");

            if(xVal != null) result.X           = xVal.Value.ToObject<float>();
            if(yVal != null) result.Y           = yVal.Value.ToObject<float>();
            if(wVal != null) result.Width       = wVal.Value.ToObject<float>();
            if(hVal != null) result.Height      = hVal.Value.ToObject<float>();
            if(posVal != null) result.Position  = posVal.Value.ToObject<Vector2>(serializer);
            if(szVal != null) result.Size       = szVal.Value.ToObject<SizeF>(serializer);
            if(oVal != null) result.Position    = -oVal.Value.ToObject<Vector2>(serializer);
            
            return result;
        }
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(RectangleF);
    }
}