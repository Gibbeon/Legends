using System;
using Microsoft.Xna.Framework;
using Legends.Engine;
using Legends.Engine.Input;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using Legends.App.Screens;

namespace Legends.Scripts;

public class LaunchGameBehavior : BaseBehavior
{
    public float ScrollSpeed { get; set; }
    private InputCommandSet _commands;

    public LaunchGameBehavior(): this(null, null)
    {

    }
    public LaunchGameBehavior(IServiceProvider services, SceneObject parent) : base(services, parent)
    {
        ScrollSpeed = 1;

        if(services != null)
        {
            _commands = new InputCommandSet(Services, services.Get<IInputHandlerService>().Current);
            _commands.Add("START",    EventType.KeyPressed,    Keys.Enter);
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
                    case "START":   
                        Services.Get<IGameManagementService>().ScreenManager.LoadScreen(new MapScreen(Services), new FadeTransition(Services.GetGraphicsDevice(), Color.Black));
                        _commands.Enabled = false;
                    break;    
                }
            }  
        }
    }
}