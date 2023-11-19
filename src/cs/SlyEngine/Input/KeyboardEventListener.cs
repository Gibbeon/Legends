using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;

namespace SlyEngine.Input;

public class KeyboardListener : InputListener
{
    private Array _keysValues = Enum.GetValues(typeof(Keys));

    private bool[]           _isInitial;

    private TimeSpan[]       _lastPressTime;

    private KeyboardState   _previousState;

    public KeyboardState KeyboardState => _previousState;

    public bool RepeatPress { get; }

    public int InitialDelay { get; }

    public int RepeatDelay { get; }

    public event EventHandler<KeyboardEventArgs>? KeyTyped;

    public event EventHandler<KeyboardEventArgs>? KeyPressed;

    public event EventHandler<KeyboardEventArgs>? KeyReleased;

    public KeyboardListener()
        : this(new KeyboardListenerSettings())
    {
    }

    public KeyboardListener(KeyboardListenerSettings settings)
    {
        RepeatPress     = settings.RepeatPress;
        InitialDelay    = settings.InitialDelayMilliseconds;
        RepeatDelay     = settings.RepeatDelayMilliseconds;
        
        _isInitial      = new bool[256];        
        _lastPressTime  = new TimeSpan[256];
    }

    public override void Update(GameTime gameTime)
    {
        KeyboardState state = Keyboard.GetState();
        RaisePressedEvents(gameTime, state);
        RaiseReleasedEvents(state);
        if (RepeatPress)
        {
            RaiseRepeatEvents(gameTime, state);
        }

        _previousState = state;
    }

    private void RaisePressedEvents(GameTime gameTime, KeyboardState currentState)
    {
        if (currentState.IsKeyDown(Keys.LeftAlt) || currentState.IsKeyDown(Keys.RightAlt))
        {
            return;
        }

        foreach (Keys item in from Keys key in _keysValues
                              where currentState.IsKeyDown(key) && _previousState.IsKeyUp(key)
                              select key)
        {
            KeyboardEventArgs keyboardEventArgs = new KeyboardEventArgs(item, currentState);
            this.KeyPressed?.Invoke(this, keyboardEventArgs);
            if (keyboardEventArgs.Character.HasValue)
            {
                this.KeyTyped?.Invoke(this, keyboardEventArgs);
            }

            //_previousKey = item;
            _lastPressTime[(int)item]    = gameTime.TotalGameTime;
            _isInitial[(int)item]        = true;
        }
    }

    private void RaiseReleasedEvents(KeyboardState currentState)
    {
        foreach (Keys item in from Keys key in _keysValues
                              where currentState.IsKeyUp(key) && _previousState.IsKeyDown(key)
                              select key)
        {
            this.KeyReleased?.Invoke(this, new KeyboardEventArgs(item, currentState));
        }
    }

    private void RaiseRepeatEvents(GameTime gameTime, KeyboardState currentState)
    {       
        foreach (Keys item in from Keys key in _keysValues
                              where currentState.IsKeyDown(key) && _previousState.IsKeyDown(key)
                              select key)
        {
            double totalMilliseconds = (gameTime.TotalGameTime - _lastPressTime[(int)item]).TotalMilliseconds;

            if (_previousState.IsKeyDown(item) && ((_isInitial[(int)item] && totalMilliseconds > (double)InitialDelay) || (!_isInitial[(int)item] && totalMilliseconds > (double)RepeatDelay)))
            {
                KeyboardEventArgs keyboardEventArgs = new KeyboardEventArgs(item, currentState);
                this.KeyPressed?.Invoke(this, keyboardEventArgs);
                if (keyboardEventArgs.Character.HasValue)
                {
                    this.KeyTyped?.Invoke(this, keyboardEventArgs);
                }                
                _lastPressTime[(int)item] = gameTime.TotalGameTime;
                _isInitial[(int)item] = false;
            }
        }
    }
}