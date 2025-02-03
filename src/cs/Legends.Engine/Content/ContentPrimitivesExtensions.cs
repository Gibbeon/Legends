using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Content;
using Legends.Engine;
using Legends.Engine.Content;
using System;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework;
using SharpDX;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework.Graphics;



namespace Legends.Engine.Content;

public static class ContentPrimitivesExtensions
{
    private enum AssetStatus
    {
        Null = 0,
        Reference = 1,
        Instance = 2
    }
    public static void Write(this ContentWriter output, Vector2 size2)
    {
        output.Write(size2.X);
        output.Write(size2.Y);
    }

    public static Vector2 ReadVector2(this ContentReader input)
    {
        return new Vector2(input.ReadSingle(), input.ReadSingle());
    }

    public static IAsset ReadAsset(this ContentReader input)
    {
        switch((AssetStatus)input.Read7BitEncodedInt() )
        {
            case AssetStatus.Null: 
                Console.WriteLine("ReadAsset AssetStatus.Null");
                return null;
            case AssetStatus.Reference: 
                Console.WriteLine("ReadAsset AssetStatus.Reference");
                var typeName = input.ReadString();
                var instanceName = input.ReadString();

                return Activator.CreateInstance(TypeCache.GetType(typeName), input.ContentManager.Load<object>(instanceName)) as IAsset;

                //return input.ContentManager.Load<object>(input.ReadString()) as IAsset;
            case AssetStatus.Instance: 
            default:
                Console.WriteLine("ReadAsset AssetStatus.Instance");
                return input.ReadComplexObject(null, typeof(IAsset)) as IAsset;
        }
    }

    public static void Write(this ContentWriter output, IAsset result)
    {        
        if(result == null) {
            Console.WriteLine("Write AssetStatus.Null");
            output.Write7BitEncodedInt((int)AssetStatus.Null);
            return;
        }
        
        if(string.IsNullOrEmpty(result.AssetName))
        {
            Console.WriteLine("Write AssetStatus.Instance {0}, {1} @ {2}", result, result.GetType(), output.BaseStream.Position);
            output.Write7BitEncodedInt((int)AssetStatus.Instance);
            output.WriteObject(result, result.GetType());
        }
        else
        {
            Console.WriteLine("Write AssetStatus.Reference");
            output.Write7BitEncodedInt((int)AssetStatus.Reference);
            output.Write(result.GetType().AssemblyQualifiedName);
            output.Write(result.AssetName);
        }        
    }

/*
    public static void Write(this ContentWriter output, IRef result)
    {
        output.Write7BitEncodedInt(result == null ? 0 : 1);
        output.Write(result.IsExternal);
        output.Write(result.IsExtended);
        
        output.Write(result.RefType.AssemblyQualifiedName);
        output.Write(result.Name ?? "");
        
        if(!result.IsExternal || result.IsExtended)
        {
            output.WriteObject(result.Get(), result.RefType);
        } 
    }


    public static IRef ReadRef(this ContentReader input)
    {
        if(input.Read7BitEncodedInt() == 0) return null;
        var isExternal = input.ReadBoolean();
        var isExtended = input.ReadBoolean();

        var typeName = input.ReadString(); 
        var srcName = input.ReadString();

        var type = Type.GetType(typeName);

        var result = (IRef)Activator.CreateInstance(typeof(Ref<>).MakeGenericType(type), srcName, null, isExternal, isExtended);

        if(isExternal && !isExtended)
        {
            //result load not working with dybamicassembly
            result.Load(input.ContentManager);
        }
        else if(!isExternal || isExtended)
        {
            result.Set(input.ReadComplexObject(result.Get(), type));
        }
        
        return result;
    }
*/
}