using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.BitmapFonts;
using System;
using Legends.Engine.Graphics2D;
namespace Legends.Engine;


public class RenderService : IRenderService
{
    public class Frame
    {
        private List<Layer> _layers;

        public IReadOnlyList<Layer> Layers => _layers.AsReadOnly();

        public Frame(int layers)
        {
            _layers = new List<Layer>(Enumerable.Repeat(new Layer(), layers));
        }

        public void Reset()
        {
            foreach(var layer in _layers)
            {
                layer.Reset();
            }
        }
    }

    public class PositionDrawableComparer : IComparer<IBatchDrawable>
    {
        public int Compare(IBatchDrawable? x, IBatchDrawable? y)
        {
            return x.Position.Y.CompareTo(y.Position.Y);
        }
    }

    public class RenderState : IComparable<RenderState>
    {
        public SpriteSortMode SpriteSortMode { get; set; }
        public BlendState? BlendState { get; set; }
        public SamplerState? SamplerState { get; set; }
        public DepthStencilState? DepthStencilState { get; set; }
        public RasterizerState? RasterizerState { get; set; }
        public Effect? Effect { get; set; }
        public Matrix? Matrix { get; set; }

        public int CompareTo(RenderState? other)
        {
            if(other != null)
            {
                if(this.SpriteSortMode != other.SpriteSortMode) return -1;
                if(this.BlendState != other.BlendState) return -1;
                if(this.SamplerState != other.SamplerState) return -1;
                if(this.DepthStencilState != other.DepthStencilState) return -1;
                if(this.RasterizerState != other.RasterizerState) return -1;
                if(this.Effect != other.Effect) return -1;
                if(this.Matrix != other.Matrix) return -1;
            }

            return -1;    
        }
    }

    public class Layer
    {
        private IList<IBatchDrawable> _drawables;

        public IList<IBatchDrawable> Drawables => _drawables;

        public IOrderedEnumerable<IBatchDrawable> OrderedDrawables => _drawables.Where(n=> n.IsVisible || true).OrderBy(n => DrawableComparer);

        public IComparer<IBatchDrawable> DrawableComparer { get; set; }

        public Layer()
        {
            _drawables = new List<IBatchDrawable>();
            DrawableComparer = new PositionDrawableComparer();
        }

        public void Reset()
        {
            _drawables.Clear();
        } 
    }

    public Color? ClearColor;

    public Texture2D DefaultTexture { get; private set; }

    public int LayerCount { get; set; }

    public SystemServices Services { get; private set; }

    public Legends.Engine.Graphics2D.Camera? Camera { get; set; }

    public Frame Current { get; private set; }

    public RenderState DefaultRenderState { get; set; }

    public Effect DefaultEffect { get; set; }

    private SpriteBatch _spriteBatch;

    public RenderService(SystemServices services, int layerCount = 1)
    {
        Services = services;
        services.Services.AddService<IRenderService>(this);
        LayerCount = layerCount;
        Current = new Frame(LayerCount);
        DefaultRenderState = new RenderState();
        ClearColor = Color.Black;
    }

    public void DrawBatched(IBatchDrawable drawable)
    {
        Current.Layers[0].Drawables.Add(drawable);
    }

    public void Initialize()
    {
        if(DefaultTexture == null || _spriteBatch == null || DefaultEffect == null) // initialize
        {
            DefaultTexture = new Texture2D(Services.GraphicsDevice, 1, 1);
            DefaultTexture.SetData<Color>(new Color[] { Color.Green });
            _spriteBatch = new SpriteBatch(Services.GraphicsDevice);
            DefaultEffect = new BasicEffect (Services.GraphicsDevice)
            {
                VertexColorEnabled = true,
                TextureEnabled = true
            };

            IEffectMatrices? mtxEffect = (DefaultEffect as IEffectMatrices);

            Matrix _projection; 
            Matrix.CreateOrthographicOffCenter(0f, Services.GraphicsDevice.Viewport.Width, Services.GraphicsDevice.Viewport.Height, 0f, 0f, -1f, out _projection);
            mtxEffect.View         = Matrix.Identity;
            mtxEffect.Projection   = _projection;//Camera.Projection;
            mtxEffect.World        = Matrix.Identity;
        }  
    }

    public void Draw(GameTime gameTime)
    {
        if(ClearColor != null)
        {
            Services.GraphicsDevice.Clear(ClearColor.Value);
        }
        
        RenderState state = DefaultRenderState;
        bool batchStarted = false;
        foreach(var layer in Current.Layers)
        {
            foreach(var drawable in layer.OrderedDrawables)
            {
                if(!batchStarted)// || drawable.RequiredState != state)
                {
                    if(batchStarted)
                    {
                        _spriteBatch.End();
                    }

                    var mtx = state.Matrix;
                    var effect = state.Effect ?? DefaultEffect;

                    if(Camera != null)
                    {
                        if(effect is IEffectMatrices)
                        {
                            IEffectMatrices? mtxEffect = (effect as IEffectMatrices);

                            mtxEffect.View         = Camera.View;
                            mtxEffect.Projection   = Camera.Projection;
                            mtxEffect.World        = state.Matrix ?? Matrix.Identity;
                        } else {
                           mtx = Matrix.Multiply(Camera.View, Camera.Projection);
                        }
                    }

                    _spriteBatch.Begin(
                        state.SpriteSortMode,
                        state.BlendState,
                        state.SamplerState,
                        state.DepthStencilState,
                        state.RasterizerState,
                        DefaultEffect,//effect, // Effect
                        mtx // TransforMatrix
                    );

                    batchStarted = true;
                }

                if(drawable is ISpriteBatchDrawable)
                {
                    _spriteBatch.Draw(
                        (drawable as ISpriteBatchDrawable).SourceData ?? DefaultTexture,
                        (drawable as ISpriteBatchDrawable).DestinationBounds,
                        drawable.SourceBounds,
                        drawable.Color,
                        drawable.Rotation,
                        Vector2.Zero,//drawable.Origin,
                        drawable.Effect,
                        0);

                } 
                else if (drawable is IBitmapFontBatchDrawable)
                {
                    var fontDrawable = drawable as IBitmapFontBatchDrawable;

                    if(fontDrawable.Rotation > 0 || fontDrawable.Scale != Vector2.One)
                    {
                        _spriteBatch.DrawString(
                            fontDrawable.SourceData,
                            fontDrawable.Text,
                            drawable.Position,
                            drawable.Color,
                            drawable.Rotation,
                            drawable.Origin,
                            drawable.Scale,
                            drawable.Effect,
                            0,
                            null);
                    } else 
                    {
                        _spriteBatch.DrawString(
                            fontDrawable.SourceData,
                            fontDrawable.Text,
                            drawable.Position,
                            drawable.Color);
                    }
                }
            }
        }

        if(batchStarted)
        {
            _spriteBatch.End();
        }

        Current.Reset();
    }
    
}