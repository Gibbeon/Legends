using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;

namespace Legends.Engine.Input;
public class InputManager
{
    private Legends.Engine.Input.KeyboardListener    _keyboardListener;
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

    public string GetText(string command)
    {
        string result = "";
        foreach(var cmd in _triggers.Where(n => n.Command == command))
        {
            result += (!string.IsNullOrEmpty(result) ? " OR " : "") + cmd.Label;
        }

        return result;
    }

    public string? GetText(EventType type, Keys key) 
    {
        switch(type)
        {
            case EventType.KeyPressed:
            case EventType.KeyReleased:
            case EventType.KeyTyped:
                return KeyToString(key);                
        }
        return "";
    }

    public string? GetText(EventType type, MouseButton button) 
    {
        switch(type)
        {
            case EventType.MouseClicked:
                return button.ToString() + "Click";  
            case EventType.MouseScroll:
                return button.ToString() + "Scroll";                 
        }
        return "";
    }

    public void Update(GameTime gameTime)
    {
        _results.Clear();
        _keyboardListener.Update(gameTime);
        _mouseListener.Update(gameTime);
    }

    public Trigger Register(string command, string? label, Func<EventType, EventArgs, bool> test)
    {
        var result = new Trigger(this, command, label, test);
        _triggers.Add(result);
        return result;
    }

    public Trigger Register(string command, EventType eventType)
    {
        return Register(command, "", (type, args) => type == eventType);
    }

    public Trigger Register(string command, Keys key, EventType eventType = EventType.KeyPressed)
    {
        return Register(command, GetText(eventType, key), (type, args) => type == eventType && (args as KeyboardEventArgs)?.Key == key);
    }

    public Trigger Register(string command, MouseButton button, EventType eventType = EventType.MouseClicked)
    {
        return Register(command, GetText(eventType, button), (type, args) => type == eventType && (args as MouseEventArgs)?.Button == button);
    }

