using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.BitmapFonts;
using System;
using MonoGame.Extended.Sprites;

namespace Legends.Engine.Graphics2D;

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
            if (drawable is ISelfRenderable selfDrawable)
            {
                if (batchStarted)
                {
                    _spriteBatch.End();
                    batchStarted = false;
                }
                selfDrawable.DrawImmediate(gameTime);
            }
            else if (drawable is ISpriteRenderable batchDrawable)
            {
                if (!batchStarted ||
                    ((batchDrawable.RenderState ?? _renderService.DefaultRenderState) != _renderState) ||
                    _viewState != batchDrawable.ViewState)
                {
                    if (batchStarted)
                    {
                        _spriteBatch.End();
                        batchStarted = false;
                    }

                    _renderState.CopyFrom(batchDrawable.RenderState ?? _renderService.DefaultRenderState);
                    _viewState.CopyFrom(batchDrawable.ViewState);

                    if (_renderState.Effect is IEffectMatrices mtxEffect)
                    {
                        mtxEffect.View = _viewState.View;
                        mtxEffect.Projection = _viewState.Projection;
                        mtxEffect.World = _viewState.World;
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

                if (drawable is ITexturedSpriteRenderable spriteBatchDrawable)
                {
                    _spriteBatch.Draw(
                        spriteBatchDrawable.SourceData.Texture,
                        spriteBatchDrawable.DestinationBounds,
                        (Rectangle)spriteBatchDrawable.SourceData.BoundingRectangle,
                        spriteBatchDrawable.Color,
                        spriteBatchDrawable.Rotation,
                        Vector2.Zero,//drawable.Origin,
                        spriteBatchDrawable.Effect,
                        0);

                }
                else if (drawable is IBitmapFontBatchRenderable fontDrawable)
                {
                    if (String.IsNullOrEmpty(fontDrawable.Text))
                    {
                        continue;
                    }

                    if (fontDrawable.Rotation > 0 || fontDrawable.Scale != Vector2.One)
                    {
                        _spriteBatch.DrawString(
                            fontDrawable.SourceData,
                            fontDrawable.Text,
                            Vector2.Zero,
                            fontDrawable.Color,
                            fontDrawable.Rotation,
                            -fontDrawable.Position,
                            fontDrawable.Scale,
                            fontDrawable.Effect,
                            0,
                            fontDrawable.DestinationBounds);
                    }
                    else
                    {
                        _spriteBatch.DrawString(
                            fontDrawable.SourceData,
                            fontDrawable.Text,
                            fontDrawable.Position,
                            fontDrawable.Color,
                            fontDrawable.DestinationBounds);
                    }
                }
            }

            if (batchStarted)
            {
                _spriteBatch.End();
                batchStarted = false;
            }
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
    }
}
