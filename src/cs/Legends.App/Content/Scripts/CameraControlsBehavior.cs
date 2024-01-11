using System;
using Microsoft.Xna.Framework;
using Legends.Engine;
using Legends.Engine.Input;
using Microsoft.Xna.Framework.Input;

namespace Legends.Scripts;

public class CameraControlsBehavior : Behavior
{
    public float ScrollSpeed { get; set; }
    private InputCommandSet _commands;

    public CameraControlsBehavior(): this(null, null)
    {
        ScrollSpeed = 1;
    }
    
    public CameraControlsBehavior(IServiceProvider services, SceneObject parent) : base(services, parent)
    {

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
                case "MOVE_LEFT":   Parent.Scene.Camera.Move(-ScrollSpeed, 0); break;
                case "MOVE_RIGHT":  Parent.Scene.Camera.Move( ScrollSpeed, 0); break;
                case "MOVE_UP":     Parent.Scene.Camera.Move( 0,-ScrollSpeed); break;
                case "MOVE_DOWN":   Parent.Scene.Camera.Move( 0, ScrollSpeed); break;
                default:
                    Console.WriteLine("Unknown Command: {0}", command.Name); break;             
            }
        }  
    }

    public override void Initialize()
    {
        _commands = new InputCommandSet(Services, Services.Get<IInputHandlerService>().Current);

        _commands.Add("MOVE_LEFT",    EventType.KeyPressed,    Keys.Left,   Keys.None);             
        _commands.Add("MOVE_RIGHT",   EventType.KeyPressed,    Keys.Right,  Keys.None);      
        _commands.Add("MOVE_UP",      EventType.KeyPressed,    Keys.Up,     Keys.None);             
        _commands.Add("MOVE_DOWN",    EventType.KeyPressed,    Keys.Down,   Keys.None);

        _commands.Enabled = true;
    }

    public override void Reset()
    {
        
    }
}