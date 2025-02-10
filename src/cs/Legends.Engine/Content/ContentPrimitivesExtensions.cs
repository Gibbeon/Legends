using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Content;
using System;
using Microsoft.Xna.Framework;
using Legends.Engine.Runtime;
using System.IO;



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

    public static void Write(this ContentWriter output, SizeF size2)
    {
        output.Write(size2.Width);
        output.Write(size2.Height);
    }

    public static SizeF ReadSizeF(this ContentReader input)
    {
        return new SizeF(input.ReadSingle(), input.ReadSingle());
    }

    public static void Write(this ContentWriter output, Size size2)
    {
        output.Write(size2.Width);
        output.Write(size2.Height);
    }

    public static Size ReadSize(this ContentReader input)
    {
        return new Size(input.ReadInt32(), input.ReadInt32());
    }

    public static void Write(this ContentWriter output, Rectangle rectangle)
    {
        output.Write(rectangle.X);
        output.Write(rectangle.Y);
        output.Write(rectangle.Width);
        output.Write(rectangle.Height);
    }

    public static Rectangle ReadRectangle(this ContentReader input)
    {
        return new Rectangle(input.ReadInt32(), input.ReadInt32(), input.ReadInt32(), input.ReadInt32());
    }


    public static void Write(this ContentWriter output, RectangleF rectangle)
    {
        output.Write(rectangle.X);
        output.Write(rectangle.Y);
        output.Write(rectangle.Width);
        output.Write(rectangle.Height);
    }

    public static RectangleF ReadRectangleF(this ContentReader input)
    {
        return new RectangleF(input.ReadSingle(), input.ReadSingle(), input.ReadSingle(), input.ReadSingle());
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
                var typeName        = input.ReadString(); 
                var instanceName    = input.ReadString();

                var type = TypeCache.GetType(typeName);
                
                return (type.CreateOrDefault(input.ContentManager.ServiceProvider, input.GetParents().Peek(), instanceName) ?? 
                        type.CreateOrDefault(input.ContentManager.ServiceProvider, instanceName)) as IAsset;

                //return Activator.CreateInstance(TypeCache.GetType(typeName), input.ContentManager.ServiceProvider, input.GetParents().Peek(), instanceName) as IAsset;
            case AssetStatus.Instance: 
            default:
                Console.WriteLine("ReadAsset AssetStatus.Instance");
                return input.ReadComplexObject(null, typeof(IAsset)) as IAsset;
        }
    }

    public static void Write(this ContentWriter output, IAsset result)
    {        
        ContentLogger.Trace(output.Seek(0, SeekOrigin.Current), 
            "Write(this ContentWriter output, IAsset result[{0}])", result);

        if(result == null) {
            Console.WriteLine("Write AssetStatus.Null");
            output.Write7BitEncodedInt((int)AssetStatus.Null);
            return;
        }
        
        if(string.IsNullOrEmpty(result.Name))
        {
            
        ContentLogger.Trace(output.Seek(0, SeekOrigin.Current), 
            "Write AssetStatus.Instance {0}, {1}", result, result.GetType());
            output.Write7BitEncodedInt((int)AssetStatus.Instance);
            
        ContentLogger.Trace(output.Seek(0, SeekOrigin.Current),
            "BEGIN output.WriteObject(result [{0}], result.GetType() [{1}]);", result, result.GetType()); 
            output.WriteObject(result, result.GetType());
         ContentLogger.Trace(output.Seek(0, SeekOrigin.Current),
            "END output.WriteObject(result [{0}], result.GetType() [{1}]);", result, result.GetType()); 
      
        }
        else
        {
            Console.WriteLine("Write AssetStatus.Reference");
            output.Write7BitEncodedInt((int)AssetStatus.Reference);
            output.Write(result.GetType().AssemblyQualifiedName);
            output.Write(result.Name);
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