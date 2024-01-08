using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Legends.Engine.Input;

public interface IInputHandlerService : IUpdate
{
    InputManager Current { get; }
    void Push(InputManager manager);
    void Remove(InputManager manager);
}


public class InputHandlerService : IInputHandlerService
{
    public IServiceProvider Services { get; private set; }

    public InputManager Current => _managers.Count == 0 ? null : _managers[_managers.Count - 1];

    private IList<InputManager> _managers;

    public InputHandlerService(IServiceProvider services)
    {
        Services = services;
        Services.Add<IInputHandlerService>(this);
        
        _managers = new List<InputManager>();
    }

    public void Update(GameTime gameTime)
    {
        Current?.Update(gameTime);
    }

    public void Push(InputManager manager)
    {
        Current?.Deactivate();
        _managers.Add(manager);
        manager.Activate();
    }

    public void Remove(InputManager manager)
    {
        manager.Deactivate();
        _managers.Remove(manager);
        Current?.Activate();
    }
}
