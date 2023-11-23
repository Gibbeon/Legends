using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.Sprites;
using System;
using System.Net.Http.Headers;
using System.Xml.Linq;
using MonoGame.Extended;
using MonoGame.Framework.Utilities;
using System.Runtime.InteropServices;

namespace Legends.Engine;

public class SystemServices
{
    private Game _game;

    internal Game Game => _game;

    public GameServiceContainer Services => _game.Services;
    public ContentManager Content => _game.Content;
    public GameComponentCollection Components => _game.Components;
    //public GamePlatform Platform => _game.Platform;
    public GameWindow Window => _game.Window;

    public GraphicsDevice GraphicsDevice => _game.Services.GetService<IGraphicsDeviceService>().GraphicsDevice;

    public SystemServices(Game game)
    {
        _game = game;
    }

    public TType GetService<TType>()
        where TType : class
    {
        return _game.Services.GetService<TType>();
    }

    public void Exit()
    {
        _game.Exit();
    }
}
