using Microsoft.Xna.Framework;
using Legends.Engine;
using System;

namespace Legends.App;

public class CameraControlsBehavior : BaseBehavior
{
    public float ScrollSpeed;
    //private InputCommandSet _commands;

    public CameraControlsBehavior(): this(null, null)
    {

    }
    public CameraControlsBehavior(IServiceProvider? services, SceneObject? parent) : base(services, parent)
    {
        ScrollSpeed = 1;
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public override void Update(GameTime gameTime)
    {
        throw new NotImplementedException();
    }

    /*public override void Initialize()
    {
        base.Initialize();

        _commands = new InputCommandSet(Services);
        
        //_commands.Add("LOOK_AT",      EventType.MouseClicked,  MouseButton.Right);  
        //_commands.Add("ZOOM",         EventType.MouseScroll,   MouseButton.Middle); 
        //_commands.Add("ROTATE",       EventType.MousePressed,  MouseButton.Left, Keys.LeftAlt);
        //_commands.Add("ROTATE",       EventType.MousePressed,  MouseButton.Left, Keys.RightAlt);
        //_commands.Add("ROTATE",       EventType.MousePressed,  MouseButton.Middle);
        
        _commands.Add("MOVE_LEFT",    EventType.KeyPressed,    Keys.Left);             
        _commands.Add("MOVE_RIGHT",   EventType.KeyPressed,    Keys.Right);      
        _commands.Add("MOVE_UP",      EventType.KeyPressed,    Keys.Up);             
        _commands.Add("MOVE_DOWN",    EventType.KeyPressed,    Keys.Down);

        _commands.Enabled = true;
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
                    case "MOVE_LEFT":   Parent?.GetParentScene()?.Camera?.Move(-ScrollSpeed, 0); break;
                    case "MOVE_RIGHT":  Parent?.GetParentScene()?.Camera?.Move( ScrollSpeed, 0); break;
                    case "MOVE_UP":     Parent?.GetParentScene()?.Camera?.Move( 0,-ScrollSpeed); break;
                    case "MOVE_DOWN":   Parent?.GetParentScene()?.Camera?.Move( 0, ScrollSpeed); break;
                    default:
                        Console.WriteLine("Unknown Command: {0}", command.Name); break;             
                }
            }  
        }
    }*/
}