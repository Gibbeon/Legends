using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Input;
using SlyEngine.Input;

namespace Legends.App.Screens
{
    public class TitleScreen : Screen
    {
        private Game _game;
        private SpriteBatch _spriteBatch;

        private InputManager _input;

        private string _string;

        public TitleScreen(Game game)
        {
            _game = game;
            _spriteBatch = new SpriteBatch(_game.GraphicsDevice);
            _input = new InputManager();

            _input.Register("EXIT",     Keys.Escape);
            _input.Register("EXIT",     MouseButton.Right);

            _input.Register("START",    Keys.Enter);
            _input.Register("START",    MouseButton.Left);
        }

        public override void Draw(GameTime gameTime)
        {
            _game.GraphicsDevice.Clear(Color.Black);
            
            _spriteBatch.Begin();
            _spriteBatch.DrawString(Global.Fonts.Menu, "Press ESC to exit or ENTER to move forward", Vector2.Zero, Color.White);
            _spriteBatch.End(); 
        }

        public override void Update(GameTime gameTime)
        {
            _input.Update(gameTime);

            foreach(var result in _input.Results)
            {
                switch(result.Command)
                {
                    case "EXIT": _game.Exit(); break;
                    case "START": Start(); break;
                }
            }            
        }

        protected void Start()
        {
            ScreenManager.LoadScreen(new MapScreen(_game), new MonoGame.Extended.Screens.Transitions.FadeTransition(_game.GraphicsDevice, Color.Black));
        }
    }
}