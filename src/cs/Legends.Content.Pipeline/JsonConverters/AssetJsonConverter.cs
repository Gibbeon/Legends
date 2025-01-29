using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Legends.Engine;
using Legends.Engine.Serialization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Legends.Content.Pipline.JsonConverters;

//PropertyRenameAndIgnoreSerializerContractResolver 

public class AssetJsonConverter : JsonConverter
{
    private static string WildCardToRegular(string value) {
        return "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$"; 
    }
    public override bool CanConvert(Type objectType)
    {
        Console.WriteLine("CanConvert: {0} = {1}", objectType.Name, objectType.IsAssignableTo(typeof(IAsset)));
        return objectType.IsAssignableTo(typeof(IAsset));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object value, JsonSerializer serializer)
    {   
        Console.WriteLine("ReadJson: (objectType: {0},  value: {1})", objectType.Name, value);

        try
        {      
            if(reader.TokenType == JsonToken.StartObject) 
            {
                var jsonObject = JObject.Load(reader);

                var jsonSource = jsonObject.Property("$source");
                var jsonType   = jsonObject.Property("$type");

                Type instanceType = objectType;
                
                if(jsonSource != null)
                {       
                    //throw new Exception(string.Format("ReadJson\n{0}", jsonObject.ToString()));

                    var assetName = Path.ChangeExtension(jsonSource.Value.ToString(), null);

                    switch(Path.GetExtension(jsonSource.Value.ToString()).ToLower())
                    {
                        case ".cs":
                            var dynamicAssembly = DynamicClassLoader.Compile(jsonSource.Value.ToString(), File.ReadAllText(jsonSource.Value.ToString()));
                            

                            instanceType = (jsonType != null) ? 
                                dynamicAssembly.Assembly.GetType(jsonType.Value.ToString()) : 
                                dynamicAssembly.Assembly.GetTypes().Single(n => Regex.IsMatch(n.FullName, WildCardToRegular("*." + Path.GetFileNameWithoutExtension(jsonSource.Value.ToString()))));
                                 
                            jsonObject.Remove("$source");
                            jsonObject.Remove("$type");
                            break;
                            
                        case ".json":
                        case ".jsonx":
                            var jsonImportSource = JObject.Parse(File.ReadAllText(jsonSource.Value.ToString()));
            
                            jsonObject.Remove("$source");
                            
                            jsonImportSource.Merge(jsonObject,
                                new JsonMergeSettings() {
                                    MergeArrayHandling = MergeArrayHandling.Merge                            
                                }
                            );

                            jsonObject = jsonImportSource;                    
                            
                            break;
                         default:
                            throw new InvalidDataException(string.Format("$source attribute defined but no handler for extension: {0}", Path.GetExtension(jsonSource.Value.ToString()).ToLower()));
                    }
                    
                        // IAsset Constuctor                        
                    //var instanceObject = serializer.Deserialize(new StringReader(jsonObject.ToString()), instanceType);
                    //return Activator.CreateInstance(objectType, assetName, instanceObject, true, true); 

                    //throw new InvalidDataException();
                    // how to I make sure for this same object it's disabled

                    throw new Exception(jsonObject.ToString());

                    return serializer.Deserialize(new StringReader(jsonObject.ToString()), objectType);
                }
            }

            return serializer.Deserialize(reader, objectType);
        }
        catch(Exception error)
        {
            Console.WriteLine("ObjectJsonConverter.ReadJson Error:" + error.Message);
            throw;
        }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {       
        Console.WriteLine("WriteJson: (value: {0})", value);
        try
        {
            //base.WriteJson(writer, value, serializer);
            //serializer.Serialize(writer, value);    
        }
        catch(Exception error)
        {
            Console.WriteLine("AssetJsonConverter.WriteJson Error:" + error.Message);
            throw;
        }
    }
}
