using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;
using Legends.Engine.Input;
using Legends.Engine;
using System;
using Legends.Engine.Content;
using System.IO;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using SharpFont;
using Microsoft.Xna.Framework.Content;
using System.Text;
using System.Security.AccessControl;
using System.Reflection;
using System.Linq;
using Legends.Engine.Graphics2D;

namespace Legends.App.Screens;

public class TitleScreen : Screen
{
    private readonly IServiceProvider _services;
    private readonly Ref<Scene> _scene;
    private readonly InputManager _input;
    public TitleScreen(IServiceProvider services)
    {
        _services = services;
        _input = new InputManager(_services);

        _scene = _services.GetContentManager().GetRef<Scene>("Scenes/TitleScreen");
        ((Map)(~_scene).Children[0]).CreateMapFromTexture();
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