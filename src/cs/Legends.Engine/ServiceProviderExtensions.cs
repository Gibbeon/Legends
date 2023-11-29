using System;
using System.ComponentModel.Design;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Legends.Engine;

public interface IGameManagementService
{
    ContentManager Content { get; }
}

public class GameManagementService : IGameManagementService
{
    public Game Game { get; protected set; }
    
    public ContentManager Content => Game.Content;

    public GameManagementService(Game game)
    {
        Game = game;
        Game.Services.Add<IGameManagementService>(this);
    }
}

public static class ServiceProviderExtensions
{
    public static GraphicsDevice GetGraphicsDevice(this IServiceProvider serviceProvider)
    {
        return serviceProvider.Get<IGraphicsDeviceService>().GraphicsDevice;
    }
    
    public static ContentManager GetContentManager(this IServiceProvider serviceProvider)
    {
        return serviceProvider.Get<IGameManagementService>().Content;
    }

    public static TType Get<TType>(this IServiceProvider serviceProvider)
        where TType: class
    {
        return (serviceProvider.GetService(typeof(TType)) as TType) ?? throw new InvalidOperationException(string.Format("Could not service of type {0}.", typeof(TType).Name));
    }

    public static void Add<TType>(this IServiceProvider serviceProvider, TType value)
        where TType: class
    {
        if(serviceProvider is GameServiceContainer serviceContainer)
        {
            serviceContainer.AddService(typeof(TType), value);
        } 
        else
        {
            throw new InvalidOperationException(string.Format("Could not register service of type: {0}", typeof(TType).Name));
        }
    }

    /*
    private Game _game;

    internal Game Game => _game;

    public GameServiceContainer Services => _game.Services;
    public ContentManager Content => _game.Content;
    public GameComponentCollection Components => _game.Components;
    //public GamePlatform Platform => _game.Platform;
    public GameWindow Window => _game.Window;

    public GraphicsDevice GraphicsDevice => _game.Services.GetService<IGraphicsDeviceService>().GraphicsDevice;

    public IServiceProviderExtensions(Game game)
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
    */
}
