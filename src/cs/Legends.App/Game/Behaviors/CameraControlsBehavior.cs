﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Legends.Engine;
using Legends.Engine.Input;
using System;

namespace Legends.App;

public class CameraControlsBehavior : BaseBehavior
{
    public float ScrollSpeed;
    private InputCommandSet _commands;
    public CameraControlsBehavior(IServiceProvider services, SceneObject parent) : base(services, parent)
    {
        ScrollSpeed = 1;

        _commands = new InputCommandSet(services);
        
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
        _commands?.Dispose();
    }
    public override void Update(GameTime gameTime)
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
}