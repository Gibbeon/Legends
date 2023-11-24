using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;

namespace Legends.Engine.Input;

public class InputHandlerService : IUpdate, IInputHandlerService
{
    public SystemServices Services { get; private set; }

    public InputManager? Current => _managers.Count == 0 ? null : _managers.Peek();

    private Stack<InputManager> _managers;

    public InputHandlerService(SystemServices services)
    {
        Services = services;
        services.Services.AddService<IInputHandlerService>(this);
        _managers = new Stack<InputManager>();
    }

    public void Update(GameTime gameTime)
    {
        Current?.Update(gameTime);
    }

    public void Push(InputManager manager)
    {
        _managers.Push(manager);
        manager.Activate();
    }

    public void Pop()
    {
        _managers.Pop().Deactivate();
    }
}
