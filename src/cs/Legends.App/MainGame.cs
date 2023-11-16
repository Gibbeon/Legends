using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;

namespace Legends.App
{

public class MainGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private ScreenManager _screenManager;
    
    public MainGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        _screenManager = new ScreenManager();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();   

        Components.Add(_screenManager);        
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _screenManager.LoadScreen(new Screens.TitleScreen(this));

        Global.Fonts.LoadContent(this.Content);
        Global.Defaults.LoadContent(this.Content);
    }

    protected override void Update(GameTime gameTime)
    {
        _screenManager.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _screenManager.Draw(gameTime);

        base.Draw(gameTime);
    }
}
}
