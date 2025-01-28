using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;
using Legends.Engine.Input;
using Legends.Engine;
using System;
using Legends.Engine.Content;
using Microsoft.Xna.Framework.Input;
using System.Xml.Linq;

namespace Legends.Editor.Screens;

public class ViewerScreen : Screen
{
    public float ScrollSpeed { get; set; }
    public float ZoomFactor { get; set; }
    public float RotateFactor { get; set; }

    private readonly IServiceProvider _services;
    private readonly Scene _scene;
    private readonly InputManager _input;
    private InputCommandSet _commands;
    public ViewerScreen(IServiceProvider services)
    {
        _services = services;
        _input = new InputManager(services);
        _scene = new Scene(_services);
        _scene.Initialize();

        var so = _services.GetContentManager().Load<SceneObject>("Sprites/Actor");
        _scene.AttachChild(so);

        ScrollSpeed = 1;
        ZoomFactor = 2;
        RotateFactor = MathF.PI / 12;
    }

    public override void Initialize()
    {
        _scene.Initialize();

        _commands = new InputCommandSet(_services, _input);

        _commands.Add("MOVE_LEFT",    EventType.KeyPressed,    Keys.Left);             
        _commands.Add("MOVE_RIGHT",   EventType.KeyPressed,    Keys.Right);      
        _commands.Add("MOVE_UP",      EventType.KeyPressed,    Keys.Up);             
        _commands.Add("MOVE_DOWN",    EventType.KeyPressed,    Keys.Down);
        
        _commands.Add("ZOOM_IN",      EventType.KeyPressed,    Keys.T);           
        _commands.Add("ZOOM_OUT",     EventType.KeyPressed,    Keys.Y);             
        _commands.Add("ROTATE",       EventType.KeyPressed,    Keys.R);
        
        _commands.Enabled = true;
    }

    public override void Draw(GameTime gameTime)
    { 
        _scene.Draw(gameTime);       
    }

    public override void Update(GameTime gameTime)
    {
        foreach(var command in _commands.EventActions)
        {
            switch(command.Name)
            {
                case "MOVE_LEFT":   _scene.Camera.Move(-ScrollSpeed, 0); break;
                case "MOVE_RIGHT":  _scene.Camera.Move( ScrollSpeed, 0); break;
                case "MOVE_UP":     _scene.Camera.Move( 0,-ScrollSpeed); break;
                case "MOVE_DOWN":   _scene.Camera.Move( 0, ScrollSpeed); break;
                case "ZOOM_IN":     _scene.Camera.Zoom(1 / ZoomFactor); break;
                case "ZOOM_OUT":    _scene.Camera.Zoom(    ZoomFactor); break;
                case "ROTATE":      _scene.Camera.RotateByDegrees(RotateFactor); break;
                default:
                    Console.WriteLine("Unknown Command: {0}", command.Name); break;             
            }
        }  

        _scene.Update(gameTime);
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);

        _scene.Dispose();

        base.Dispose();
    }
}