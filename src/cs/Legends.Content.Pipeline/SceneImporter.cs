using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Newtonsoft.Json;
using Legends.Engine;
using Legends.Content.Pipline.JsonConverters;
using Legends.Engine.Serialization;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Legends.Engine.Graphics2D;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace Legends.Content.Pipline;

public static class StdStuff
{
    public static string Code = @"
        using Microsoft.Xna.Framework.Content.Pipeline;
        using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
        using Legends.Engine;
        using Legends.Engine.Graphics2D;
        using Legends.Engine.Serialization;
        using {1};

        namespace Legends.Content.Pipline;
    
        [ContentTypeWriter]
        public class {0}Writer : ContentTypeWriter<{0}> {{
            protected override void Write(ContentWriter output, {0} value)         {{ this.GenericWriteObject(output, value); }}
            public override string GetRuntimeType(TargetPlatform targetPlatform)   {{ return GetType().BaseType.GenericTypeArguments[0].AssemblyQualifiedName; }}
            public override string GetRuntimeReader(TargetPlatform targetPlatform) {{ return typeof(GenericReader<>).MakeGenericType(GetType().BaseType.GenericTypeArguments[0]).AssemblyQualifiedName; }}
        }}";
}

[ContentImporter(".json", DisplayName = "Legends Scene Importer", DefaultProcessor = "SceneProcessor")]
public class SceneImporter : ContentImporter<Scene>
{
    public IEnumerable<Type> GetTypesForInstance(object? instance, IList<Type>? result = default)
    {
        result ??= new List<Type>();

        if(instance == null) return result;

        //typeof(ContentWriter).Write params + extensions instead of hardcoded list

        Console.WriteLine("{0}", instance.GetType());

        if(result.Contains(instance.GetType()))     return result;
        if(instance.GetType() == typeof(string))    return result;
        if(instance.GetType().Namespace.StartsWith("Newtonsoft.Json"))      return result;
        if(instance.GetType() == typeof(object))    return result;
        if(instance.GetType() == typeof(Vector2))   return result;
        if(instance.GetType() == typeof(Vector3))   return result;
        if(instance.GetType() == typeof(Size2))     return result;
        if(instance.GetType() == typeof(Point2))    return result;
        if(instance.GetType() == typeof(RectangleF)) return result;
        if(instance.GetType() == typeof(Color))     return result;
        if(instance.GetType() == typeof(Matrix))    return result;
        if(instance.GetType() == typeof(Asset))     return result;
        if(instance.GetType().IsGenericType && instance.GetType().GetGenericTypeDefinition() == typeof(Asset<>)) return result;
        if(instance.GetType().IsPrimitive)          return result;
        if(instance.GetType().IsEnum)               return result;
        
        if(instance is IDynamicallyCompiledType script)
        {
            var type = DynamicClassLoader.CompileCodeAndExtractClass(script.Source, File.ReadAllText(script.Source), script.TypeName);
            //foreach(var type in assembly.GetTypes())
            {
                Console.WriteLine("Dynamicly compiled type {0} from {1}", type.Name, script.Source);
                GetTypesForInstance(Activator.CreateInstance(type), result);
            }
            return result;
            //dynamicly compiple ScriptBehavior
        }
        
        if(instance.GetType().IsArray)            
        {
            Console.WriteLine("IsArray {0}", instance.GetType().Name);
            Array value = (Array)instance;
            foreach(var item in value)
            {
                GetTypesForInstance(item, result);
            }
            return result;
        }

        if(instance is IEnumerable enumerable)
        {
            Console.WriteLine("IEnumerable {0}", instance.GetType().Name);
            foreach(var item in enumerable)
            {
                GetTypesForInstance(item, result);
            }
        }

        foreach(var field in instance.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
        {
            GetTypesForInstance(field.GetValue(instance), result);
        }

        foreach(var property in instance.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
        {
            if(property.GetIndexParameters().Length > 0) continue;
            if(property.IsDefined(typeof(JsonIgnoreAttribute), true)) continue;

            GetTypesForInstance(property.GetValue(instance), result);
        }
        
        if(!(instance.GetType().IsGenericType && instance.GetType().GetGenericTypeDefinition() == typeof(List<>)))
        { 
            result.Add(instance.GetType());
        }

        return result;
    }

    public override Scene Import(string filename, ContentImporterContext context)
    {
        context.Logger.LogMessage("Importing file: {0}", filename);
        try
        {            
            //DynamicClassLoader.Compile("Scene",                 string.Format(StdStuff.Code, "Scene"));       
            //DynamicClassLoader.Compile("SceneObject",           string.Format(StdStuff.Code, "SceneObject"));       
            //DynamicClassLoader.Compile("Camera",                string.Format(StdStuff.Code, "Camera"));
            //DynamicClassLoader.Compile("TextRenderBehavior",    string.Format(StdStuff.Code, "TextRenderBehavior"));
            
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
            };

            settings.Converters.Add(new AssetJsonConverter());
            
            var result = JsonConvert.DeserializeObject<Scene>(File.ReadAllText(filename), settings);

            foreach(var item in GetTypesForInstance(result))
            {
                Console.WriteLine("Dynamic support for type {0}, {1}", item.Name, item.Namespace);
                DynamicClassLoader.Compile(item.Name, string.Format(StdStuff.Code, item.Name, item.Namespace));
            }

            context.Logger.LogMessage(JsonConvert.ToString(JsonConvert.SerializeObject(result, settings)).Replace("{", "{{").Replace("}", "}}"));

            return result ?? new Scene(null);
        } 
        catch(Exception err)
        {
            context.Logger.LogImportantMessage(err.Message);
            throw;
        }
    }
}

[ContentProcessor(DisplayName = "Legends Scene Processor")]
class SceneProcessor : ContentProcessor<Scene, Scene>
{
    public override Scene Process(Scene input, ContentProcessorContext context)
    {
        return input;
    }
}