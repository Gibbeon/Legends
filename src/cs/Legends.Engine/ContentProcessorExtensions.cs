using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using Newtonsoft.Json;
using Legends.Engine.Content;
using System.Reflection;
using System.Linq;
using System.Data;
using System.Collections;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json.Linq;

namespace Legends.Content.Pipline;

public static class ContentProcessorExtensions
{
    private static readonly Type[] ExclusionTypes =
    {
        typeof(string),
        typeof(bool),
        typeof(Vector2),
        typeof(Vector3),
        typeof(Vector4),
        typeof(Size2),
        typeof(Point2),
        typeof(RectangleF),
        typeof(Color),
        typeof(Matrix),
        typeof(JObject),
        typeof(JProperty),
        typeof(JArray),
        typeof(Type)
    };
    
    private static void BuildAsset(this ContentProcessorContext processor, Asset asset)
    {
        try
        {
            if(string.IsNullOrEmpty(asset.Source)) return;

            Console.WriteLine("Build Aseet: {0}", asset.Source);

            var mi = processor.GetType()
                    .GetMethods()
                    .Single(n =>   n.Name == "BuildAsset" 
                                    && n.IsGenericMethod
                                    && n.GetParameters().Length == 2)
                    .MakeGenericMethod(asset.AssetType, asset.AssetType);
            
            var result = mi.Invoke(processor, new [] { asset.AsExternalReference(), null });

            Console.WriteLine("ContentItem {0}", result.GetType().GetProperty("Filename").GetValue(result));
        } catch(Exception err)
        {
            Console.WriteLine(err.Message);
        }
    }

    private static void ProcessBuildAssetMemberValue(this ContentProcessorContext processor, object instance, Type type)
    { 
        var derivedType = instance == null ? type : instance.GetType();

        if(instance == null 
                || derivedType == typeof(object)
                || ExclusionTypes.Any(n => n.IsAssignableFrom(derivedType))
                || instance.GetType().IsPrimitive
                || instance.GetType().IsEnum
                || instance.GetType().IsValueType) return;

        if(instance is Asset asset) processor.BuildAsset(asset);
        else if(instance is IEnumerable enumerable) foreach(var item in enumerable) processor.BuildAssetDependencies(item, item.GetType());
        else processor.BuildAssetDependencies(instance, type);
    }

    public static void BuildAssetDependencies(this ContentProcessorContext processor, object instance, Type type)
    {
        try
        {
            var derivedType = instance == null ? type : instance.GetType();

            //Console.WriteLine("{0}", derivedType.Name);

            if(instance == null 
                || derivedType == typeof(object)
                || ExclusionTypes.Any(n => n.IsAssignableFrom(derivedType))
                || instance.GetType().IsPrimitive
                || instance.GetType().IsEnum
                || instance.GetType().IsValueType) return;

            if(instance is Asset asset) { processor.BuildAsset(asset); }            

            foreach(var member in Enumerable.Concat<MemberInfo>(
                derivedType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty),
                derivedType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                .Where(n => !n.IsDefined(typeof(JsonIgnoreAttribute))))
            {
                if(member is PropertyInfo property)
                {
                    //Console.WriteLine(".{0}", property.Name);
                    processor.ProcessBuildAssetMemberValue(property.GetValue(instance), property.PropertyType);
                }
                if(member is FieldInfo field)
                {
                    processor.ProcessBuildAssetMemberValue(field.GetValue(instance), field.FieldType);
                }
            }
        } 
        catch(Exception error)
        {
            Console.WriteLine(error.Message);
        }
    }
}