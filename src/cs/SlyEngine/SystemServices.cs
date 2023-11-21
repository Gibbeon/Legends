using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.Sprites;
using System;
using System.Net.Http.Headers;
using System.Xml.Linq;
using MonoGame.Extended;
using MonoGame.Framework.Utilities;

namespace SlyEngine;

public class SpriteRenderService
{
    public class Frame
    {
        private List<Layer> _layers;

        public IReadOnlyList<Layer> Layers => _layers.AsReadOnly();

        public Frame(int layers)
        {
            _layers = new List<Layer>(Enumerable.Repeat(new Layer(), layers));
        }
    }

    public class PositionDrawableComparer : IComparer<ISpriteBatchDrawable>
    {
        public int Compare(ISpriteBatchDrawable? x, ISpriteBatchDrawable? y)
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
        private IList<ISpriteBatchDrawable> _drawables;

        public IList<ISpriteBatchDrawable> Drawables => _drawables;

        public IOrderedEnumerable<ISpriteBatchDrawable> OrderedDrawables => _drawables.Where(n=> n.IsVisible).OrderBy(n => DrawableComparer);

        public IComparer<ISpriteBatchDrawable> DrawableComparer { get; set; }

        public Layer()
        {
            _drawables = new List<ISpriteBatchDrawable>();
            DrawableComparer = new PositionDrawableComparer();
        }
    }

    public int LayerCount { get; set; }

    public SystemServices Services { get; private set; }

    public Frame Current { get; private set; }

    public RenderState DefaultRenderState { get; set; }

    public SpriteRenderService(SystemServices services, int layerCount = 1)
    {
        Services = services;
        LayerCount = layerCount;
        Current = new Frame(LayerCount);
        DefaultRenderState = new RenderState();
    }

    public void Draw(GameTime gameTime)
    {
        SpriteBatch spriteBatch = new SpriteBatch(Services.GraphicsDevice);
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
                        spriteBatch.End();
                    }

                    spriteBatch.Begin(
                        state.SpriteSortMode,
                        state.BlendState,
                        state.SamplerState,
                        state.DepthStencilState,
                        state.RasterizerState,
                        state.Effect, // Effect
                        state.Matrix // TransforMatrix
                    );
                }

                spriteBatch.Draw(
                    drawable.TextureRegion.Texture,
                    drawable.Position,
                    drawable.TextureRegion.Bounds,
                    drawable.Color,
                    drawable.Rotation,
                    drawable.Origin,
                    drawable.Scale,
                    drawable.Effect,
                    0);
            }
        }

        if(batchStarted)
        {
            spriteBatch.End();
        }
    }
}


public class SystemServices: IUpdate
{
    private Game _game;

    public GraphicsDevice GraphicsDevice => _game.Services.GetService<IGraphicsDeviceService>().GraphicsDevice;

    public SystemServices(Game game)
    {
        _game = game;
        _game.Services.AddService<SpriteRenderService>(new SpriteRenderService(this));
    }

    public void Update(GameTime gameTime)
    {
        
    }

    public void Draw(GameTime gameTime)
    {
        _game.Services.GetService<SpriteRenderService>().Draw(gameTime);
    }
}
