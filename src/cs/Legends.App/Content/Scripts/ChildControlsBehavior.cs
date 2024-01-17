using System;
using Microsoft.Xna.Framework;
using Legends.Engine;
using Legends.Engine.Input;
using Microsoft.Xna.Framework.Input;

namespace Legends.Scripts;

public class ChildControlsBehavior : Behavior
{
    public float ScrollSpeed { get; set; }
    public float ZoomFactor { get; set; }
    public float RotateFactor { get; set; }
    private InputCommandSet _commands;

    public ChildControlsBehavior(): this(null, null)
    {
    }
    
    public ChildControlsBehavior(IServiceProvider services, SceneObject parent) : base(services, parent)
    {
        ScrollSpeed = 1;
        ZoomFactor = 2;
        RotateFactor = MathF.PI / 12;
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
        _commands?.Dispose();
    }
    public override void Update(GameTime gameTime)
    {
        foreach(var command in _commands.EventActions)
        {
            switch(command.Name)
            {
                case "MOVE_LEFT":   Parent.Move(-ScrollSpeed, 0); break;
                case "MOVE_RIGHT":  Parent.Move( ScrollSpeed, 0); break;
                case "MOVE_UP":     Parent.Move( 0,-ScrollSpeed); break;
                case "MOVE_DOWN":   Parent.Move( 0, ScrollSpeed); break;
                case "ZOOM_IN":     Parent.Zoom(1 / ZoomFactor); break;
                case "ZOOM_OUT":    Parent.Zoom(    ZoomFactor); break;
                case "ROTATE":      Parent.RotateByDegrees(RotateFactor); break;
                default:
                    Console.WriteLine("Unknown Command: {0}", command.Name); break;             
            }
        }  
    }

    public override void Initialize()
    {
        _commands = new InputCommandSet(Services, Services.Get<IInputHandlerService>().Current);

        _commands.Add("MOVE_LEFT",    EventType.KeyPressed,    Keys.Left);             
        _commands.Add("MOVE_RIGHT",   EventType.KeyPressed,    Keys.Right);      
        _commands.Add("MOVE_UP",      EventType.KeyPressed,    Keys.Up);             
        _commands.Add("MOVE_DOWN",    EventType.KeyPressed,    Keys.Down);
        
        _commands.Add("ZOOM_IN",      EventType.KeyPressed,     Keys.T);           
        _commands.Add("ZOOM_OUT",     EventType.KeyPressed,    Keys.Y);             
        _commands.Add("ROTATE",       EventType.KeyPressed,        Keys.R);
        
        _commands.Enabled = true;
    }

    public override void Reset()
    {
        
    }
}