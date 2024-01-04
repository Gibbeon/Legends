using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;
using Legends.Engine.Input;
using MonoGame.Extended.Input.InputListeners;
using Legends.Engine;
using System;

namespace Legends.App.Screens;

public class MapScreen : Screen
{
    private Ref<Scene> _scene;
    private IServiceProvider _services;
    private InputManager _input;

    public MapScreen(IServiceProvider services)
    {
        _services = services;
        _input = new InputManager(_services, new KeyboardListenerSettings()
        {
            InitialDelayMilliseconds = 0,
            RepeatDelayMilliseconds = 0,
            RepeatPress = true
        });

        _scene = _services.GetContentManager().GetRef<Scene>("Maps/WorldMap");
    }

    public override void Draw(GameTime gameTime)
    {        
        (~_scene).Draw(gameTime);
    }

    public override void Update(GameTime gameTime)
    {   
        (~_scene).Update(gameTime);
    }
}