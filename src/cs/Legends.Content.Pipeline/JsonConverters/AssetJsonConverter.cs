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
       try {
            //Console.WriteLine("CanConvert {0} is IAsset, _skip = {1}", objectType, _skip);
            return !_skip && objectType.IsAssignableTo(typeof(IAsset));
       }
       finally {
            _skip = false;
       }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object value, JsonSerializer serializer)
    {   _skip = false;
        Console.WriteLine("ReadJson: (objectType: {0},  value: {1}) with token {2}", objectType.Name, value, reader.TokenType);

        try
        {   
            if(reader.TokenType == JsonToken.String)
            {
                var readValue = reader.Value.ToString();
                Console.WriteLine("reading static reference to {0}", readValue);
                // IAsset where the token is a string;
                return objectType.Create(null, readValue) as IAsset;
            }

            var jsonObject = JObject.Load(reader);

            var jsonSource = jsonObject.Property("$source");
            var jsonType   = jsonObject.Property("$type");

            if(jsonType != null) 
            {
                objectType = serializer.SerializationBinder.BindToType(null, jsonType.Value.ToString());
                if(objectType == null) throw new InvalidDataException(string.Format("{0} was not found.", jsonType));
            }
            
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
                        //objectType = serializer.SerializationBinder.BindToType(null, jsonType.Value.ToString()) ?? objectType;                  
                        
                        break;
                        default:
                        throw new InvalidDataException(string.Format("$source attribute defined but no handler for extension: {0}", Path.GetExtension(jsonSource.Value.ToString()).ToLower()));
                }
            }
            //Console.WriteLine("Trying to create {0} and token was {1}.", objectType, jsonType.Value.ToString());

            var objectInstance = objectType.Create() as IAsset;
            serializer.Populate(new StringReader(jsonObject.ToString()), objectInstance);
                
            return objectInstance;
        }
        catch(Exception error)
        {
            Console.WriteLine("AssetJsonConverter.ReadJson Error:" + error.Message);
            throw;
        }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {       
        Console.WriteLine("WriteJson: (value: {0}) writestate: {1}", value, writer.WriteState);
        try
        {
            var assetValue = value as IAsset;
                       
            if(!string.IsNullOrEmpty(assetValue.Name))
            {
                Console.WriteLine("writing reference {0} of type {1}", assetValue.Name, value.GetType());
                writer.WriteValue(assetValue.Name);
            }
            else
            {
                Console.WriteLine("writing inline {0} of type {1}", assetValue, value.GetType());
                _skip = true; // HACK see:https://stackoverflow.com/questions/26129448/json-net-how-to-customize-serialization-to-insert-a-json-property
                serializer.Serialize(writer, assetValue, value.GetType());
                _skip = false;
            }
        }
        catch(Exception error)
        {
            Console.WriteLine("AssetJsonConverter.WriteJson Error:" + error.Message);
            throw;
        }
    }
}
