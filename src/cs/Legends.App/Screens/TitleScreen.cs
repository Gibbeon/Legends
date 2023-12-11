using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;
using Legends.Engine.Input;
using Legends.Engine;
using System;
using Legends.Engine.Content;

namespace Legends.App.Screens;

public class TitleScreen : Screen
{
    private readonly IServiceProvider _services;
    private readonly Scene _scene;
    private readonly InputManager _input;
    public TitleScreen(IServiceProvider services)
    {
        _services = services;
        _input = new InputManager(_services);

        _scene = (Scene)_services.GetContentManager().Load<ContentObject>("Scenes/TitleScreen").Instance
    }

    public override void Draw(GameTime gameTime)
    { 
        _scene.Draw(gameTime);       
    }

    public override void Update(GameTime gameTime)
    {
        _scene.Update(gameTime);
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);

        _scene.Dispose();
        _input.Dispose();

        base.Dispose();
    }
}