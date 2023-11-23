using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Input;
using Legends.Engine.Input;
using Legends.Engine;
using Legends.Engine.Graphics2D;

namespace Legends.App.Screens
{
    public class TitleScreen : Screen
    {
        private SystemServices _services;
        private SpriteBatch _spriteBatch;
        private InputManager _input;

        private GameObject _text;

        public TitleScreen(SystemServices services)
        {
            _services = services;
            //_spriteBatch = new SpriteBatch(_services.GraphicsDevice);
            _input = new InputManager();

            _input.Register("EXIT",     Keys.Escape);
            _input.Register("EXIT",     MouseButton.Right);

            _input.Register("START",    Keys.Enter);
            _input.Register("START",    MouseButton.Left);

            _text = new GameObject(_services);
            _text.AttachBehavior(new TextRenderBehavior(_text)
            {
                Font = services.Content.Load<BitmapFont>("Sensation"),
                Text = "Press ESC to Exit or ENTER to Start",
                Color = Color.White
            }); 
        }

        public override void Draw(GameTime gameTime)
        {
                       
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

            _text.Update(gameTime);
        }

        protected void Start()
        {
            _text.Dispose();
            ScreenManager.LoadScreen(new MapScreen(_services), new MonoGame.Extended.Screens.Transitions.FadeTransition(_services.GraphicsDevice, Color.Black));
        }
    }
}