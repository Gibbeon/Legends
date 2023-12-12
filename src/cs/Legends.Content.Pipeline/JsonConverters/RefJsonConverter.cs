using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Legends.Engine.Content;
using Legends.Engine.Serialization;
using System.IO;
using Legends.Engine;

namespace Legends.Content.Pipline.JsonConverters;

public class RefJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType.IsAssignableTo(typeof(IRef));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object value, JsonSerializer serializer)
    {   
        try
        {
            if(reader.ValueType == typeof(string))
            {
                var result= Activator.CreateInstance(objectType, reader.Value);

                Console.WriteLine("Returning Ref {0}", result);
                return result;
            } 
            else 
            {
                var jObject = JObject.Load(reader);
                var refType = objectType.IsGenericType ? objectType.GetGenericArguments()[0] : objectType;
                var parsedValue = jObject.ToObject(refType);
                var result = Activator.CreateInstance(objectType, parsedValue);
                Console.WriteLine("Returning Ref {0}", result);
                return result;
            }
            
            /*var jObject = JObject.Load(reader);
            var parsedValue = jObject.ToObject(objectType);

            if(parsedValue is IScriptable scriptable)
            {
                if(!string.IsNullOrEmpty(scriptable.Source))
                {
                    var derivedType = DynamicClassLoader.Compile(scriptable.Source, File.ReadAllText(scriptable.Source)).GetType(scriptable.TypeName);
                    scriptable.Set(serializer.Deserialize(new StringReader(scriptable.Properties.ToString()), derivedType));
                }
                else
                {
                    scriptable.Set(serializer.Deserialize(new JTokenReader(jObject)));
                }
                return scriptable;
            }*/
            //else
            //{
            //    throw new JsonException();
           // }
        }
        catch(Exception error)
        {
            Console.WriteLine("AssetJsonConverter.ReadJson Error:" + error.Message);
            throw;
        }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {       
        try
        {
            //if(value is IScriptable scriptable)
            //{
            //    serializer.Serialize(writer, scriptable);
            //}
            //else 
            if(value is IRef reference)
            {
                writer.WriteValue(reference.Name);
            }
        }
        catch(Exception error)
        {
            Console.WriteLine("AssetJsonConverter.WriteJson Error:" + error.Message);
            throw;
        }
    }
}
