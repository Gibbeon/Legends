using MonoGame.Extended;
using Legends.Engine.Graphics2D;

namespace Legends.Engine;

public interface IBehavior : IUpdate
{
    GameObject Parent { get; }
}
