using System;
using Microsoft.Xna.Framework;

namespace Legends.App;

public static class DirectionConstants
{
    public static Vector2 Left = new Vector2(-1, 0);
    public static Vector2 Right = new Vector2(1, 0);
    public static Vector2 Up = new Vector2(0, -1);
    public static Vector2 Down = new Vector2(0, 1);

    public static Vector2 GetNearestFacing(Vector2 value)
    {
        if(MathF.Abs(value.Y) >= MathF.Abs(value.X))
        {
            return value.Y > 0 ? Up : Down;
        }
        return value.X > 0 ? Left : Right;
    }
}
