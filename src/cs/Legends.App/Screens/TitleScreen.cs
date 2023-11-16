using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Screens;

namespace Legends.App.Screens
{
    public class TitleScreen : Screen
    {
        private Game _game;
        private SpriteBatch _spriteBatch;

        public TitleScreen(Game game)
        {
            _game = game;
            _spriteBatch = new SpriteBatch(_game.GraphicsDevice);
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                _game.Exit();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Enter))
                this.ScreenManager.LoadScreen(new MapScreen(_game), new MonoGame.Extended.Screens.Transitions.FadeTransition(_game.GraphicsDevice, Color.Black));
        }
    }
}