using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using MonoGame.Extended;

namespace Legends.Engine;

public interface IResetable
{
    void Reset();
}
public interface IInitalizable : IResetable, IDisposable
{
    void Initialize();
}

