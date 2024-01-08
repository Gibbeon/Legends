using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using MonoGame.Extended;

namespace Legends.Engine;

public interface IInitalizable : IDisposable
{
    void Initialize();
    void Reset();
}

