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

namespace Legends.Content.Pipline;

public static class ContentProcessorExtensions
{
    private static readonly Type[] ExclusionTypes =
    {
        typeof(string),
        typeof(object),
        typeof(Vector2),
        typeof(Vector3),
        typeof(Vector4),
        typeof(Size2),
        typeof(Point2),
        typeof(RectangleF),
        typeof(Color),
        typeof(Matrix)
    };
    
    private static void BuildAsset(this ContentProcessorContext processor, Asset asset)
    {
        processor.GetType()
                 .GetMethods()
                 .Single(n =>   n.Name == "BuildAsset" 
                                && n.IsGenericMethod
                                && n.GetParameters().Length == 2)
                 .MakeGenericMethod(asset.MakeGenericType(), asset.MakeGenericType()).Invoke(processor, new [] { asset.AsExternalReference(), null });
    }

    private static void ProcessBuildAssetMemberValue(this ContentProcessorContext processor, object instance)
    {
        if(instance == null 
            || ExclusionTypes.Any(n => n == instance.GetType())
            || instance.GetType().IsPrimitive
            || instance.GetType().IsEnum
            || instance.GetType().IsValueType) return;

        if(instance is Asset asset) processor.BuildAsset(asset);
        if(instance is IEnumerable enumerable) foreach(var item in enumerable) processor.ProcessBuildAssetMemberValue(item);
    }

    public static void BuildAssetDependencies(this ContentProcessorContext processor, object instance, Type type)
    {
        foreach(var member in Enumerable.Concat<MemberInfo>(
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance),
            type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            .Where(n => !n.IsDefined(typeof(JsonIgnoreAttribute))))
        {
            if(member is PropertyInfo property)
            {
                processor.ProcessBuildAssetMemberValue(property.GetValue(instance));
            }
            if(member is FieldInfo field)
            {
                processor.ProcessBuildAssetMemberValue(field.GetValue(instance));
            }
        }
    }
}