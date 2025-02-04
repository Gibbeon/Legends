using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Legends.Engine.Graphics2D;

/*
public interface IRenderLayer : IInitalizable
{    
    Color? ClearColor { get; set; }
    IEnumerable<IRenderable> Drawables { get; }
    void Enqueue(IRenderable renderable);
    void BeginDraw();
    void DrawImmediate(GameTime gameTime);
    void EndDraw();
}
public class RenderLayer : IRenderLayer
{
    protected List<IRenderable> _drawables;
    protected IRenderService _renderService;
    protected RenderState _renderState;
    protected ViewState _viewState;
    protected SpriteBatch _spriteBatch;
    public IEnumerable<IRenderable> Drawables => _drawables;
    public IOrderedEnumerable<IRenderable> OrderedVisibleDrawables => _drawables.Where(n => n.Visible).OrderBy(n => DrawableComparer ?? Comparer<IRenderable>.Default);
    public IComparer<IRenderable> DrawableComparer { get; set; }
    public Color? ClearColor { get; set; }
    public bool Visible { get; set; }
    
    public RenderLayer(IRenderService renderService)
    {
        _renderState = new RenderState();
        _drawables = new List<IRenderable>();
        _renderService = renderService;
        
        Reset();
    }

    public void Enqueue(IRenderable renderable)
    {
        _drawables.Add(renderable);
    }

    public void BeginDraw()
    {
        if (Visible && ClearColor.HasValue)
        {
            _renderService.GraphicsDevice.Clear(ClearColor.Value);
        }
    }

    public void EndDraw()
    {
        // swap buffers
        _drawables.Clear();
    }

    public void DrawImmediate(GameTime gameTime)
    {
        if (!Visible)
        {
            return;
        }

        bool batchStarted = false;
        foreach (IRenderable drawable in OrderedVisibleDrawables)
        {            
            if (!batchStarted ||
                ((drawable.RenderState ?? _renderService.DefaultRenderState) != _renderState) ||
                _viewState != drawable.ViewState)
            {
                if (batchStarted)
                {
                    _spriteBatch.End();
                    batchStarted = false;
                }

                _renderState.CopyFrom(drawable.RenderState ?? _renderService.DefaultRenderState);
                _viewState.CopyFrom(drawable.ViewState);
                
                _renderService.GraphicsDevice.Viewport = _viewState.Viewport;

                if(_renderState.Effect == null) {
                    _renderState.Effect = _renderService.DefaultRenderState.Effect;
                }

                if (_renderState.Effect is IEffectMatrices mtxEffect)
                {
                    mtxEffect.View          = _viewState.View;
                    mtxEffect.Projection    = _viewState.Projection;
                    mtxEffect.World         = _viewState.World;
                }

                _spriteBatch.Begin(
                    _renderState.SpriteSortMode,
                    _renderState.BlendState,                    
                    _renderState.SamplerState,
                    _renderState.DepthStencilState,
                    _renderState.RasterizerState,
                    _renderState.Effect,
                    null
                );

                batchStarted = true;
            }
            
            drawable.DrawImmediate(gameTime, _spriteBatch);
        }

        if (batchStarted)
        {
            _spriteBatch.End();
        }
    }

    public void Initialize()
    {
        Reset();
        Visible = true;
    }

    public void Reset()
    {        
        _spriteBatch = new SpriteBatch(_renderService.GraphicsDevice);
        Matrix.CreateOrthographicOffCenter(0f, _renderService.GraphicsDevice.Viewport.Width, _renderService.GraphicsDevice.Viewport.Height, 0f, 0f, -1f, out Matrix projection);

        _viewState = new ViewState()
        {
            View = Matrix.Identity,
            Projection = projection,
            World = Matrix.Identity
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
*/