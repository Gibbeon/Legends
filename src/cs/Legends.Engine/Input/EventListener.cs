using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace Legends.Engine.Input;
public class EventListener
{
    public InputManager Manager { get; private set;}
    public string Name { get; private set; }
    public string Label { get; set; }
    private Func<EventType, EventArgs, bool> _evaluate;

    public bool Eval(EventType type, EventArgs args)
    {
        return _evaluate(type, args) && _modifiers.All((fn) => { return fn(type, args); });
    }

    public EventListener WithModifierAny(params Keys[] modifiers)
    {                
        _modifiers.Add((type, args) => modifiers.Any(n => Manager.KeyboardListener.KeyboardState.IsKeyDown(n)));

        return this;   
    }

    public EventListener WithModifierAll(params Keys[] modifiers)
    {                
        _modifiers.Add((type, args) => modifiers.All(n => Manager.KeyboardListener.KeyboardState.IsKeyDown(n)));

        return this;   
    }

    public EventListener(InputManager manager, string actionName, string label, Func<EventType, EventArgs, bool> evaluator)
    {
        Manager = manager;
        Name = actionName;
        Label = label;
        _evaluate = evaluator;
        _modifiers = new List<Func<EventType, EventArgs, bool>>();
    }

    IList<Func<EventType, EventArgs, bool>> _modifiers;
}