    protected void ProcessEvent(EventType type, EventArgs args)
    {
        foreach(var command in _triggers)
        {
            if(command.Eval(type, args))
            {
                _results.Add(new Result(command, type, args));
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
            var result = entry.GetPosition();
            if(result.HasValue)
            {
                point = result.Value;
                return true;
            }
        }
        point = Point2.NaN;

        return false;
    }

    private static string? KeyToString(Keys key, KeyboardModifiers modifiers = KeyboardModifiers.None)
    {
        bool flag = (modifiers & KeyboardModifiers.Shift) == KeyboardModifiers.Shift;
        if (key == Keys.A)
        {
            return flag ? "A" : "a";
        }

        if (key == Keys.B)
        {
            return flag ? "B" : "b";
        }

        if (key == Keys.C)
        {
            return flag ? "C" : "c";
        }

        if (key == Keys.D)
        {
            return flag ? "D" : "d";
        }

        if (key == Keys.E)
        {
            return flag ? "E" : "e";
        }

        if (key == Keys.F)
        {
            return flag ? "F" : "f";
        }

        if (key == Keys.G)
        {
            return flag ? "G" : "g";
        }

        if (key == Keys.H)
        {
            return flag ? "H" : "h";
        }

        if (key == Keys.I)
        {
            return flag ? "I" : "i";
        }

        if (key == Keys.J)
        {
            return flag ? "J" : "j";
        }

        if (key == Keys.K)
        {
            return flag ? "K" : "k";
        }

        if (key == Keys.L)
        {
            return flag ? "L" : "l";
        }

        if (key == Keys.M)
        {
            return flag ? "M" : "m";
        }

        if (key == Keys.N)
        {
            return flag ? "N" : "n";
        }

        if (key == Keys.O)
        {
            return flag ? "O" : "o";
        }

        if (key == Keys.P)
        {
            return flag ? "P" : "p";
        }

        if (key == Keys.Q)
        {
            return flag ? "Q" : "q";
        }

        if (key == Keys.R)
        {
            return flag ? "R" : "r";
        }

        if (key == Keys.S)
        {
            return flag ? "S" : "s";
        }

        if (key == Keys.T)
        {
            return flag ? "T" : "t";
        }

        if (key == Keys.U)
        {
            return flag ? "U" : "u";
        }

        if (key == Keys.V)
        {
            return flag ? "V" : "v";
        }

        if (key == Keys.W)
        {
            return flag ? "W" : "w";
        }

        if (key == Keys.X)
        {
            return flag ? "X" : "x";
        }

        if (key == Keys.Y)
        {
            return flag ? "Y" : "y";
        }

        if (key == Keys.Z)
        {
            return flag ? "Z" : "z";
        }

        if ((key == Keys.D0 && !flag) || key == Keys.NumPad0)
        {
            return "0";
        }

        if ((key == Keys.D1 && !flag) || key == Keys.NumPad1)
        {
            return "1";
        }

        if ((key == Keys.D2 && !flag) || key == Keys.NumPad2)
        {
            return "2";
        }

        if ((key == Keys.D3 && !flag) || key == Keys.NumPad3)
        {
            return "3";
        }

        if ((key == Keys.D4 && !flag) || key == Keys.NumPad4)
        {
            return "4";
        }

        if ((key == Keys.D5 && !flag) || key == Keys.NumPad5)
        {
            return "5";
        }

        if ((key == Keys.D6 && !flag) || key == Keys.NumPad6)
        {
            return "6";
        }

        if ((key == Keys.D7 && !flag) || key == Keys.NumPad7)
        {
            return "7";
        }

        if ((key == Keys.D8 && !flag) || key == Keys.NumPad8)
        {
            return "8";
        }

        if ((key == Keys.D9 && !flag) || key == Keys.NumPad9)
        {
            return "9";
        }

        if (key == Keys.D0 && flag)
        {
            return ")";
        }

        if (key == Keys.D1 && flag)
        {
            return "!";
        }

        if (key == Keys.D2 && flag)
        {
            return "@";
        }

        if (key == Keys.D3 && flag)
        {
            return "#";
        }

        if (key == Keys.D4 && flag)
        {
            return "$";
        }

        if (key == Keys.D5 && flag)
        {
            return "%";
        }

        if (key == Keys.D6 && flag)
        {
            return "^";
        }

        if (key == Keys.D7 && flag)
        {
            return "&";
        }

        if (key == Keys.D8 && flag)
        {
            return "*";
        }

        if (key == Keys.D9 && flag)
        {
            return "(";
        }

        switch (key)
        {
            case Keys.Space:
                return "SPACE";
            case Keys.Tab:
                return "TAB";
            case Keys.Enter:
                return "ENTER";
            case Keys.Escape:
                return "ESC";
            case Keys.Back:
                return "BACKSPACE";
            case Keys.Add:
                return "+";
            case Keys.Decimal:
                return ".";
            case Keys.Divide:
                return "/";
            case Keys.Multiply:
                return "*";
            case Keys.OemBackslash:
                return "\\";
            case Keys.OemComma:
                if (!flag)
                {
                    return ",";
                }

                break;
        }

        if (key == Keys.OemComma && flag)
        {
            return "<";
        }

        if (key == Keys.OemOpenBrackets && !flag)
        {
            return "[";
        }

        if (key == Keys.OemOpenBrackets && flag)
        {
            return "{";
        }

        if (key == Keys.OemCloseBrackets && !flag)
        {
            return "]";
        }

        if (key == Keys.OemCloseBrackets && flag)
        {
            return "}";
        }

        if (key == Keys.OemPeriod && !flag)
        {
            return ".";
        }

        if (key == Keys.OemPeriod && flag)
        {
            return ">";
        }

        if (key == Keys.OemPipe && !flag)
        {
            return "\\";
        }

        if (key == Keys.OemPipe && flag)
        {
            return "|";
        }

        if (key == Keys.OemPlus && !flag)
        {
            return "=";
        }

        if (key == Keys.OemPlus && flag)
        {
            return "+";
        }

        if (key == Keys.OemMinus && !flag)
        {
            return "-";
        }

        if (key == Keys.OemMinus && flag)
        {
            return "_";
        }

        if (key == Keys.OemQuestion && !flag)
        {
            return "/";
        }

        if (key == Keys.OemQuestion && flag)
        {
            return "?";
        }

        if (key == Keys.OemQuotes && !flag)
        {
            return "\"";
        }

        if (key == Keys.OemQuotes && flag)
        {
            return "\"";
        }

        if (key == Keys.OemSemicolon && !flag)
        {
            return ";";
        }

        if (key == Keys.OemSemicolon && flag)
        {
            return ":";
        }

        if (key == Keys.OemTilde && !flag)
        {
            return "`";
        }

        if (key == Keys.OemTilde && flag)
        {
            return "~";
        }

        if (key == Keys.Subtract)
        {
            return "-";
        }

        return null;
    }
}
