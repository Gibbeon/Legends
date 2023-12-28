using System;
using System.ComponentModel.Design;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Screens;

namespace Legends.Engine;

public interface IGameManagementService
{
    ContentManager Content { get; }
    ScreenManager ScreenManager { get; }
}

public class GameManagementService : IGameManagementService
{
    public Game Game { get; protected set; }
    public ScreenManager ScreenManager { get; protected set; }
    
    public ContentManager Content => Game.Content;

    public GameManagementService(Game game, ScreenManager screenManager)
    {
        Game = game;
        ScreenManager = screenManager;
        Game.Services.Add<IGameManagementService>(this);
    }
}