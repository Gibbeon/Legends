using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace Legends.Engine.Input;
public class Trigger
{
    public InputManager Manager { get; private set;}
    public string Command { get; private set; }
    public string Label { get; set; }
    private Func<EventType, EventArgs, bool> _evaluate;

    public bool Eval(EventType type, EventArgs args)
    {
        return _evaluate(type, args) && _modifiers.All((fn) => { return fn(type, args); });
    }

    public Trigger WithModifierAny(params Keys[] modifiers)
    {                
        _modifiers.Add((type, args) => modifiers.Any(n => Manager.KeyboardListener.KeyboardState.IsKeyDown(n)));

        return this;   
    }

    public Trigger WithModifierAll(params Keys[] modifiers)
    {                
        _modifiers.Add((type, args) => modifiers.All(n => Manager.KeyboardListener.KeyboardState.IsKeyDown(n)));

        return this;   
    }

    public Trigger(InputManager manager, string command, string label, Func<EventType, EventArgs, bool> evaluator)
    {
        Manager = manager;
        Command = command;
        Label = label;
        _evaluate = evaluator;
        _modifiers = new List<Func<EventType, EventArgs, bool>>();
    }

    IList<Func<EventType, EventArgs, bool>> _modifiers;
}
