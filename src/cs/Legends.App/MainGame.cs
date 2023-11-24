using Legends.Engine;
using Legends.Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;

namespace Legends.App;

public class MainGame : Microsoft.Xna.Framework.Game
{
    private SystemServices _services;
    private GraphicsDeviceService _graphicsDeviceManager;
    private RenderService _spriteRenderService;

    private InputHandlerService _inputService;
    private ScreenManager _screenManager;
    
    public MainGame()
    {
        _services = new SystemServices(this);

        _graphicsDeviceManager  = new GraphicsDeviceService(_services);        
        _spriteRenderService    = new RenderService(_services);
        _inputService           = new InputHandlerService(_services);

        _screenManager = new ScreenManager();
        Content.RootDirectory = "Content";
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
        _screenManager.LoadScreen(new Screens.TitleScreen(_services));
    }

    protected override void Update(GameTime gameTime)
    {
        _inputService.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        _spriteRenderService.Draw(gameTime);
    }
}