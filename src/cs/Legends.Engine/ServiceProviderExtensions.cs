using System;
using System.ComponentModel.Design;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Screens;

namespace Legends.Engine;

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
}
