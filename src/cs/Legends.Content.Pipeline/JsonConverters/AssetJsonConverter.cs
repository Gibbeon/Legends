using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Legends.Engine;
using Legends.Engine.Serialization;
using System.Linq;
using System.Text.RegularExpressions;
using Legends.Engine.Runtime;
using Legends.Engine.Resolvers;

namespace Legends.Content.Pipline.JsonConverters;



public class AssetJsonConverter : JsonConverter
{
    private bool _skip;
    private static string WildCardToRegular(string value) {
        return "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$"; 
    }
    public override bool CanConvert(Type objectType)
    {
        Console.WriteLine("CanConvert: {0} = {1}", objectType.Name, objectType.IsAssignableTo(typeof(IAsset)));
        bool result = !_skip && objectType.IsAssignableTo(typeof(IAsset));
        _skip = false;
        return result;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object value, JsonSerializer serializer)
    {   
        Console.WriteLine("ReadJson: (objectType: {0},  value: {1})", objectType.Name, value);

        try
        {   
            if(reader.TokenType == JsonToken.String)
            {
                // IAsset where the token is a string
                return objectType.Create(AssetType.Static, value.ToString()) as IAsset;
            }

            if(reader.TokenType == JsonToken.StartObject) 
            {
                var jsonObject = JObject.Load(reader);

                var jsonSource = jsonObject.Property("$source");
                var jsonType   = jsonObject.Property("$type");
                
                if(jsonSource != null)
                {       
                    var assetName = Path.ChangeExtension(jsonSource.Value.ToString(), null);

                    switch(Path.GetExtension(jsonSource.Value.ToString()).ToLower())
                    {
                        case ".cs":
                            var dynamicAssembly = DynamicClassLoader.Compile(jsonSource.Value.ToString(), File.ReadAllText(jsonSource.Value.ToString()));

                            objectType = (jsonType != null) ? 
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

                    var objectInstance = objectType.Create() as IAsset;
                    serializer.Populate(new StringReader(jsonObject.ToString()), objectInstance);
                    return objectInstance;
                }
            }

            throw new InvalidDataException("Json value must be either a string or an object type");
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
            var assetValue = value as IAsset;
            writer.WriteValue(assetValue.AssetType);

            switch(assetValue.AssetType)
            {
                case AssetType.Static:
                    writer.WriteValue(assetValue.AssetName);
                    break;
                case AssetType.Dynamic:
                // this gets the converter for the object & calls write json
                // and the converter can write
                // and then it calls it again
                // etc.

                // for each property do I need to serilaize it?
                // 
                    _skip = true; // HACK
                    serializer.Serialize(writer, assetValue, value.GetType()); 
                    break;
                default:
                    throw new InvalidOperationException("AssetType is invalid.");
            }
        }
        catch(Exception error)
        {
            Console.WriteLine("AssetJsonConverter.WriteJson Error:" + error.Message);
            throw;
        }
    }
}
