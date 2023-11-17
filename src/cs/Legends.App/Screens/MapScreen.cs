using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended;
using Legends.App.Graphics;
using Legends.App.Input;
using MonoGame.Extended.Input.InputListeners;

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
            _entity.Spatial.Position = new Vector2(64, 64);
            _entity.Spatial.OriginNormalized = new Vector2(.5f, .5f); 

            _canvas.Add().Add(_entity);

            _input = new InputManager(new KeyboardListenerSettings()
            {
                InitialDelayMilliseconds = 0
            });

            _input.Register("EXIT",     Keys.Escape);
            _input.Register("EXIT",     MouseButton.Right);
            
            _input.Register("LOOK_AT",  MouseButton.Left);  
            
            _input.Register("MOVE_LEFT",    Keys.Left);             
            _input.Register("MOVE_RIGHT",   Keys.Right);      
            
            _input.Register("MOVE_UP",      Keys.Up);             
            _input.Register("MOVE_DOWN",    Keys.Down); 
        }

        public override void Draw(GameTime gameTime)
        {
            _canvas.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {        
            _input.Update(gameTime);

            foreach(var result in _input.Results)
            {
                switch(result.Command)
                {
                    case "EXIT":        _game.Exit(); break;
                    case "LOOK_AT":     _canvas.Camera.Position  = (Vector2)result.GetPosition(); break;

                    case "MOVE_LEFT":   _entity.Spatial.Move(-1, 0); break;
                    case "MOVE_RIGHT":  _entity.Spatial.Move( 1, 0); break;
                    case "MOVE_UP":     _entity.Spatial.Move( 0,-1); break;
                    case "MOVE_DOWN":   _entity.Spatial.Move( 0, 1); break;
                }
            }    

            _canvas.Update(gameTime);   
        }
    }
}