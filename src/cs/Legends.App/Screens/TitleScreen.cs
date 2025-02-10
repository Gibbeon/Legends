using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;
using Legends.Engine.Input;
using Legends.Engine;
using System;

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

        _scene = _services.GetContentManager().Load<Scene>("Scenes/TitleScreen/Interface");
    }

    public override void Initialize()
    {
        _scene.Initialize();
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