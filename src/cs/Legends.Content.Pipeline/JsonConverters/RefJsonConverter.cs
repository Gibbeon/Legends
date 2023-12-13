using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Legends.Engine;
using Legends.Engine.Serialization;

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
                return result;
            } 
            else 
            {
                var jObject = JObject.Load(reader);
                var valueType = objectType.IsGenericType ? objectType.GetGenericArguments()[0] : objectType;
                var jProperty = jObject.Property("$src");
                var name = (string)"";
                bool isExternal = false;
                bool isExtended = false;
                
                if(jProperty != null)
                {
                    var assembly = DynamicClassLoader.Compile(jProperty.Value.ToString(), File.ReadAllText(jProperty.Value.ToString()));
                    valueType = assembly.Assembly.GetType(jObject.Property("$type").Value.ToString());
                    name = Path.ChangeExtension(jObject.Property("$src").Value.ToString(), null);
                    jObject.Remove("$src");
                    jObject.Remove("$type");
                    isExternal = true;
                    isExtended = true;
                }

                //Console.WriteLine("isExternal {0} for {1}", isExternal, valueType.Name);

                var parsedValue = serializer.Deserialize(new StringReader(jObject.ToString()), valueType);
                var result = Activator.CreateInstance(objectType, name, parsedValue, isExternal, isExtended);
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
