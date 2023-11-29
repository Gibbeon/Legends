using Microsoft.Xna.Framework.Content;
using System.Linq;
using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Legends.Engine.Graphics2D;

namespace Legends.Engine.Serialization;

public static class ContentReaderExtensions
{
    private static MethodInfo _readRawObject;
    private static MethodInfo[] _contentReaderReadMethods;
    public static void ReadBaseObject<TType>(this ContentReader input, TType result)
    {
        if(typeof(TType).BaseType != null && typeof(TType).BaseType != typeof(object))
        {
            _readRawObject = _readRawObject ?? 
                typeof(ContentReader).GetMethods().Single(n => n.Name == "ReadRawObject" && n.IsGenericMethod && n.GetParameters().Length == 1 && n.GetParameters()[0].ParameterType.IsGenericParameter);
            _readRawObject.MakeGenericMethod(typeof(TType).BaseType)?.Invoke(input, new object[] { result });
            //input.ReadRawObject<SceneObject>(result);
        }
    }

    static IEnumerable<MethodInfo> GetExtensionMethods(Type extendedType, Assembly? assembly = default)
    {
        assembly = assembly ?? typeof(ContentReaderExtensions).Assembly;
        var isGenericTypeDefinition = extendedType.IsGenericType && extendedType.IsTypeDefinition;
        var query = from type in assembly.GetTypes()
            where type.IsSealed && !type.IsGenericType && !type.IsNested
            from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            where method.IsDefined(typeof(ExtensionAttribute), false)
            where isGenericTypeDefinition
                ? method.GetParameters()[0].ParameterType.IsGenericType && method.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == extendedType
                : method.GetParameters()[0].ParameterType == extendedType
            select method;
        return query;
    }

    public static void ReadFields<TType>(this ContentReader input, TType value)
    {
        var fields = typeof(TType).GetFields(
            BindingFlags.DeclaredOnly |
            BindingFlags.Public |  
            BindingFlags.Instance);

        var properties = typeof(TType).GetProperties(
            BindingFlags.DeclaredOnly |
            BindingFlags.Public |  
            BindingFlags.Instance);

        _contentReaderReadMethods = _contentReaderReadMethods ?? Enumerable.Concat(input.GetType().GetMethods(
                                                                        BindingFlags.Public |  
                                                                        BindingFlags.Instance)
                                                                        .Where(n => n.Name.StartsWith("Read")),
                                                                        GetExtensionMethods(input.GetType()).Where(n => n.Name.StartsWith("Read"))).ToArray();

        foreach(var field in fields)
        {

        }

        foreach(var property in properties)
        {
            if(property.CustomAttributes.Any(n => n.AttributeType == typeof(JsonIgnoreAttribute)))
            {
                //Log<TType>("!!Ignoring Property: {0}", property.Name);
                continue;
            }

            var readMethod = _contentReaderReadMethods.SingleOrDefault(
                    n => !n.IsStatic && n.ReturnType.IsAssignableTo(property.PropertyType)
                    || n.IsStatic && n.ReturnType.IsAssignableTo(property.PropertyType));

            if(readMethod != null)
            {
                if(!readMethod.IsStatic)
                {
                    property.SetValue(value, readMethod.Invoke(input, new object?[]{}));
                } 
                else
                {
                    property.SetValue(value, readMethod.Invoke(null, new object?[] { input }));
                }
            }
            else if(property.PropertyType.IsEnum)
            {
                property.SetValue(value, Enum.Parse(property.PropertyType, input.ReadString()));
            }
            else if(property.PropertyType.IsArray || property.PropertyType.GetInterface(typeof(IEnumerable).Name) != null)
            {  
                int count = input.ReadInt32();
                var itemType = property.PropertyType.IsArray ? property.PropertyType.GetElementType() :
                    property.PropertyType.GenericTypeArguments[0];

                var list = property.GetValue( value );
                if(list == null)
                {
                    if(property.PropertyType.IsArray)
                    {
                        list = Array.CreateInstance(itemType, count);
                    }
                    else
                    {
                        list = Activator.CreateInstance(property.PropertyType);
                        if(list.GetType().GetInterfaces().Any(n => n.IsGenericType && n.GetGenericTypeDefinition() == typeof(IList<>)))
                        {
                            
                        }
                    }

                    property.SetValue(value, list);
                }

                for(int i = 0; i < count; i++)
                {
                    readMethod = _contentReaderReadMethods.SingleOrDefault(
                                n => !n.IsStatic && n.ReturnType.IsAssignableTo(itemType)
                                || n.IsStatic && n.ReturnType.IsAssignableTo(itemType));

                    if(readMethod == null)
                    {
                        readMethod = _contentReaderReadMethods.Single(n => n.Name == "ReadObject" && n.IsGenericMethod && n.GetParameters().Length == 0).MakeGenericMethod(itemType);
                    }

                    if(list.GetType().IsArray)
                    {
                        ((Array)list).SetValue(readMethod.Invoke(input, null), i);
                    }
                    else
                    {
                        var addMethod = typeof(ICollection<>).MakeGenericType(itemType).GetMethod("Add");
                        addMethod.Invoke(list, new object?[] { readMethod.Invoke(input, null) });
                    }
                }
            }
            else
            {
                readMethod = _contentReaderReadMethods.Single(n => n.Name == "ReadObject" && n.IsGenericMethod && n.GetParameters().Length == 0).MakeGenericMethod(property.PropertyType);

                property.SetValue(value, readMethod.Invoke(input, null));
            }
        }
    }
}

