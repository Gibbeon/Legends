using MonoGame.Extended;
using SlyEngine.Graphics2D;

namespace SlyEngine;

public interface IBehavior : IUpdate
{
    GameObject Parent { get; }
}
