using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;

namespace Legends.App.Input
{
    public class Trigger
    {
        public InputManager Manager { get; private set;}
        public string Command { get; private set; }
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

        public Trigger(InputManager manager, string command, Func<EventType, EventArgs, bool> evaluator)
        {
            Manager = manager;
            Command = command;
            _evaluate = evaluator;
            _modifiers = new List<Func<EventType, EventArgs, bool>>();
        }

        IList<Func<EventType, EventArgs, bool>> _modifiers;
    }

    public class Result
    {
        public string Command => Trigger.Command;

        public Trigger Trigger { get; set; }

        public EventType Type { get; set; }

        public EventArgs Args { get; set; }

        public MouseEventArgs MouseEventArgs            { get => Args as MouseEventArgs; }
        public TouchEventArgs TouchEventArgs            { get => Args as TouchEventArgs; }
        public KeyboardEventArgs KeyboardEventArgs      { get => Args as KeyboardEventArgs; }

        public Point2 GetPosition()
        {
            if(Args is MouseEventArgs)
            {
                return (Args as MouseEventArgs).Position;
            }    

            if(Args is TouchEventArgs)
            {
                return (Args as TouchEventArgs).RawTouchLocation.Position;
            }    

            return Point2.NaN;
        }

        public float GetScrollValue()
        {
            return (Args as MouseEventArgs).ScrollWheelValue / 1000.0f;
        }

        public float GetScrollDelta()
        {
            return (Args as MouseEventArgs).ScrollWheelDelta / 1000.0f;
        }

        public char? GetCharacter()
        {
            if(Args is KeyboardEventArgs)
            {
                return (Args as KeyboardEventArgs).Character;
            }

            return null; 
        }
    }

    public enum EventType
    {
        None,
        KeyPressed,
        KeyReleased,
        KeyTyped,
        MouseClicked,
        MouseMove,
        MouseScroll

    }

    public class InputManager
    {
        private Legends.App.Input.KeyboardListener    _keyboardListener;
        private MouseListener       _mouseListener;

        private IList<Trigger> _triggers;
        private IList<Result> _results;

        public IList<Result> Results => _results;

        public KeyboardListener KeyboardListener => _keyboardListener;
        public MouseListener    MouseListener => _mouseListener;

        public InputManager() : this( new KeyboardListenerSettings())
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
}