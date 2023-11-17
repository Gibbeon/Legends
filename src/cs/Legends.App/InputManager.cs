using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;

namespace Legends.App.Input
{
    public class Trigger
    {
        public string Command { get; set; }
        public Func<EventType, EventArgs, bool> Eval { get; set; }
    }

    public class Result
    {
        public string Command => Trigger.Command;

        public Trigger Trigger { get; set; }

        public EventType Type { get; set; }

        public EventArgs Args { get; set; }

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
        MouseClicked

    }

    public class InputManager
    {
        private Legends.App.Input.KeyboardListener    _keyboardListener;
        //GamePadListener     _gamePadListener = new GamePadListener();
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
            _mouseListener.MouseClicked += (sender, args)   => ProcessEvent(EventType.MouseClicked, args);
        }

        public void Update(GameTime gameTime)
        {
            _results.Clear();
            _keyboardListener.Update(gameTime);
            _mouseListener.Update(gameTime);
        }

        public void Register(string command, Func<EventType, EventArgs, bool> test)
        {
            _triggers.Add(new Trigger() {Command = command, Eval = test});
        }

        public void Register(string command, EventType eventType)
        {
            Register(command, (type, args) => type == eventType);
        }

        public void Register(string command, Keys key, EventType eventType = EventType.KeyPressed)
        {
            Register(command, (type, args) => type == eventType && (args as KeyboardEventArgs).Key == key);
        }

        public void Register(string command, MouseButton button, EventType eventType = EventType.MouseClicked)
        {
            Register(command, (type, args) => type == eventType && (args as MouseEventArgs).Button == button);
        }

        protected void ProcessEvent(EventType type, EventArgs args)
        {
            foreach(var command in _triggers)
            {
                if(command.Eval(type, args))
                {
                    _results.Add(new Result() { Trigger = command, Type = type, Args = args });
                    Console.WriteLine(command.Command);
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