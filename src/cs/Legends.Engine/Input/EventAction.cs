using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;

namespace Legends.Engine.Input;    

public class EventAction
{
    public string Name => EventListener.Name;
    public EventListener EventListener { get; private set; }
    public EventType Type { get; private set; }
    public EventArgs Args { get; private set; }
    public bool Handled { get; set; }

    public MouseEventArgs MouseEventArgs            { get => Args as MouseEventArgs; }
    public TouchEventArgs TouchEventArgs            { get => Args as TouchEventArgs; }
    public KeyboardEventArgs KeyboardEventArgs      { get => Args as KeyboardEventArgs; }

    public EventAction(EventListener listener, EventType type, EventArgs args)
    {
        EventListener = listener;
        Type = type;
        Args = args;
    }

    public Point GetPosition()
    {
        if(Args is MouseEventArgs)
        {
            return (Args as MouseEventArgs).Position;
        }    

        if(Args is TouchEventArgs)
        {
            return (Args as TouchEventArgs).RawTouchLocation.Position.ToPoint();
        }    

        throw new ArgumentException("Not supported on this event type.");
    }

    public float GetScrollValue()
    {
        return (Args as MouseEventArgs).ScrollWheelValue / 1000.0f;
    }

    public float GetScrollDelta()
    {
        return (Args as MouseEventArgs).ScrollWheelDelta / 1000.0f;
    }

    public char GetCharacter()
    {
        if(Args is KeyboardEventArgs keyArgs)
        {
            return keyArgs.Character == null ? (char)0 : (char)keyArgs.Character;
        }

        return (char)0; 
    }
}
