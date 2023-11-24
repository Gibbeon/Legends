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

namespace Legends.App.Screens;



 public class MapScreen : Screen
{

    private Legends.Engine.Graphics2D.Camera _camera;
    private Legends.App.Actor _entity;

    private SystemServices _services;
    
    public MapScreen(SystemServices services)
    {
        _services = services;
        _camera = new Camera(services); 
        services.GetService<IRenderService>().Camera = _camera;     
        _entity = new Actor(_services)
        {
            Size = new Size2(26, 36),
            OriginNormalized = new Vector2(.5f, .5f)
        };

        _entity.GetBehavior<SpriteRenderBehavior>().TextureRegion = 
        new MonoGame.Extended.TextureAtlases.TextureRegion2D(_services.Content.Load<Texture2D>("npc1"), new Rectangle(Point.Zero, (Point)_entity.Size));

         var input = new InputManager(services, new KeyboardListenerSettings()
        {
            InitialDelayMilliseconds = 0
        });

        input.Register("EXIT",     Keys.Escape);
        input.Register("LOOK_AT",  MouseButton.Right);  
        input.Register("ZOOM",     EventType.MouseScroll); 

        input.Register("SELECT",   MouseButton.Left);
        input.Register("ROTATE",   MouseButton.Left).WithModifierAny(Keys.LeftAlt, Keys.RightAlt);
        
        input.Register("MOVE_LEFT",    Keys.Left);             
        input.Register("MOVE_RIGHT",   Keys.Right);      
        input.Register("MOVE_UP",      Keys.Up);             
        input.Register("MOVE_DOWN",    Keys.Down);

        _services.GetService<IInputHandlerService>().Push(input);

        //_entity.Spatial.Rotate(MathF.PI/4);
        //_entity.Spatial.Scale = new Vector2(.5f, .5f);

        //_canvas.Camera.Spatial.SetScale(2.0f);   

        //_animator = new Vector2Animator(_entity.Spatial.Scale, _entity.Spatial.Scale + new Vector2(2, 2), 20, (n) => { _entity.Spatial.Scale = n; });
        
        //_animator = new ValueAnimator<Color>(_entity.Material.AmbientColor, Color.Black, 20, (n) => { _entity.Material.AmbientColor = n; }, Color.Lerp);

        /*_animator = new ArrayValueAnimator<float>(Matrix.ToFloatArray(_entity.Spatial.LocalWorldMatrix), Matrix.ToFloatArray(Matrix.Identity), 10, (n) => 
        {
            Matrix m = new Matrix();
            var idx = 0;
            foreach(var value in n)
            {
                m[idx] = value;
                idx++;
            }
            Vector2 position;
            float rotation;
            Vector2 scale;

            m.Decompose(out position, out rotation, out scale);

            _entity.Spatial.Position = position;
            _entity.Spatial.Rotation = rotation;
            _entity.Spatial.Scale = scale;

        }, MathHelper.Lerp);
        */

        /*

        _animator = new KeyframeAnimator<Rectangle>(list.ToList());
        */
    }

    public override void Draw(GameTime gameTime)
    {
        //_canvas.Draw(gameTime);

        //SpriteBatch batch = new SpriteBatch(_game.GraphicsDevice);
        
        //batch.Begin();
        //batch.DrawString(Global.Fonts.Menu, string.Format("Camera Loc: {0:N0}, {1:N0} Center: {2:N0}, {3:N0}", _canvas.Camera.Position.X, _canvas.Camera.Position.Y, _canvas.Camera.Center.X, _canvas.Camera.Center.Y), Vector2.Zero, Color.White);
        //batch.DrawString(Global.Fonts.Menu, string.Format(" Mouse Abs: {0:N0}, {1:N0} World: {2:N0}, {3:N0}", Mouse.GetState().Position.X,Mouse.GetState().Position.Y, _canvas.Camera.ScreenToWorld(Mouse.GetState().Position.ToVector2()).X,_canvas.Camera.ScreenToWorld(Mouse.GetState().Position.ToVector2()).Y), new Vector2(0, 18), Color.White);
        //batch.DrawString(Global.Fonts.Menu, string.Format("Entity Loc: {0:N2}, {1:N2}", _entity.Spatial.Position.X, _entity.Spatial.Position.Y), new Vector2(0, 36+18), Color.White);
        //batch.End(); 
    }

    public override void Update(GameTime gameTime)
    {                
        foreach(var command in _services.GetService<IInputHandlerService>().Current.EventActions)
        {
            switch(command.Name)
            {
                case "EXIT":        _services.Exit(); break;

                case "MOVE_LEFT":   _entity.Move(-1, 0); break;
                case "MOVE_RIGHT":  _entity.Move( 1, 0); break;
                case "MOVE_UP":     _entity.Move( 0,-1); break;
                case "MOVE_DOWN":   _entity.Move( 0, 1); break;
                
                //case "ZOOM":        _canvas.Camera.ZoomIn(Command.GetScrollDelta()); break;  
                //case "ROTATE":      _canvas.Camera.Rotate(MathF.PI/8); break;
                //case "LOOK_AT":     _canvas.Camera.LookAt(_canvas.Camera.ScreenToWorld((Vector2)Command.GetPosition())); break;
                
                /*
                    case "SELECT":                   
                    if(_entity.Spatial.Contains(_canvas.Camera.ScreenToWorld(Mouse.GetState().Position.ToVector2())))
                    {
                        _entity.Material.AmbientColor = Color.DarkGray;          
                    }
                    else
                    {
                        _entity.Material.AmbientColor = Color.White;
                    }
                    break;
                */                 
            }
        }  
        
        _entity.Update(gameTime);  
    }
}