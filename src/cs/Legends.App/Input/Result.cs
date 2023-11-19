using System;
using MonoGame.Extended;
using MonoGame.Extended.Input.InputListeners;

namespace Legends.App.Input;    

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
