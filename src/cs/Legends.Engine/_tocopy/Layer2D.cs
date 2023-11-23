using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Legends.Engine.Graphics.Collections;

namespace Legends.Engine.Graphics;

/*
    public class Layer2D : Spatial2D, IDrawable, IUpdateable
    {
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;

        public bool Visible
        {
            get => _visible;
            set => SetVisible(value);
        }

        public int DrawOrder
        {
            get => _drawOrder;
            set => SetDrawOrder(value);
        }
        
        public bool Enabled 
        {
            get => _enabled;
            set => SetEnabled(value);
        }
        public int UpdateOrder
        {
            get => _updateOrder;
            set => SetUpdateOrder(value);
        }
        public Canvas2D Canvas
        {
            get; protected set;
        }

        public Camera2D? Camera
        {
            get; set;
        }

        public SpriteSortMode SpriteSortMode
        {
            get;
            set;
        }
        public BlendState? BlendState
        {
            get;
            set;
        }
        public SamplerState? SamplerState
        {
            get;
            set;
        }
        public DepthStencilState? DepthStencilState
        {
            get;
            set;
        }
        public RasterizerState? RasterizerState
        {
            get;
            set;
        }
        public Effect? Effect
        {
            get;
            set;
        }
        public IReadOnlyList<ISpriteBatchDrawable2D> Sprites
        {
            get => _sprites;
        }

        private List<ISpriteBatchDrawable2D> _sprites;
        private DrawableListIndex<ISpriteBatchDrawable2D> _spritesDrawIndex;
        protected int _drawOrder;
        protected int _updateOrder;
        protected bool _enabled;
        protected bool _visible;

        public Layer2D(Canvas2D canvas)
        {
            _sprites = new List<ISpriteBatchDrawable2D>();
            _spritesDrawIndex = new DrawableListIndex<ISpriteBatchDrawable2D>(_sprites);
            _visible = true;

            Canvas = canvas;
            SamplerState = SamplerState.PointClamp;
            Effect = new BasicEffect(canvas.Game.GraphicsDevice) { 
                TextureEnabled = true,
                VertexColorEnabled = true
            };
        }


        public virtual void Draw(GameTime gameTime)
        {
            if (!Visible) return;

            Canvas.Game.GraphicsDevice.Clear(Color.Black);

            //Matrix? modelViewProjection = null;

            var camera = Canvas.Camera;

            IEffectMatrices? effect = (Effect as IEffectMatrices);

            effect.View         = camera.View;
            //effect.View         = Matrix.Identity; //camera.View;
            effect.Projection   = camera.Projection;
            //effect.Projection   = Matrix.Identity; //camera.View;
            effect.World        = Matrix.Identity;
            
            var spriteBatch = new SpriteBatch(Canvas.Game.GraphicsDevice);

            spriteBatch.Begin(
                SpriteSortMode,
                BlendState, 
                SamplerState, 
                DepthStencilState, 
                RasterizerState, 
                Effect           
            );

            foreach (var drawable in _spritesDrawIndex)
            {
                drawable.DrawBatched(spriteBatch, gameTime);
            }

            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {            
            if (!Enabled) return;

            base.Update(gameTime);

        }

        public void Add(DrawableEntity2D item)
        {

            var sprite = item as DrawableEntity2D;

            if (sprite != null)
            {
                _spritesDrawIndex.Add(sprite);
            }
        }

        public void Remove(DrawableEntity2D item)
        {
            //base.Remove(item);

            var sprite = item as DrawableEntity2D;

            if (sprite != null)
            {
                _spritesDrawIndex.Remove(sprite);
            }
        }

        protected void SetVisible(bool value)
        {
            if (_visible != value)
            {
                _visible = value;
                if (VisibleChanged != null)
                {
                    VisibleChanged(this, EventArgs.Empty);
                }
            }
        }
        protected void SetDrawOrder(int value)
        {
            if (_drawOrder != value)
            {
                _drawOrder = value;
                if (DrawOrderChanged != null)
                {
                    DrawOrderChanged(this, EventArgs.Empty);
                }
            }
        }
        protected void SetEnabled(bool value)
        {
            if (_enabled != value)
            {
                _enabled = value;
                if (EnabledChanged != null)
                {
                    EnabledChanged(this, EventArgs.Empty);
                }
            }
        }
        protected void SetUpdateOrder(int value)
        {
            if (_updateOrder != value)
            {
                _updateOrder = value;
                if (UpdateOrderChanged != null)
                {
                    UpdateOrderChanged(this, EventArgs.Empty);
                }
            }
        }
    }

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

*/
