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
                var result = Activator.CreateInstance(objectType, reader.Value);
                return result;
            } 
            else 
            {
                var jObject = JObject.Load(reader);
                var valueType = objectType.IsGenericType ? objectType.GetGenericArguments()[0] : objectType;
                var jSource = jObject.Property("$compile");
                var jRef = jObject.Property("$ref");
                var JTemplate = jObject.Property("$template");
                var name = jRef == null ? "" : jRef.Value.ToString();
                bool isExternal = !string.IsNullOrEmpty(name);
                bool isExtended = false;
                
                if(jSource != null)
                {
                    var assembly = DynamicClassLoader.Compile(jSource.Value.ToString(), File.ReadAllText(jSource.Value.ToString()));
                    valueType = assembly.Assembly.GetType(jObject.Property("$type").Value.ToString());
                    name = Path.ChangeExtension(jObject.Property("$compile").Value.ToString(), null);
                    jObject.Remove("$compile");
                    jObject.Remove("$type");
                    isExternal = true;
                    isExtended = true;
                } 
                else if(JTemplate != null)
                {
                    var filename = JTemplate.Value.ToString();
                    name = Path.ChangeExtension(jObject.Property("$template").Value.ToString(), null);
                    var jTemplate = JObject.Parse(File.ReadAllText(filename));

                    jObject.Merge(jTemplate,
                        new JsonMergeSettings() {
                            MergeArrayHandling = MergeArrayHandling.Union
                        }
                    );

                    jObject.Remove("$template");
                    
                    isExternal = true;
                    isExtended = true;
                }

                var parsedValue = serializer.Deserialize(new StringReader(jObject.ToString()), valueType);
                var result = Activator.CreateInstance(objectType, name, parsedValue, isExternal, isExtended);
                return result;
            }
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
            if(value is IRef refValue)
            {
                if(refValue.IsExternal && !refValue.IsExtended)
                {
                    writer.WriteValue(refValue.Name);               
                }
                else
                {
                    //writer.WriteStartObject();
                   // writer.WritePropertyName("Name"); writer.WriteValue(refValue.Name);
                    //writer.WritePropertyName("RefType"); writer.WriteValue(refValue.RefType.FullName);
                    //writer.WritePropertyName("IsExternal"); writer.WriteValue(refValue.IsExternal);
                    //writer.WritePropertyName("IsExtended"); writer.WriteValue(refValue.IsExtended);
                    
                    //writer.WriteEndObject();
                    serializer.Serialize(writer, refValue.Get());
                }
            }
        }
        catch(Exception error)
        {
            Console.WriteLine("AssetJsonConverter.WriteJson Error:" + error.Message);
            throw;
        }
    }
}
