using System;
using Microsoft.Xna.Framework;
using Legends.Engine;
using Legends.Engine.Input;
using Microsoft.Xna.Framework.Input;

namespace Legends.Scripts;

public class ActorControlBehavior : Behavior
{
    public float MoveSpeed { get; set; }
    private InputCommandSet _commands;

    public ActorControlBehavior(): this(null, null)
    {
        MoveSpeed = 1;
    }
    
    public ActorControlBehavior(IServiceProvider services, SceneObject parent) : base(services, parent)
    {

        if(services != null)
        {
            _commands = new InputCommandSet(Services, services.Get<IInputHandlerService>().Current);

            _commands.Add("MOVE_LEFT",    EventType.KeyPressed,    Keys.Left);             
            _commands.Add("MOVE_RIGHT",   EventType.KeyPressed,    Keys.Right);      
            _commands.Add("MOVE_UP",      EventType.KeyPressed,    Keys.Up);             
            _commands.Add("MOVE_DOWN",    EventType.KeyPressed,    Keys.Down);

            _commands.Enabled = true;
        }
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
        _commands?.Dispose();
    }
    public override void Update(GameTime gameTime)
    {
        if(_commands != null)
        {
            foreach(var command in _commands.EventActions)
            {
                switch(command.Name)
                {
                    case "MOVE_LEFT":   Parent?.Move(-MoveSpeed, 0); break;
                    case "MOVE_RIGHT":  Parent?.Move( MoveSpeed, 0); break;
                    case "MOVE_UP":     Parent?.Move( 0,-MoveSpeed); break;
                    case "MOVE_DOWN":   Parent?.Move( 0, MoveSpeed); break;
                    default:
                        Console.WriteLine("Unknown Command: {0}", command.Name); break;             
                }
            }  
        }
    }
}