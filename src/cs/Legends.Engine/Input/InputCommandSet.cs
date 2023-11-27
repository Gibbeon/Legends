using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;
using System.Diagnostics;
using MonoGame.Extended.ViewportAdapters;

namespace Legends.Engine.Input;

public class InputCommandSet
{
    private readonly IList<EventListener>   _eventListeners;
    private IList<EventAction>              _eventActions;
    private readonly SystemServices         _services;
    public IEnumerable<EventAction> EventActions => _eventActions.Where(n => !n.Handled);
    public IList<EventListener> EventListeners => _eventListeners;
    public bool Enabled { get; set; }

    public InputCommandSet(SystemServices service)
    {
        _services = service;
        _eventActions = new List<EventAction>();
        _eventListeners = new List<EventListener>();

        _services.GetService<IInputHandlerService>().Current.CommandSets.Add(this);

        Enabled = true;
    }

    internal void AddAction(EventAction action)
    {
        _eventActions.Add(action);
    }

    public void Clear()
    {
        _eventActions = new List<EventAction>();
    }

    public void Add(string action, Func<EventType, EventArgs, bool> test)
    {
        _eventListeners.Add(new EventListener(this, action, test));
    }

    public void Add(string action, EventType eventType)
    {
        Add(action, 
            (type, args) => type == eventType);
    }

    public void Add(string action, EventType eventType, MouseButton button, params Keys[] modifers)
    {
        Add(action, 
            (type, args) => type == eventType
                            && (args as MouseEventArgs)?.Button == button
                            && _services.GetService<IInputHandlerService>().Current.KeyboardListener.IsPressed(modifers));
    }

    public void Add(string action, EventType eventType, Keys key, params Keys[] modifers)
    {
        Add(action, 
            (type, args) => type == eventType
                            && (args as KeyboardEventArgs)?.Key == key
                            && _services.GetService<IInputHandlerService>().Current.KeyboardListener.IsPressed(modifers));    
    }

    public void Dispose()
    {
        _eventActions.Clear();
        _eventListeners.Clear();
        _services.GetService<IInputHandlerService>().Current.CommandSets.Remove(this);
    }

}
