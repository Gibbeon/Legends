using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.BitmapFonts;
using Legends.App.Graphics;
using Legends.App.Input;
using MonoGame.Extended.Input.InputListeners;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Legends.App.Screens
{
    public class MapScreen : Screen
    {
        private Game _game;
        
        private Canvas2D _canvas;

        private DrawableEntity2D _entity;
        
        private InputManager _input;

        public MapScreen(Game game)
        {
            _game = game;
            _canvas  = new Canvas2D(game);

            _entity = new DrawableEntity2D(game);
            _entity.Spatial.Position = new Vector2(256, 256);
            _entity.Spatial.OriginNormalized = new Vector2(.5f, .5f); 

            //_entity.Spatial.Scale = new Vector2(.5f, .5f);

            _canvas.Add().Add(_entity);

            _input = new InputManager(new KeyboardListenerSettings()
            {
                InitialDelayMilliseconds = 0
            });

            _input.Register("EXIT",     Keys.Escape);
            _input.Register("LOOK_AT",  MouseButton.Right);  
            
            _input.Register("MOVE_LEFT",    Keys.Left);             
            _input.Register("MOVE_RIGHT",   Keys.Right);      
            _input.Register("MOVE_UP",      Keys.Up);             
            _input.Register("MOVE_DOWN",    Keys.Down); 

            _entity.Spatial.Rotate(MathF.PI/4);
        }

        public override void Draw(GameTime gameTime)
        {
            _canvas.Draw(gameTime);

            SpriteBatch batch = new SpriteBatch(_game.GraphicsDevice);
            
            batch.Begin();
            batch.DrawString(Global.Fonts.Menu, string.Format("Camera Position: {0:N0}, {1:N0} Center: {2:N0}, {3:N0}", _canvas.Camera.Position.X, _canvas.Camera.Position.Y, _canvas.Camera.Center.X, _canvas.Camera.Center.Y), Vector2.Zero, Color.White);
            batch.DrawString(Global.Fonts.Menu, string.Format("Mouse Position Screen {0:N0}, {1:N0}", Mouse.GetState().Position.X,Mouse.GetState().Position.Y), new Vector2(0, 18), Color.White);
            batch.DrawString(Global.Fonts.Menu, string.Format("Mouse Position World {0:N0}, {1:N0}", _canvas.Camera.ScreenToWorld(Mouse.GetState().Position.ToVector2()).X,_canvas.Camera.ScreenToWorld(Mouse.GetState().Position.ToVector2()).Y), new Vector2(0, 36), Color.White);  
            batch.DrawString(Global.Fonts.Menu, string.Format("Entity Position World {0:N0}, {1:N0}", _entity.Spatial.Position.X, _entity.Spatial.Position.Y), new Vector2(0, 36+18), Color.White);
            batch.End(); 
        }

        public override void Update(GameTime gameTime)
        {        
            _input.Update(gameTime);

            foreach(var result in _input.Results)
            {
                switch(result.Command)
                {
                    case "EXIT":        _game.Exit(); break;
                    case "LOOK_AT":     _canvas.Camera.LookAt(_canvas.Camera.ScreenToWorld((Vector2)result.GetPosition())); break;

                    case "MOVE_LEFT":   _entity.Spatial.Move(-1, 0); break;
                    case "MOVE_RIGHT":  _entity.Spatial.Move( 1, 0); break;
                    case "MOVE_UP":     _entity.Spatial.Move( 0,-1); break;
                    case "MOVE_DOWN":   _entity.Spatial.Move( 0, 1); break;
                }
            }    

            if(_entity.Spatial.Contains(_canvas.Camera.ScreenToWorld(Mouse.GetState().Position.ToVector2())))
            {
                _entity.Material.AmbientColor = Color.DarkGray;          
            }
            else
            {
                _entity.Material.AmbientColor = Color.White;
            }

            _canvas.Update(gameTime);   
        }
    }
}