using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using Legends.Engine.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended;
using Legends.Engine;
using Legends.Engine.Graphics2D;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Legends.App.Screens;

public class MapScreen : Screen
{
    private Legends.App.Actor _entity;
    private Scene _scene;
    private SystemServices _services;

    private InputManager _input;

    public Actor NewEntity()
    {
        var result = new Actor(_services, _scene)
        {
            Size = new Size2(26, 36),
            OriginNormalized = new Vector2(.5f, .5f)
        };

        result.GetBehavior<SpriteRenderBehavior>().TextureRegion = new MonoGame.Extended.TextureAtlases.TextureRegion2D(_services.Content.Load<Texture2D>("npc1"), new Rectangle(Point.Zero, (Point)result.Size));

        return result;
    }
    
    public MapScreen(SystemServices services)
    {
        _services = services;
        
        _input = new InputManager(services, new KeyboardListenerSettings()
        {
            InitialDelayMilliseconds = 0,
            RepeatDelayMilliseconds = 0,
            RepeatPress = true
        });

        _scene = new Scene(services); 
        _scene.Camera.AttachBehavior(new CameraControlsBehavior(_scene.Camera));

        _scene.AttachChild(new Map(_services, _scene));
        _entity = NewEntity();

        

        /*
        input.Register("EXIT",     Keys.Escape);
        input.Register("LOOK_AT",  MouseButton.Right);  
        input.Register("ZOOM",     EventType.MouseScroll); 

        
        input.Register("ADD",     Keys.Add); 

        input.Register("SELECT",   MouseButton.Left);
        input.Register("ROTATE",   MouseButton.Left).WithModifierAny(Keys.LeftAlt, Keys.RightAlt);
        
        input.Register("MOVE_LEFT",    Keys.Left);             
        input.Register("MOVE_RIGHT",   Keys.Right);      
        input.Register("MOVE_UP",      Keys.Up);             
        input.Register("MOVE_DOWN",    Keys.Down);

        _services.GetService<IInputHandlerService>().Push(input);
        */
    }

    public override void Draw(GameTime gameTime)
    {        
        _scene.Draw(gameTime);
    }

    public override void Update(GameTime gameTime)
    {                
        /*foreach(var command in _services.GetService<IInputHandlerService>().Current.EventActions)
        {
            switch(command.Name)
            {
                case "EXIT":        _services.Exit(); break;
                case "ADD":         _scene.AttachChild(NewEntity()); break;
             
            }
        } */
        
        _scene.Update(gameTime);
    }
}