using System;
using System.Collections.Generic;
using System.IO;
using LitEngine.Framework.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;

namespace LitEngine.Framework.Graphics
{
    public class Layer2D : Spatial, IDrawable
    {
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;

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
        public Canvas2D Canvas
        {
            get; protected set;
        }

        public Camera LocalCamera
        {
            get; set;
        }

        public SpriteBatch SpriteBatch
        {
            get;
            set;
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
        public IReadOnlyList<IDrawable2D> Sprites
        {
            get => _sprites;
        }

        private List<IDrawable2D> _sprites;
        private DrawableListIndex<IDrawable2D> _spritesDrawIndex;
        protected int _drawOrder;

        public Layer2D(Canvas2D canvas) : this(canvas, new SpriteBatch(GameEngine.Instance.GraphicsDevice))
        {

        }

        public Layer2D(Canvas2D canvas, SpriteBatch spriteBatch)
        {
            _sprites = new List<IDrawable2D>();
            _spritesDrawIndex = new DrawableListIndex<IDrawable2D>(_sprites);
            _visible = true;

            Canvas = canvas;
            SpriteBatch = spriteBatch;
            SamplerState = SamplerState.PointClamp;
            Effect = new BasicEffect(spriteBatch.GraphicsDevice) { 
                TextureEnabled = true,
                VertexColorEnabled = true
            };
        }


        public virtual void Draw(GameTime gameTime)
        {
            if (!Visible) return;

            Matrix? modelViewProjection = null;

            var camera = (Canvas.Camera ?? LocalCamera);

            IEffectMatrices? effect = (Effect as IEffectMatrices);
            
            //if (effect != null && camera != null)
            {
                effect.View = camera.View;
                effect.Projection = camera.Projection;
                effect.World =  camera.GlobalWorldMatrix;
            }
            //else if (effect == null)
            {
                modelViewProjection = (camera != null ? camera.ModelViewProjection : Matrix.Identity) * GlobalWorldMatrix;
            }

            SpriteBatch.Begin(
                SpriteSortMode,
                BlendState,
                SamplerState,
                DepthStencilState,
                RasterizerState,
                Effect,
                GlobalWorldMatrix
                );

            foreach (var drawable in _spritesDrawIndex)
            {
                drawable.DrawBatched(gameTime, SpriteBatch);
            }

            SpriteBatch.End();
        }

        public void Add(IDrawable2D item)
        {
            //base.Add(item);

            var sprite = item as IDrawable2D;

            if (sprite != null)
            {
                _spritesDrawIndex.Add(sprite);
            }
        }

        public void Remove(IDrawable2D item)
        {
            //base.Remove(item);

            var sprite = item as IDrawable2D;

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
    }
}
