using System;

namespace Legends.Engine;

public interface IResetable
{
    void Reset();
}
public interface IInitalizable : IResetable, IDisposable
{
    void Initialize();
}

