using System;
using MonoGame.Extended;
using MonoGame.Extended.Input.InputListeners;

namespace SlyEngine.Input;    

public class Result
{
    public string Command => Trigger.Command;

    public Trigger Trigger { get; private set; }

    public EventType Type { get; private set; }

    public EventArgs Args { get; private set; }

    public MouseEventArgs? MouseEventArgs            { get => Args as MouseEventArgs; }
    public TouchEventArgs? TouchEventArgs            { get => Args as TouchEventArgs; }
    public KeyboardEventArgs? KeyboardEventArgs      { get => Args as KeyboardEventArgs; }

    public Result(Trigger trigger, EventType type, EventArgs args)
    {
        Trigger = trigger;
        Type = type;
        Args = args;
    }

    public Point2? GetPosition()
    {
        if(Args is MouseEventArgs)
        {
            return (Args as MouseEventArgs)?.Position;
        }    

        if(Args is TouchEventArgs)
        {
            return (Args as TouchEventArgs)?.RawTouchLocation.Position;
        }    

        return Point2.NaN;
    }

    public float? GetScrollValue()
    {
        return (Args as MouseEventArgs)?.ScrollWheelValue / 1000.0f;
    }

    public float? GetScrollDelta()
    {
        return (Args as MouseEventArgs)?.ScrollWheelDelta / 1000.0f;
    }

    public char? GetCharacter()
    {
        if(Args is KeyboardEventArgs)
        {
            return (Args as KeyboardEventArgs)?.Character;
        }

        return null; 
    }
}
