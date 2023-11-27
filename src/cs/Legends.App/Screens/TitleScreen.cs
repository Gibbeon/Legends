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
        private Scene _scene;
        private SceneObject _text;
        private InputCommandSet _commands;
        private InputManager _input;
        public TitleScreen(SystemServices services)
        {
            _services = services;
            _input = new InputManager(_services);

            _commands = new InputCommandSet(_services);
            _commands.Add("EXIT", EventType.KeyTyped, Keys.Escape);
            _commands.Add("EXIT", EventType.MouseClicked, MouseButton.Right);

            _commands.Add("START", EventType.KeyTyped, Keys.Enter);
            _commands.Add("START", EventType.MouseClicked, MouseButton.Left);
            
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
            foreach(var action in _commands.EventActions)
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
            _input.Enabled = false;
            ScreenManager.LoadScreen(new MapScreen(_services), new MonoGame.Extended.Screens.Transitions.FadeTransition(_services.GraphicsDevice, Color.Black));
        }

        public override void Dispose()
        {
            _scene.Dispose();
            _input.Dispose();

            base.Dispose();
        }
    }
}