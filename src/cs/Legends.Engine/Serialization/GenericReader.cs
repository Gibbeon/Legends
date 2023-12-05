using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;

using Legends.Engine.Graphics2D;
using MonoGame.Extended;

namespace Legends.Engine.Serialization;

public class GenericReaderStack
{
    private static Stack<object> _parentObjects;
    public static Stack<object> ParentObjects => _parentObjects ?? (_parentObjects = new Stack<object>());
}

public class GenericReader<TType> : ContentTypeReader<TType>
{
    protected override TType Read(ContentReader input, TType existingInstance)
    {
        object[] paramConstructors;

        if(GenericReaderStack.ParentObjects.Count > 0)
        {
            paramConstructors = new object[] { input.ContentManager.ServiceProvider, GenericReaderStack.ParentObjects.Peek() };
        }
        else
        {
            paramConstructors = new object[] { input.ContentManager.ServiceProvider };
        }

        var result = existingInstance ?? 
            ContentReaderExtensions.Create<TType>(paramConstructors);

        if(result == null) throw new NotSupportedException();
        
        GenericReaderStack.ParentObjects.Push(result);

        input.ReadFields(result);

        GenericReaderStack.ParentObjects.Pop();

        return result;
    }
}