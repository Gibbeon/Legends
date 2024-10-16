using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;
using Legends.Engine.Input;
using Legends.Engine;
using System;
using Legends.Engine.Content;

namespace Legends.Editor.Screens;

public class ViewerScreen : Screen
{
    private readonly IServiceProvider _services;
    private readonly Ref<Scene> _scene;
    private readonly InputManager _input;
    public ViewerScreen(IServiceProvider services)
    {
        _services = services;
        _input = new InputManager(_services);
        _scene = new Scene(_services);

        var so = _services.GetContentManager().GetRef<SceneObject>("Sprites/Fence");
        (~_scene).AttachChild(so);
    }

    public override void Initialize()
    {
        (~_scene).Initialize();
    }

    public override void Draw(GameTime gameTime)
    { 
        (~_scene).Draw(gameTime);       
    }

    public override void Update(GameTime gameTime)
    {
        (~_scene).Update(gameTime);
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);

        (~_scene).Dispose();
        _input.Dispose();

        base.Dispose();
    }
}