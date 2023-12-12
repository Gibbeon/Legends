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
using System.Collections.Generic;

namespace Legends.Content.Pipline;

/*
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
    
    public static void BuildAssetReference(this ContentProcessorContext processor, Asset asset)
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

    private static void GetAssetMemberValue(this ContentProcessorContext processor, object instance, Type type, ref IList<Asset> results)
    { 
        var derivedType = instance == null ? type : instance.GetType();

        if(instance == null 
                || derivedType == typeof(object)
                || ExclusionTypes.Any(n => n.IsAssignableFrom(derivedType))
                || instance.GetType().IsPrimitive
                || instance.GetType().IsEnum
                || instance.GetType().IsValueType) return;

        if(instance is Asset asset) results.Add(asset);
        else if(instance is IEnumerable enumerable) foreach(var item in enumerable) processor.GetAssetDependencies(item, item.GetType(), results);
        else processor.GetAssetDependencies(instance, type, results);
    }

    public static IEnumerable<Asset> GetAssetDependencies(this ContentProcessorContext processor, object instance, Type type, IList<Asset> results = default)
    {
        results ??= new List<Asset>();

        try
        {
            var derivedType = instance == null ? type : instance.GetType();

            if(instance == null 
                || derivedType == typeof(object)
                || ExclusionTypes.Any(n => n.IsAssignableFrom(derivedType))
                || instance.GetType().IsPrimitive
                || instance.GetType().IsEnum
                || instance.GetType().IsValueType) return results;

            if(instance is Asset asset) { results.Add(asset); }            

            foreach(var member in Enumerable.Concat<MemberInfo>(
                derivedType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty),
                derivedType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                .Where(n => !n.IsDefined(typeof(JsonIgnoreAttribute))))
            {
                if(member is PropertyInfo property)
                {
                    //Console.WriteLine(".{0}", property.Name);
                    processor.GetAssetMemberValue(property.GetValue(instance), property.PropertyType, ref results);
                }
                if(member is FieldInfo field)
                {
                    processor.GetAssetMemberValue(field.GetValue(instance), field.FieldType, ref results);
                }
            }
        } 
        catch(Exception error)
        {
            Console.WriteLine(error.Message);
        }

        return results;
    }
}
*/