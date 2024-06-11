
using System;
using Newtonsoft.Json;
using MonoGame.Extended;
using MonoGame.Extended.Serialization;

namespace Legends.Content.Pipline.JsonConverters;

/*
public class SizeJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Size size = (Size)value;
        writer.WriteValue($"{size.Width} {size.Height}");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        int[] array = reader.ReadAsMultiDimensional<int>();
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
        return objectType == typeof(Size2);
    }
}*/