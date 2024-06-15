using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;
using Legends.Engine.Input;
using MonoGame.Extended.Input.InputListeners;
using Legends.Engine;
using System;
using Legends.Engine.Content;
using System.Security.Cryptography.X509Certificates;

namespace Legends.App.Screens;

public class MapScreen : Screen
{
    private Ref<Scene>[] _scenes;
    private IServiceProvider _services;
    private InputManager _input;

    public MapScreen(IServiceProvider services)
    {
        _services = services;
        _scenes = new Ref<Scene>[2];
        _scenes[0] = _services.GetContentManager().GetRef<Scene>("Maps/WorldMap");
        _scenes[1] = _services.GetContentManager().GetRef<Scene>("Scenes/HUD/HudScene");
    }

    public override void Initialize()
    {
        base.Initialize();

        _input = new InputManager(_services, new KeyboardListenerSettings()
        {
            InitialDelayMilliseconds = 0,
            RepeatDelayMilliseconds = 0,
            RepeatPress = true
        });

        (~_scenes[0]).Initialize();
        (~_scenes[1]).Initialize();
    }

    public override void Draw(GameTime gameTime)
    {        
        (~_scenes[0]).Draw(gameTime);
        (~_scenes[1]).Draw(gameTime);
    }

    public override void Update(GameTime gameTime)
    {   
        (~_scenes[0]).Update(gameTime);
        (~_scenes[1]).Update(gameTime);
    }
}