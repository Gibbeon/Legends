using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Input;
using Legends.Engine.Input;
using Legends.Engine;

namespace Legends.App.Screens
{
    public class TitleScreen : Screen
    {
        private SystemServices _services;
        private SpriteBatch _spriteBatch;
        private InputManager _input;

        public TitleScreen(SystemServices services)
        {
            _services = services;
            //_spriteBatch = new SpriteBatch(_services.GraphicsDevice);
            _input = new InputManager();

            _input.Register("EXIT",     Keys.Escape);
            _input.Register("EXIT",     MouseButton.Right);

            _input.Register("START",    Keys.Enter);
            _input.Register("START",    MouseButton.Left);
        }

        public override void Draw(GameTime gameTime)
        {
            _services.GraphicsDevice.Clear(Color.Black);

            //_spriteBatch.Begin();

            //_spriteBatch.DrawString(Global.Fonts.Menu, string.Format("Press {0} to Exit. Press {1} to Start", _input.GetText("EXIT"), _input.GetText("START")), Vector2.Zero, Color.White);

            //_spriteBatch.End();            
        }

        public override void Update(GameTime gameTime)
        {
            _input.Update(gameTime);

            foreach(var result in _input.Results)
            {
                switch(result.Command)
                {
                    case "EXIT": _services.Exit(); break;
                    case "START": Start(); break;
                }
            }            
        }

        protected void Start()
        {
            ScreenManager.LoadScreen(new MapScreen(_services), new MonoGame.Extended.Screens.Transitions.FadeTransition(_services.GraphicsDevice, Color.Black));
        }
    }
}