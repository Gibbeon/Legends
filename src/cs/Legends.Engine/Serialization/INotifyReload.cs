using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Legends.Engine.Content;
using Legends.Engine.Runtime;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended.Content;
using Newtonsoft.Json;

namespace Legends.Engine;

public interface INotifyReload
{
    void OnReload();
}

