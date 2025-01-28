using System;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Legends.Engine;

public interface IBounds
{
    bool Contains(Vector2 point);
}
