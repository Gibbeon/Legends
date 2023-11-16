using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Screens;
using Legends.App.Graphics;

namespace Legends.App.Screens
{
    public class MapScreen : Screen
    {
        private Game _game;
        private SpriteBatch _spriteBatch;

        private DrawableEntity _entity;

        public MapScreen(Game game)
        {
            _game = game;
            _spriteBatch = new SpriteBatch(_game.GraphicsDevice);

            _entity = new DrawableEntity(game);
            _entity.Spatial.Position = new Vector2(64, 64);
            _entity.Spatial.OriginNormalized = new Vector2(.5f, .5f); 
        }

        public override void Draw(GameTime gameTime)
        {
            _game.GraphicsDevice.Clear(Color.Black);
            
            _spriteBatch.Begin();
            _entity.DrawBatched(_spriteBatch, new Camera2D(), gameTime);
            _spriteBatch.DrawString(Global.Fonts.Menu, "Press ESC to exit", Vector2.Zero, Color.White);
            _spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            _entity.Spatial.Rotation += gameTime.GetElapsedSeconds();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                _game.Exit();

        }
    }
}