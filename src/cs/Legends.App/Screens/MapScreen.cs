using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.BitmapFonts;
using SlyEngine.Input;
using MonoGame.Extended.Input.InputListeners;
using Microsoft.Xna.Framework.Graphics;
using System;
using MonoGame.Extended;
using System.Collections.Generic;
using System.Linq;

namespace Legends.App.Screens;
/*
    public interface IValueAnimator : IUpdate
    {
        public float ElapsedTime        { get; }     
        public float Duration           { get; }
        public bool IsComplete          { get; }
    }

    public class Keyframe<TType>
    {
        public TType Value { get; set; }
        public float Duration { get; set; }
    }

    public class KeyframeAnimator<TType> : IValueAnimator
    {
        private IList<Keyframe<TType>> _frames;
        private int _current;
        
        public float ElapsedTime  { get; private set; }
        
        public float Duration => _frames.Sum(n => n.Duration);
        
        public bool IsComplete  => _current >= _frames.Count;

        public Keyframe<TType> Current => IsComplete ? null : _frames[_current];

        public KeyframeAnimator(IList<Keyframe<TType>> frames)
        {
            _frames = frames;
        }

        public void Update(GameTime gameTime)
        {
            if(IsComplete) return;

            ElapsedTime += gameTime.GetElapsedSeconds();

            while(ElapsedTime >= Current.Duration) {
                ElapsedTime -= Current.Duration;
                MoveNext();

                if(IsComplete) 
                {
                    ElapsedTime = 0;
                    return;
                }
            }
        }

        protected void MoveNext()
        {
            _current++;

            if(_current >= _frames.Count)
            {
                _current = 0;
            }
        }

    }

    public class ArrayValueAnimator<TType> : IValueAnimator
        where TType: struct
    {
        public IEnumerable<TType> OldValue                      { get; private set; }
        public IEnumerable<TType> NewValue                      { get; private set; }
        public float ElapsedTime                                { get; private set; }     
        public float Duration                                   { get; private set; }
        public Func<TType, TType, float, TType> Interpolate     { get; protected set; }
        public Action<IEnumerable<TType>> SetValue              { get; protected set; }
        public bool IsComplete => Duration == ElapsedTime;

        public ArrayValueAnimator(IEnumerable<TType> oldValue, IEnumerable<TType> newValue, float duration, Action<IEnumerable<TType>> setValue, Func<TType, TType, float, TType> interpolate)
        {
            OldValue        = oldValue;
            NewValue        = newValue;
            Duration        = duration;
            SetValue        = setValue;
            Interpolate     = interpolate;
        }

        private IEnumerable<TType> InterpolatedValues()
        {
            var oldEnumerator = OldValue.GetEnumerator();
            var newEnumerator = NewValue.GetEnumerator();

            while(oldEnumerator.MoveNext() && newEnumerator.MoveNext())
            {
                yield return Interpolate(oldEnumerator.Current, newEnumerator.Current, ElapsedTime / Duration);
            }
        }

        public void Update(GameTime gameTime)
        {
            ElapsedTime += Math.Min(gameTime.GetElapsedSeconds(), Duration - ElapsedTime); // clamp value

            if(IsComplete)
            {
                SetValue(NewValue);
            }
            else
            {
                SetValue(InterpolatedValues());
            }
        }
    }

    public class ValueAnimator<TType> : IValueAnimator
        where TType: struct
    {
        public TType OldValue                                   { get; private set; }
        public TType NewValue                                   { get; private set; }
        public float ElapsedTime                                { get; private set; }     
        public float Duration                                   { get; private set; }
        public Func<TType, TType, float, TType> Interpolate     { get; protected set; }
        public Action<TType> SetValue                           { get; protected set; }
        public bool IsComplete => Duration == ElapsedTime;

        public ValueAnimator(TType oldValue, TType newValue, float duration, Action<TType> setValue, Func<TType, TType, float, TType> interpolate)
        {
            OldValue        = oldValue;
            NewValue        = newValue;
            Duration        = duration;
            SetValue        = setValue;
            Interpolate     = interpolate;
        }

        public void Update(GameTime gameTime)
        {
            ElapsedTime += Math.Min(gameTime.GetElapsedSeconds(), Duration - ElapsedTime); // clamp value

            if(IsComplete)
            {
                SetValue(NewValue);
            }
            else
            {
                SetValue(Interpolate(OldValue, NewValue, ElapsedTime / Duration));
            }
        }
    }

    public class Vector2Animator: ValueAnimator<Vector2>
    {
        public Vector2Animator(Vector2 oldValue, Vector2 newValue, float duration, Action<Vector2> setValue, Func<Vector2, Vector2, float, Vector2> interpolate)
        : base(oldValue, newValue, duration, setValue, interpolate)
        {

        }

        public Vector2Animator(Vector2 oldValue, Vector2 newValue, float duration, Action<Vector2> setValue)
        : base(oldValue, newValue, duration, setValue, Vector2.Lerp)
        {

        }
    }

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
            _entity.Spatial.Size = new Size2(26, 36); 
            _entity.Spatial.OriginNormalized = new Vector2(.5f, .5f); 
            //_entity.Spatial.Rotation = MathF.PI / 4;

            //_entity.Spatial.Position = new Vector2(-32, -32);

            _canvas.Add().Add(_entity);

            _input = new InputManager(new KeyboardListenerSettings()
            {
                InitialDelayMilliseconds = 0
            });

            _input.Register("EXIT",     Keys.Escape);
            _input.Register("LOOK_AT",  MouseButton.Right);  
            _input.Register("ZOOM",     EventType.MouseScroll); 

            _input.Register("SELECT",   MouseButton.Left);
            _input.Register("ROTATE",   MouseButton.Left).WithModifierAny(Keys.LeftAlt, Keys.RightAlt);
            
            _input.Register("MOVE_LEFT",    Keys.Left);             
            _input.Register("MOVE_RIGHT",   Keys.Right);      
            _input.Register("MOVE_UP",      Keys.Up);             
            _input.Register("MOVE_DOWN",    Keys.Down);

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

            Keyframe<Rectangle>[] list = {
                new Keyframe<Rectangle>() { Duration = 0.5f, Value = new Rectangle(new Point(26 * 0, 0), (Point)_entity.Spatial.Size ) },
                new Keyframe<Rectangle>() { Duration = 0.5f, Value = new Rectangle(new Point(26 * 1, 0), (Point)_entity.Spatial.Size ) },
                new Keyframe<Rectangle>() { Duration = 0.5f, Value = new Rectangle(new Point(26 * 2, 0), (Point)_entity.Spatial.Size ) },
                new Keyframe<Rectangle>() { Duration = 0.5f, Value = new Rectangle(new Point(26 * 1, 0), (Point)_entity.Spatial.Size ) }
            };

            _animator = new KeyframeAnimator<Rectangle>(list.ToList());
        }

        private IValueAnimator _animator;

        public override void Draw(GameTime gameTime)
        {
            _canvas.Draw(gameTime);

            SpriteBatch batch = new SpriteBatch(_game.GraphicsDevice);
            
            batch.Begin();
            batch.DrawString(Global.Fonts.Menu, string.Format("Camera Loc: {0:N0}, {1:N0} Center: {2:N0}, {3:N0}", _canvas.Camera.Position.X, _canvas.Camera.Position.Y, _canvas.Camera.Center.X, _canvas.Camera.Center.Y), Vector2.Zero, Color.White);
            batch.DrawString(Global.Fonts.Menu, string.Format(" Mouse Abs: {0:N0}, {1:N0} World: {2:N0}, {3:N0}", Mouse.GetState().Position.X,Mouse.GetState().Position.Y, _canvas.Camera.ScreenToWorld(Mouse.GetState().Position.ToVector2()).X,_canvas.Camera.ScreenToWorld(Mouse.GetState().Position.ToVector2()).Y), new Vector2(0, 18), Color.White);
            batch.DrawString(Global.Fonts.Menu, string.Format("Entity Loc: {0:N2}, {1:N2}", _entity.Spatial.Position.X, _entity.Spatial.Position.Y), new Vector2(0, 36+18), Color.White);
            batch.End(); 
        }

        public override void Update(GameTime gameTime)
        {        
            _input.Update(gameTime);
            _animator?.Update(gameTime);

            foreach(var result in _input.Results)
            {
                switch(result.Command)
                {
                    case "EXIT":        _game.Exit(); break;

                    case "MOVE_LEFT":   _entity.Spatial.Move(-1, 0); break;
                    case "MOVE_RIGHT":  _entity.Spatial.Move( 1, 0); break;
                    case "MOVE_UP":     _entity.Spatial.Move( 0,-1); break;
                    case "MOVE_DOWN":   _entity.Spatial.Move( 0, 1); break;
                    
                    case "ZOOM":        _canvas.Camera.ZoomIn(result.GetScrollDelta()); break;  
                    case "ROTATE":      _canvas.Camera.Rotate(MathF.PI/8); break;
                    case "LOOK_AT":     _canvas.Camera.LookAt(_canvas.Camera.ScreenToWorld((Vector2)result.GetPosition())); break;
                    
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
                }
            }    

            if(!(_animator as KeyframeAnimator<Rectangle>).IsComplete)
            {
                _entity.Material.ColorMap.Bounds = (_animator as KeyframeAnimator<Rectangle>).Current.Value;
            }



            _canvas.Update(gameTime);   
        }
    }

    */