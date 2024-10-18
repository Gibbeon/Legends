
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Legends.Engine;
using Legends.Engine.Runtime;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Legends.Engine;


public interface IUpdate
{
    void Update(GameTime gameTime);
}