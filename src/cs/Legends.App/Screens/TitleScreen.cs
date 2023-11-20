using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Input;
using SlyEngine.Input;
using MonoGame.Extended;
using System;
using MonoGame.Extended.Graphics;

namespace Legends.App.Screens
{
    public class TitleScreen : Screen
    {
        private Microsoft.Xna.Framework.Game _game;
        private SpriteBatch _spriteBatch;
        private InputManager _input;

        public TitleScreen(Microsoft.Xna.Framework.Game game)
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

            //_spriteBatch.Begin(blendState: BlendState.NonPremultiplied, samplerState: SamplerState.LinearClamp, depthStencilState: DepthStencilState.Default, rasterizerState: RasterizerState.CullNone, Eff
            //_spriteBatch.Draw()
            
            

            /*
            SpriteEffect
            AlphaTestEffect
            DualTextureEffect
            EnvironmentMapEffect
            SkinnedEffect
            SpriteEffect
            BasicEffect
            */

            /*var effect = new BasicEffect(_game.GraphicsDevice) { 
                TextureEnabled = true,
                VertexColorEnabled = true
            };
            
            IEffectMatrices? effect2 = (effect as IEffectMatrices);

            effect2.View         = _camera.View;
            //effect.View         = Matrix.Identity; //camera.View;
            effect2.Projection   = _camera.Projection;
            //effect.Projection   = Matrix.Identity; //camera.View;
            effect2.World        = _camera.World;

            _spriteBatch.Begin(effect: effect);

            _spriteBatch.DrawRectangle(
                _camera.BoundingRectangle, 
                Color.Red);
                
            _spriteBatch.DrawRectangle(
                _parent.BoundingRectangle, 
                Color.Blue);

            _spriteBatch.DrawRectangle(
                _entity.BoundingRectangle, 
                Color.Green);
                */

               //_spriteBatch.Draw()


            //_spriteBatch.DrawString(Global.Fonts.Menu, "Press ESC to exit or ENTER to move forward", Vector2.Zero, Color.White);
            //_spriteBatch.End(); 
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
            //ScreenManager.LoadScreen(new MapScreen(_game), new MonoGame.Extended.Screens.Transitions.FadeTransition(_game.GraphicsDevice, Color.Black));
        }
    }
}