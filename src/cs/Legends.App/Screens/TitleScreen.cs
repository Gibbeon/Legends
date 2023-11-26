using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Input;
using Legends.Engine.Input;
using Legends.Engine;
using Legends.Engine.Graphics2D;
using Autofac.Core;

namespace Legends.App.Screens
{
    public class TitleScreen : Screen
    {
        private SystemServices _services;

        private Scene _scene;
        private SceneObject _text;

        public TitleScreen(SystemServices services)
        {
            _services = services;
            var input = new InputManager(_services);

            input.Register("EXIT",     Keys.Escape);
            input.Register("EXIT",     MouseButton.Right);

            input.Register("START",    Keys.Enter);
            input.Register("START",    MouseButton.Left);

            _services.GetService<IInputHandlerService>().Push(input);
            
            _scene = new Scene(_services);

            _text = new SceneObject(_services, _scene);
            _text.AttachBehavior(new TextRenderBehavior(_text)
            {
                Font = services.Content.Load<BitmapFont>("Sensation"),
                Text = "Press ESC to Exit or ENTER to Start",
                Color = Color.White
            }); 
        }

        public override void Draw(GameTime gameTime)
        { 
            _scene.Draw(gameTime);       
        }

        public override void Update(GameTime gameTime)
        {
            foreach(var action in _services.GetService<IInputHandlerService>().Current.EventActions)
            {
                switch(action.Name)
                {
                    case "EXIT": _services.Exit(); break;
                    case "START": Start(); break;
                }
            }

            _scene.Update(gameTime);
        }

        protected void Start()
        {
            _services.GetService<IInputHandlerService>().Pop();
            _text.Dispose();
            ScreenManager.LoadScreen(new MapScreen(_services), new MonoGame.Extended.Screens.Transitions.FadeTransition(_services.GraphicsDevice, Color.Black));
        }
    }
}