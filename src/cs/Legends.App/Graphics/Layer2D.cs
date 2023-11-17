using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Legends.App.Collections;

namespace Legends.App.Graphics
{
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

            effect.View = camera.View;
            effect.Projection = camera.Projection;
            effect.World =  Matrix.Identity;
            
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

        public void Update(GameTime gameTime)
        {            
            if (!Enabled) return;

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
}
