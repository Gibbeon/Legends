using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;

namespace Legends.App.Input;
public class InputManager
{
    private Legends.App.Input.KeyboardListener    _keyboardListener;
    private MouseListener       _mouseListener;

    private IList<Trigger> _triggers;
    private IList<Result> _results;

    public IList<Result> Results => _results;

    public KeyboardListener KeyboardListener => _keyboardListener;
    public MouseListener    MouseListener => _mouseListener;

    public InputManager() : this(new KeyboardListenerSettings())
    {

    }

    public InputManager(KeyboardListenerSettings keyboardListenerSettings)
    {
        _triggers = new List<Trigger>();
        _results = new List<Result>();

        _keyboardListener = new KeyboardListener(keyboardListenerSettings);
        _keyboardListener.KeyPressed += (sender, args)  => ProcessEvent(EventType.KeyPressed, args);
        _keyboardListener.KeyReleased += (sender, args) => ProcessEvent(EventType.KeyReleased, args);
        _keyboardListener.KeyTyped += (sender, args)    => ProcessEvent(EventType.KeyTyped, args);

        _mouseListener = new MouseListener();
        
        _mouseListener.MouseClicked += (sender, args)       => ProcessEvent(EventType.MouseClicked, args);
        _mouseListener.MouseDoubleClicked += (sender, args) => ProcessEvent(EventType.MouseClicked, args);

        _mouseListener.MouseWheelMoved+= (sender, args)     => ProcessEvent(EventType.MouseScroll, args);
        _mouseListener.MouseMoved += (sender, args)         => ProcessEvent(EventType.MouseMove, args);
    }

    public void Update(GameTime gameTime)
    {
        _results.Clear();
        _keyboardListener.Update(gameTime);
        _mouseListener.Update(gameTime);
    }

    public Trigger Register(string command, Func<EventType, EventArgs, bool> test)
    {
        var result = new Trigger(this, command, test);
        _triggers.Add(result);
        return result;
    }

    public Trigger Register(string command, EventType eventType)
    {
        return Register(command, (type, args) => type == eventType);
    }

    public Trigger Register(string command, Keys key, EventType eventType = EventType.KeyPressed)
    {
        return Register(command, (type, args) => type == eventType && (args as KeyboardEventArgs).Key == key);
    }

    public Trigger Register(string command, MouseButton button, EventType eventType = EventType.MouseClicked)
    {
        return Register(command, (type, args) => type == eventType && (args as MouseEventArgs).Button == button);
    }

    protected void ProcessEvent(EventType type, EventArgs args)
    {
        foreach(var command in _triggers)
        {
            if(command.Eval(type, args))
            {
                _results.Add(new Result() { Trigger = command, Type = type, Args = args });
            }
        }

    }
    
    public bool HasTriggered(string command)
    {
        foreach(var entry in _results.Where(n => n.Trigger.Command == command))
        {
            return true;                
        }

        return false;
    }

    public bool HasTriggered(string command, out Point2 point)
    {
        foreach(var entry in _results.Where(n => n.Trigger.Command == command))
        {
            point = entry.GetPosition();
            return true;
        }
        point = Point2.NaN;

        return false;
    }
}
