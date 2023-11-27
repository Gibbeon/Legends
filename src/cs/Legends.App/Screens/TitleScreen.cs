using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Input;
using Legends.Engine.Input;
using Legends.Engine;
using Legends.Engine.Graphics2D;
using System.Net.Security;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Legends.App.Screens
{

public class ContentManager2
{
    SystemServices Services { get; set; }

    public ContentManager2(SystemServices services)
    {
        Services = services;
    }

    public TType Load<TType>(string assetName, TType result)
    {
        var innerClass = typeof(TType).GetNestedTypes()[0];
        var loadMethod = typeof(ContentManager).GetMethods().Single(n => n.IsGenericMethod && n.Name == "Load");
        var loadContentMethod = loadMethod.MakeGenericMethod(innerClass);
        var source = loadContentMethod.Invoke(Services.Content, new object[] { assetName });

        var mapping = Enumerable.Join(
            innerClass.GetFields(),
            typeof(TType).GetProperties(),
            n => n.Name,
            n => n.Name,
            (n, m) => Tuple.Create(n, m));

        foreach(var property in mapping)
        {
            if(property.Item1.FieldType == property.Item2.PropertyType)
            {
                property.Item2.SetValue(result, property.Item1.GetValue(source));
            }
            else 
            {
                Console.WriteLine("I don't know how to load this property.");
            }
        }

        return result;
    }
}


    public class TitleScreen : Screen
    {
        private SystemServices _services;
        private Scene _scene;
        private SceneObject _text;
        private InputCommandSet _commands;
        private InputManager _input;
        public TitleScreen(SystemServices services)
        {
            ContentManager2 cm2 = new ContentManager2(services);
            _scene = cm2.Load("Scenes/test", new Scene(services));

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

            _commands.Enabled = false;
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