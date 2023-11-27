using MonoGame.Extended;
using Legends.Engine.Graphics2D;
using System;
using Microsoft.Xna.Framework;

namespace Legends.Engine;

public interface IBehavior : IUpdate, IDisposable
{
    public class BehaviorDesc
    {

    }
    
    SceneObject Parent { get; }

    void Draw(GameTime gameTime);
}
