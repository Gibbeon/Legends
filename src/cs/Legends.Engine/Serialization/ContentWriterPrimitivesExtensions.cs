using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Content;
using Legends.Engine;
using Legends.Engine.Content;
using System;
using System.Collections;
using System.Linq;

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
        else if(result.IsExternal && !result.IsExtended)
        {
            output.Write7BitEncodedInt(1);
        } 
        else if(result.IsExternal && result.IsExtended)
        {
            output.Write7BitEncodedInt(2);
        }
        else
        {
            output.Write7BitEncodedInt(3);
        }
        
        if(!result.IsExternal || result.IsExtended)
        {
            // need the actual type name
            output.Write(result.RefType.AssemblyQualifiedName);
            output.Write(result.Name);
            output.WriteObject(result.Get(), result.RefType);
        } 
        else
        {
            output.Write(result.RefType.AssemblyQualifiedName);
            output.Write(result.Name);
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
                var result = (IRef)Activator.CreateInstance(typeof(Ref<>).MakeGenericType(type), input.ReadString());
                result.Load(input.ContentManager);
                return result;
            }
            case 2: 
            {
                var typeName = input.ReadString(); 
                var srcName = input.ReadString();
                var type = Type.GetType(typeName);

                input.ContentManager.Load<object>(srcName);
                var result = (IRef)Activator.CreateInstance(typeof(Ref<>).MakeGenericType(type), srcName);
                result.Set(input.ReadComplexObject(result.Get(), type));
                return result;
            }
            case 3: 
            {
                var typeName = input.ReadString(); 
                var type = Type.GetType(typeName);
                var result = (IRef)Activator.CreateInstance(typeof(Ref<>).MakeGenericType(type), input.ReadString());
                result.Set(input.ReadComplexObject(result.Get(), type));
                return result;
            }
        }

        
        return null;
    }
}