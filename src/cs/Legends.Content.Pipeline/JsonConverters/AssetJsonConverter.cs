using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Legends.Engine;
using Legends.Engine.Serialization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Legends.Content.Pipline.JsonConverters;


public class AssetJsonConverter : JsonConverter
{
    private static string WildCardToRegular(string value) {
        return "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$"; 
    }

    public bool Enabled { get; set; }

    public override bool CanConvert(Type objectType)
    {
        return objectType.IsAssignableTo(typeof(IAsset));
    }

    public override bool CanWrite => false;

    public override object ReadJson(JsonReader reader, Type objectType, object value, JsonSerializer serializer)
    {   
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
                            var jsonImportSource = JObject.Parse(File.ReadAllText(jsonSource.Value.ToString()));
            
                            jsonObject.Remove("$source");
                            
                            jsonImportSource.Merge(jsonObject,
                                new JsonMergeSettings() {
                                    MergeArrayHandling = MergeArrayHandling.Merge                            
                                }
                            );

                            jsonObject = jsonImportSource;
                            break;
                    }
                    
                    if(objectType.IsAssignableTo(typeof(IAsset)))
                    {
                        // IAsset Constuctor                        
                        var instanceObject = serializer.Deserialize(new StringReader(jsonObject.ToString()), instanceType);
                        return Activator.CreateInstance(objectType, assetName, instanceObject, true, true); 
                    }

                    // how to I make sure for this same object it's disabled
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
        try
        {
            serializer.Serialize(writer, value);    
        }
        catch(Exception error)
        {
            Console.WriteLine("AssetJsonConverter.WriteJson Error:" + error.Message);
            throw;
        }
    }
}
