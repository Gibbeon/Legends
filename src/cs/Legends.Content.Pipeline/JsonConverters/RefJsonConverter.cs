using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Legends.Engine;
using Legends.Engine.Serialization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Legends.Content.Pipline.JsonConverters;

public class RefJsonConverter : JsonConverter
{
    private static string WildCardToRegular(string value) {
        return "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$"; 
    }

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
                var result = Activator.CreateInstance(objectType, Path.ChangeExtension(reader.Value.ToString(), null));
                return result;
            } 
            else 
            {
                var jObject = JObject.Load(reader);
                var valueType = objectType.IsGenericType ? objectType.GetGenericArguments()[0] : objectType;
                var jSource = jObject.Property("$compile");
                var jRef = jObject.Property("$ref");
                var jImport = jObject.Property("$import");
                var name = jRef == null ? "" : jRef.Value.ToString();
                bool isExternal = !string.IsNullOrEmpty(name);
                bool isExtended = false;
                
                if(jSource != null)
                {
                    var assembly    = DynamicClassLoader.Compile(jSource.Value.ToString(), File.ReadAllText(jSource.Value.ToString()));
                    var typeObject  = jObject.Property("$type");
                    var typeName    = typeObject == null ? string.Empty : typeObject.Value.ToString();
                    if(string.IsNullOrEmpty(typeName))
                    {
                        typeName = Path.GetFileNameWithoutExtension(jSource.Value.ToString());
                        valueType = assembly.Assembly.GetTypes().Single(n => Regex.IsMatch(n.FullName, WildCardToRegular("*." + typeName)));
                    }
                    else
                    {
                        valueType = assembly.Assembly.GetType(typeName);
                    }

                    name = Path.ChangeExtension(jObject.Property("$compile").Value.ToString(), null);
                    jObject.Remove("$compile");
                    jObject.Remove("$type");
                    isExternal = true;
                    isExtended = true;
                } 
                else if(jImport != null)
                {
                    var filename = jImport.Value.ToString();
                    name = Path.ChangeExtension(jObject.Property("$import").Value.ToString(), null);
                    var jImportValue = JObject.Parse(File.ReadAllText(filename));

    
                    jImportValue.Merge(jObject,
                        new JsonMergeSettings() {
                            MergeArrayHandling = MergeArrayHandling.Union
                        }
                    );

                    jObject = jImportValue;
                    jObject.Remove("$import");
                    
                    isExternal = false;
                    isExtended = true;
                }

                var parsedValue = serializer.Deserialize(new StringReader(jObject.ToString()), valueType);
                var result = Activator.CreateInstance(objectType, Path.ChangeExtension(name, null), parsedValue, isExternal, isExtended);
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
