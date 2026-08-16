using System;
using System.IO;
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

        _graphicsDeviceManager.PreferredBackBufferWidth = 1280;
        _graphicsDeviceManager.PreferredBackBufferHeight = 1024;

        // The editor reads the game's compiled content. Resolve it relative to this assembly rather
        // than from absolute paths, which were machine-specific and left Linux/FreeBSD on #error.
        Content.RootDirectory = ResolveGameContentDirectory();
        Content.EnableAssetWatching();

        IsMouseVisible = true;
    }

    // .../Legends.Editor/bin/<Config>/<Tfm>/  ->  .../Legends.App/bin/<Config>/<Tfm>/Content
    // Swapping the project name keeps the configuration and target framework in step automatically.
    private static string ResolveGameContentDirectory()
    {
        var editorDirectory = AppContext.BaseDirectory;
        var separator       = Path.DirectorySeparatorChar;

        var gameContent = Path.GetFullPath(Path.Combine(
            editorDirectory.Replace(
                string.Concat(separator, "Legends.Editor", separator),
                string.Concat(separator, "Legends.App",    separator)),
            "Content"));

        return Directory.Exists(gameContent)
            ? gameContent
            : Path.GetFullPath(Path.Combine(editorDirectory, "Content"));
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