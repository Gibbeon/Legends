using Legends.Engine;
using Legends.Engine.Collision;
using Legends.Engine.Content;
using Legends.Engine.Graphics2D;
using Legends.Engine.Input;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;

namespace Legends.Editor;

public class MainGame : Microsoft.Xna.Framework.Game
{
    private readonly GraphicsDeviceManager   _graphicsDeviceManager;
    private readonly RenderService           _spriteRenderService;
    private readonly GameManagementService   _gameManagementService;
    private readonly CollisionService _collisionService;
    private readonly InputHandlerService _inputService;
    private readonly ScreenManager _screenManager;

    
    public MainGame()
    {
        _screenManager = new ScreenManager();

        _gameManagementService  = new GameManagementService(this, _screenManager);
        _graphicsDeviceManager  = new GraphicsDeviceManager(this);        
        _spriteRenderService    = new RenderService(Services);
        _inputService           = new InputHandlerService(Services);
        _collisionService       = new CollisionService(Services);

        ContentLogger.Enabled = true;
        
        //_graphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
        //_gameManagementService.Game.IsFixedTimeStep = false;

        _graphicsDeviceManager.PreferredBackBufferWidth = 1280;
        _graphicsDeviceManager.PreferredBackBufferHeight = 1024;

        #if OS_WINDOWS
            Content.RootDirectory = "/dev/Legends/src/cs/Legends.App/bin/Debug/net8.0/Content";
            Content.EnableAssetWatching();
        #elif OS_MAC
            Content.RootDirectory = "/Users/riwoods/dev/Legends/src/cs/Legends.App/bin/Debug/net8.0/Content";
        #else
            #error "Missing or invalid platform definition."
        #endif
        
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
        _screenManager.LoadScreen(new Screens.ViewerScreen(Services));
        base.LoadContent();
        
    }

    protected override void Update(GameTime gameTime)
    {
        Content.DoReloads();
        _inputService.Update(gameTime);
        base.Update(gameTime);
        _collisionService.Update(gameTime);      
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        _spriteRenderService.Draw(gameTime);
    }
}