using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Screens;
using MonoGame.Extended;
using Legends.App.Graphics;

namespace Legends.App.Screens
{
    public class MapScreen : Screen
    {
        private Game _game;
        //private SpriteBatch _spriteBatch;
        
        private Canvas2D _canvas;

        private DrawableEntity2D _entity;

        public MapScreen(Game game)
        {
            _game = game;
            _canvas  = new Canvas2D(game);
            //_spriteBatch = new SpriteBatch(_game.GraphicsDevice);

            _entity = new DrawableEntity2D(game);
            _entity.Spatial.Position = new Vector2(64, 64);
            _entity.Spatial.OriginNormalized = new Vector2(.5f, .5f); 

            _canvas.Add().Add(_entity);

            //_camera = new Camera2D(game.GraphicsDevice);
            //_camera.LookAt(Vector2.Zero);
        }

        public override void Draw(GameTime gameTime)
        {
            _canvas.Draw(gameTime);
            /*_game.GraphicsDevice.Clear(Color.Black);

            var mtx = _camera.GetViewMatrix();
            
            _spriteBatch.Begin(
                SpriteSortMode.Deferred,
                null, 
                null, 
                null, 
                null, 
                null, 
                mtx                
            );
            _entity.DrawBatched(_spriteBatch, gameTime);
            _spriteBatch.DrawString(Global.Fonts.Menu, string.Format("Camera {0:N1}, {1:N1}", _camera.Position.X, _camera.Position.Y), Vector2.Zero, Color.White);
            _spriteBatch.End();
            */
        }

        public override void Update(GameTime gameTime)
        {        
            _canvas.Update(gameTime);

            _entity.Spatial.Rotation += gameTime.GetElapsedSeconds();   

            _canvas.Camera.Rotation += gameTime.GetElapsedSeconds();       
            _canvas.Camera.Move(-new Vector2(gameTime.GetElapsedSeconds() * 2, gameTime.GetElapsedSeconds() * 2));

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                _game.Exit();
        }
    }
}