﻿using Legends.Engine;
using Legends.Engine.Collision;
using Legends.Engine.Content;
using Legends.Engine.Graphics2D;
using Legends.Engine.Input;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;

namespace Legends.App;

public class MainGame : Microsoft.Xna.Framework.Game
{
    private readonly GraphicsDeviceManager   _graphicsDeviceManager;
    private readonly IRenderService          _spriteRenderService;
    private readonly GameManagementService   _gameManagementService;
    private readonly CollisionService _collisionService;
    private readonly InputHandlerService _inputService;
    private readonly ScreenManager _screenManager;

    
    public MainGame()
    {
        _screenManager = new ScreenManager();

        _gameManagementService  = new GameManagementService(this, _screenManager);
        _graphicsDeviceManager  = new GraphicsDeviceManager(this);        
        _spriteRenderService    = new DefaultRenderService(Services);
        _inputService           = new InputHandlerService(Services);
        _collisionService       = new CollisionService(Services);

        ContentLogger.Enabled = true;

        
        //_graphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
        //_gameManagementService.Game.IsFixedTimeStep = false;


        // default screen resolution
        _graphicsDeviceManager.PreferredBackBufferWidth = 1280;
        _graphicsDeviceManager.PreferredBackBufferHeight = 1024;
        
        

        //ContentLogger.Enabled = true;
        Content.RootDirectory = "Content";
        Content.EnableAssetWatching();
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();   

        _spriteRenderService.Initialize();
        Components.Add(_screenManager);
    }

    protected override void LoadContent()
    {        
        _screenManager.LoadScreen(new Screens.TitleScreen(Services));
        base.LoadContent();
        
    }

    protected override void Update(GameTime gameTime)
    {
        Content.DoReloads();
        _inputService.Update(gameTime);
        base.Update(gameTime);
        _collisionService.Update(gameTime);   
        //_spriteRenderService.Update(gameTime);   
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        _spriteRenderService.Draw(gameTime);
    }
}