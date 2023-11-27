using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using System.Dynamic;

namespace Legends.Engine.Input;

public class InputManager
{
    private readonly KeyboardListener         _keyboardListener;
    private readonly MouseListener            _mouseListener;
    private readonly KeyboardListenerSettings _keyboardListenerSettings;
    private readonly MouseListenerSettings    _mouseListenerSettings;
    public KeyboardListener KeyboardListener => _keyboardListener;
    public MouseListener MouseListener => _mouseListener;
    public IInputHandlerService InputHandlerService => Services.GetService<IInputHandlerService>();
    public SystemServices Services { get; private set; }
    public IList<InputCommandSet> CommandSets { get; private set; }
    public bool Enabled { get; set; }
    public InputManager(SystemServices services, KeyboardListenerSettings keyboardListenerSettings) : this(services, keyboardListenerSettings, new MouseListenerSettings())
    {

    }
    public InputManager(SystemServices services) : this(services, new KeyboardListenerSettings(), new MouseListenerSettings())
    {
        
    }
    public InputManager(SystemServices services, KeyboardListenerSettings keyboardListenerSettings, MouseListenerSettings mouseListenerSettings)
    {
        Services = services;
        CommandSets = new List<InputCommandSet>();
        _keyboardListenerSettings = keyboardListenerSettings;
        _mouseListenerSettings = mouseListenerSettings;
        
        _keyboardListener = new KeyboardListener(_keyboardListenerSettings);
        _mouseListener = new MouseListener(_mouseListenerSettings); 

        services.GetService<IInputHandlerService>().Push(this);
    }

    public void Activate()
    {
        _keyboardListener.KeyPressed += (sender, args)      => ProcessEvent(EventType.KeyPressed, args);
        _keyboardListener.KeyReleased += (sender, args)     => ProcessEvent(EventType.KeyReleased, args);
        _keyboardListener.KeyTyped += (sender, args)        => ProcessEvent(EventType.KeyTyped, args);

        //_mouseListener.MouseClicked += (sender, args)       => ProcessEvent(EventType.MouseClicked, args);
        //_mouseListener.MouseDoubleClicked += (sender, args) => ProcessEvent(EventType.MouseClicked, args);
        //_mouseListener.MouseWheelMoved+= (sender, args)     => ProcessEvent(EventType.MouseScroll, args);
        //_mouseListener.MouseMoved += (sender, args)         => ProcessEvent(EventType.MouseMove, args);

        Enabled = true;
    }

    public void Deactivate()
    {
        foreach(var set in CommandSets)
            foreach(var item in set.EventActions)
                item.Handled = true;

        Enabled = false;

        _keyboardListener.KeyPressed -= (sender, args)      => ProcessEvent(EventType.KeyPressed, args);
        _keyboardListener.KeyReleased -= (sender, args)     => ProcessEvent(EventType.KeyReleased, args);
        _keyboardListener.KeyTyped -= (sender, args)        => ProcessEvent(EventType.KeyTyped, args);

        //_mouseListener.MouseClicked -= (sender, args)       => ProcessEvent(EventType.MouseClicked, args);
        //_mouseListener.MouseDoubleClicked -= (sender, args) => ProcessEvent(EventType.MouseClicked, args);
        //_mouseListener.MouseWheelMoved -= (sender, args)    => ProcessEvent(EventType.MouseScroll, args);
        //_mouseListener.MouseMoved -= (sender, args)         => ProcessEvent(EventType.MouseMove, args);
    }

    public void Update(GameTime gameTime)
    {
        foreach(var set in CommandSets) set.Clear();

        if(Enabled)
        {
            _keyboardListener.Update(gameTime);
            _mouseListener.Update(gameTime);
        }
    }

    protected void ProcessEvent(EventType type, EventArgs args)
    {
        foreach(var listener in CommandSets.Where(n => n.Enabled).SelectMany(n => n.EventListeners))
        {
            if(listener.Eval(type, args))
            {
                listener.CommandSet.AddAction(new EventAction(listener, type, args));
            }
        }
    }

    public void Dispose()
    {
        Services.GetService<IInputHandlerService>().Remove(this);
    }
}

/*
    public string GetTextForTrigger(string actionName)
    {
        string result = "";
        foreach(var cmd in _eventListeners.Where(n => n.Name == actionName))
        {
            result += (!string.IsNullOrEmpty(result) ? " OR " : "") + cmd.Label;
        }

        return result;
    }

    public string? GetInputText(EventType type, Keys key) 
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

    public string? GetInputText(EventType type, MouseButton button) 
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





    
    public bool HasEventFired(string actionName)
    {
        return _eventActions.Where(n => n.EventListener.Name == actionName).Any();
    }

    public bool HasEventFired(string actionName, out Point2 point)
    {
        foreach(var entry in _eventActions.Where(n => n.EventListener.Name == actionName))
        {
            var eventPos = entry.GetPosition();
            if(eventPos.HasValue)
            {
                point = eventPos.Value;
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
*/