public class SceneGenericReader : ContentTypeReader<Scene>
{
    protected override Scene Read(ContentReader input, Scene existingInstance)
    {
        var result = existingInstance ?? new Scene(input.ContentManager.ServiceProvider);

        input.ReadBaseObject(result);
        input.ReadFields(result);

        return result;
    }
}

public class SceneObjectGenericReader : ContentTypeReader<SceneObject>
{
    protected override SceneObject Read(ContentReader input, SceneObject existingInstance)
    {
        var result = existingInstance ?? new SceneObject(input.ContentManager.ServiceProvider);

        input.ReadBaseObject(result);
        input.ReadFields(result);

        return result;
    }
}

public class SpatialGenericReader : ContentTypeReader<Spatial>
{
    protected override Spatial Read(ContentReader input, Spatial existingInstance)
    {
        var result = existingInstance ?? new Spatial();

        input.ReadBaseObject(result);
        input.ReadFields(result);

        return result;
    }
}

public class CameraGenericReader : ContentTypeReader<Camera>
{
    protected override Camera Read(ContentReader input, Camera existingInstance)
    {
        var result = existingInstance ?? new Camera(input.ContentManager.ServiceProvider, null);

        input.ReadBaseObject(result);
        input.ReadFields(result);

        return result;
    }
}

public class TextRenderBehaviorReader : ContentTypeReader<TextRenderBehavior>
{
    protected override TextRenderBehavior Read(ContentReader input, TextRenderBehavior existingInstance)
    {
        var result = existingInstance ?? new TextRenderBehavior(input.ContentManager.ServiceProvider, null);

        input.ReadBaseObject(result);
        input.ReadFields(result);

        return result;
    }
}


public class GenericReader<TType> : ContentTypeReader<TType>
{
    protected override TType Read(ContentReader input, TType existingInstance)
    {
        var result = existingInstance ?? (TType)Activator.CreateInstance(typeof(TType), new object?[] { input.ContentManager.ServiceProvider, null });

        input.ReadBaseObject(result);
        input.ReadFields(result);

        return result;
    }
}


/*
public class SpatialReader : ContentTypeReader<Spatial.SpatialDesc>
{
    protected override Spatial.SpatialDesc Read(ContentReader input, Spatial.SpatialDesc existingInstance)
    {
        var result = existingInstance ?? new Spatial.SpatialDesc();

        result.Position   = input.ReadVector2();
        result.Scale      = input.ReadVector2();
        result.Origin     = input.ReadVector2();
        result.Rotation   = input.ReadSingle();
        result.Size       = new MonoGame.Extended.Size2(input.ReadSingle(), input.ReadSingle());
        
        return result;
    }
}

public class SceneObjectReader : ContentTypeReader<SceneObject.SceneObjectDesc>
{
    protected override SceneObject.SceneObjectDesc Read(ContentReader input, SceneObject.SceneObjectDesc existingInstance)
    {
        var result = existingInstance ?? new SceneObject.SceneObjectDesc();
        
        input.ReadRawObject<Spatial.SpatialDesc>(result);

        result.Name = input.ReadString();
        result.Enabled = input.ReadBoolean();
        result.IsVisible = input.ReadBoolean();

        var numTags = input.ReadInt32();
        for(int i = 0; i < numTags; i++)
        {
            result.Tags.Add(input.ReadString());
        }

        var numChildren = input.ReadInt32();
        for(int i = 0; i < numChildren; i++)
        {
            result.Children.Add(input.ReadObject<SceneObject.SceneObjectDesc>());
        }

        var numBehaviors = input.ReadInt32();
        for(int i = 0; i < numBehaviors; i++)
        {
            result.Behaviors.Add(input.ReadObject<IBehavior.BehaviorDesc>());
        }

        return result;
    }
}

public class SceneReader : ContentTypeReader<Scene.SceneDesc>
{
    protected override Scene.SceneDesc Read(ContentReader input, Scene.SceneDesc existingInstance)
    {
        var result = existingInstance ?? new Scene.SceneDesc();        
        input.ReadRawObject<SceneObject.SceneObjectDesc>(result); 

        result.Camera = input.ReadObject<Camera.CameraDesc>();       

        return result;
    }
}

public class CameraReader : ContentTypeReader<Camera.CameraDesc>
{
    protected override Camera.CameraDesc Read(ContentReader input, Camera.CameraDesc existingInstance)
    {
        var result = existingInstance ?? new Camera.CameraDesc();        
        input.ReadRawObject<SceneObject.SceneObjectDesc>(result);       

        return result;
    }
}

public class TextRenderBehaviorReader : ContentTypeReader<TextRenderBehavior.TextRenderBehaviorDesc>
{
    protected override TextRenderBehavior.TextRenderBehaviorDesc Read(ContentReader input, TextRenderBehavior.TextRenderBehaviorDesc existingInstance)
    {
        var result = existingInstance ?? new TextRenderBehavior.TextRenderBehaviorDesc();        
        input.ReadRawObject<ActivatorDesc>(result); 
        result.Text     = input.ReadString();
        result.Color    = input.ReadColor();
        result.Font     = input.ReadString();      

        return result;
    }
}
*/