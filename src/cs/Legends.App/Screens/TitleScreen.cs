using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Input;
using Legends.Engine.Input;
using Legends.Engine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Legends.App.Screens
{

    public class ContentManager2
{
    IServiceProvider Services { get; set; }

    public ContentManager2(IServiceProvider services)
    {
        Services = services;
    }

    public void SetValues(Type sourceType, object source, Type destinationType, object destination)
    {
        var mapping = Enumerable.Join(
            sourceType.GetFields(),
            destinationType.GetProperties(),
            n => n.Name,
            n => n.Name,
            (n, m) => Tuple.Create(n, m));


        foreach(var property in mapping)
        {
            var value = property.Item1.GetValue(source);

            if(value != null)
            {
                if(property.Item1.FieldType == property.Item2.PropertyType)
                {
                    Console.WriteLine("Setting {0} to {1}", property.Item1.Name, value);
                    property.Item2.SetValue(destination, value);
                }
                else if (property.Item1.FieldType == typeof(string))
                {
                    property.Item2.SetValue(destination, Services.GetContentManager().Load<BitmapFont>(value.ToString()));
                }
                else if(property.Item1.FieldType.GetInterface(typeof(IEnumerable).Name) != null)
                {                       
                    Console.WriteLine("Setting {0} to {1}", property.Item1.Name, value);

                    

                    Type sourceItemType     = property.Item1.FieldType.GenericTypeArguments[0];
                    Type destItemType       = sourceItemType.DeclaringType;
                    var  destList           = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(destItemType));

                    foreach(var item in (value as IList))
                    {
                        Type destItemTypeActual = destItemType;
                        Type sourceItemTypeActual = sourceItemType;
        
                        //if(item is ActivatorDesc activatorDesc)
                        //{
                        //    if(!string.IsNullOrEmpty(activatorDesc.TypeOf))
                        //    {
                        //        Console.WriteLine("Overriding type to: {0}", Type.GetType(typeof(TextRenderBehavior).AssemblyQualifiedName));
                        //        destItemTypeActual = Type.GetType(typeof(TextRenderBehavior).AssemblyQualifiedName);//Type.GetType(activatorDesc.TypeOf);
                        //        sourceItemTypeActual = destItemTypeActual.DeclaringType;
                        //    }
                        //}

                        var itemObject = Activator.CreateInstance(destItemTypeActual, Services, destination);
                        SetValues(item.GetType(), item, destItemTypeActual, itemObject);
                        destList.Add(itemObject);
                    }
                    property.Item2.SetValue(destination, destList);
                }
                else
                {        
                    Console.WriteLine("Setting {0} to {1}", property.Item1.Name, value);
                             
                    //var camera = new Camera(Services, result as Scene);
                    var propertyObject = Activator.CreateInstance(property.Item1.FieldType.DeclaringType, Services, destination);
                    
                    //LoadProperties(CameraDesc, camera);
                    SetValues(property.Item1.FieldType, value, propertyObject.GetType(), propertyObject);    

                    // Scene.Camnera = value
                    property.Item2.SetValue(destination, propertyObject);              
                }
            }
        }
    }

    public TType Load<TType>(string assetName, TType result)
    {
        return Services.GetContentManager().Load<TType>(assetName);
    }
}


    public class TitleScreen : Screen
    {
        private IServiceProvider _services;
        private Scene _scene;
        //private SceneObject _text;
        private InputCommandSet _commands;
        private InputManager _input;
        public TitleScreen(IServiceProvider services)
        {
            _services = services;
            _input = new InputManager(_services);

            //ContentManager2 cm2 = new ContentManager2(IServiceProvider);
            //_scene = cm2.Load("Scenes/test", new Scene(services));

            _scene = _services.GetContentManager().Load<Scene>("Scenes/test");
            _scene.Initialize();


            _commands = new InputCommandSet(_services, _input);
            _commands.Add("EXIT", EventType.KeyTyped, Keys.Escape);
            _commands.Add("EXIT", EventType.MouseClicked, MouseButton.Right);

            _commands.Add("START", EventType.KeyTyped, Keys.Enter);
            _commands.Add("START", EventType.MouseClicked, MouseButton.Left);
            

            /*
            _text = new SceneObject(_services, _scene);
            _text.AttachBehavior(new TextRenderBehavior(_text)
            {
                Font = services.Content.Load<BitmapFont>("Sensation"),
                Text = "Press ESC to Exit or ENTER to Start",
                Color = Color.White
            }); 
            */

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
                    //case "EXIT": _services.Exit(); break;
                    case "START": Start(); break;
                }
            }

            _scene.Update(gameTime);
        }

        protected void Start()
        {
            _input.Enabled = false;
            ScreenManager.LoadScreen(new MapScreen(_services), new MonoGame.Extended.Screens.Transitions.FadeTransition(_services.GetGraphicsDevice(), Color.Black));
        }

        public override void Dispose()
        {
            _scene.Dispose();
            _input.Dispose();

            base.Dispose();
        }
    }
}