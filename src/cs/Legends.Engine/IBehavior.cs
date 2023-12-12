using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using MonoGame.Extended;
using Legends.Engine.Content;

namespace Legends.Engine;

public interface IBehavior : IUpdate, IDisposable
{    
    [JsonIgnore]
    Ref<SceneObject> Parent { get; }
    void Draw(GameTime gameTime);
}
