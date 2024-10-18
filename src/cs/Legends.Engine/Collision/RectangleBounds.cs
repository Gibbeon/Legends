
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Legends.Engine;
using Legends.Engine.Runtime;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Legends.Engine.Collision;

public class RectangleBounds : IBounds
{
    private BoundingRectangle _rectangle;

    public RectangleBounds() : this(BoundingRectangle.Empty)
    {

    }

    public RectangleBounds(Vector2 position, Vector2 size): 
        this(new BoundingRectangle((((Vector2)size - position)/ 2), size / 2))
    {

    }

    public RectangleBounds(BoundingRectangle rectangle)
    {
        _rectangle = rectangle;
    }

    public static bool Intersects(ref BoundingRectangle first, ref BoundingRectangle second)
    {
        Vector2 vector = first.Center - second.Center;
        Vector2 vector2 = first.HalfExtents + second.HalfExtents;
        if (Math.Abs(vector.X) < vector2.X)
        {
            return Math.Abs(vector.Y) < vector2.Y;
        }

        return false;
    }

    public bool Collide(RectangleBounds other)
    {
        return Intersects(ref _rectangle, ref other._rectangle);
    }

    public bool Equals(IBounds other)
    {
        return other is RectangleBounds rectangleBounds ? rectangleBounds._rectangle == this._rectangle : false;
    }

    public bool Equals(ref IBounds other)
    {
        return other is RectangleBounds rectangleBounds ? rectangleBounds._rectangle == this._rectangle : false;
    }
}