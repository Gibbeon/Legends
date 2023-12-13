using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended;
using Legends.Engine.Graphics2D;
using Microsoft.Xna.Framework.Content;
using SharpDX.D3DCompiler;
using Legends.Engine;
using SharpFont;
using System.Xml.Linq;
using Legends.Engine.Content;
using System;

namespace Legends.Content.Pipline;

public static class ContentPrimitivesExtensions
{
    public static void Write(this ContentWriter output, Size2 size2)
    {
        output.Write(size2.Width);
        output.Write(size2.Height);
    }

    public static Size2 ReadSize2(this ContentReader input)
    {
        return new Size2(input.ReadSingle(), input.ReadSingle());
    }

    public static void Write(this ContentWriter output, IRef result)
    {
        if(result == null) 
        {
            output.Write7BitEncodedInt(0);
        }
        else if(result.IsExternal)
        {
            output.Write7BitEncodedInt(1);
            output.Write(result.RefType.FullName);
            output.Write(result.Name);
        } 
        else
        {
            output.Write7BitEncodedInt(2);
            output.Write(result.RefType.FullName);
            output.WriteObject(result.Get(), result.RefType);
        }
    }

    public static IRef ReadRef(this ContentReader input)
    {
        switch(input.Read7BitEncodedInt())
        {
            case 0: return null;
            case 1: 
            {
                var type = Type.GetType(input.ReadString());
                return (IRef)Activator.CreateInstance(typeof(Ref<>).MakeGenericType(type), input.ReadString());
            }
            
            case 2: 
            {
                var type = Type.GetType(input.ReadString());
                var instance = input.ReadComplexObject(null, type);
                if(instance == null) return null;
                return (IRef)Activator.CreateInstance(typeof(Ref<>).MakeGenericType(type), instance);
            }
        }

        
        return null;
    }
}