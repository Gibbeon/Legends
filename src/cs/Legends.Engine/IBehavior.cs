using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using MonoGame.Extended;

namespace Legends.Engine;

public interface IBehavior : IUpdate, IDisposable
{    
    [JsonIgnore]
    SceneObject Parent { get; }
    void Draw(GameTime gameTime);

    void AttachTo(SceneObject parent);
    void Initialize();
}
