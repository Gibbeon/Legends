using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;

namespace Legends.Engine.Input;

public interface IInputHandlerService
{
    InputManager? Current { get; }
    void Push(InputManager manager);
    void Pop();
}
