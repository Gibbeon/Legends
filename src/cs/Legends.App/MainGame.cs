using Legends.Engine;
using Legends.Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;

namespace Legends.App;

public class MainGame : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager _graphicsDeviceManager;
    private RenderService _spriteRenderService;

    private InputHandlerService _inputService;
    private ScreenManager _screenManager;
    
    public MainGame()
    {
        _graphicsDeviceManager  = new GraphicsDeviceManager(this);        
        _spriteRenderService    = new RenderService(Services);
        _inputService           = new InputHandlerService(Services);

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
        _screenManager.LoadScreen(new Screens.TitleScreen(Services));
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