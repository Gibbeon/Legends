using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Content;
using Legends.Engine;
using Legends.Engine.Content;
using System;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework;



namespace Legends.Engine.Content;

public static class ContentPrimitivesExtensions
{
    public static void Write(this ContentWriter output, Vector2 size2)
    {
        output.Write(size2.X);
        output.Write(size2.Y);
    }

    public static Vector2 ReadVector2(this ContentReader input)
    {
        return new Vector2(input.ReadSingle(), input.ReadSingle());
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