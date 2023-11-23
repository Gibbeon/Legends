using MonoGame.Extended;
using Legends.Engine.Graphics2D;
using System;

namespace Legends.Engine;

public interface IBehavior : IUpdate, IDisposable
{
    GameObject Parent { get; }
}
