using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.BitmapFonts;
using System;

namespace Legends.Engine;

public class RenderService : IRenderService
{
    public IServiceProvider Services { get; private set; }
    public RenderState DefaultRenderState { get; set; }
    public Texture2D DefaultTexture { get; private set; }
    public GraphicsDevice GraphicsDevice => Services.GetGraphicsDevice();
    private readonly List<ILayer> _layers;

    public RenderService(IServiceProvider services)
    {
        Services = services;
        Services.Add<IRenderService>(this);
        DefaultRenderState = new RenderState();
        _layers = new List<ILayer>();
    }

    public void Initialize()
    {
        if (DefaultTexture == null || DefaultRenderState.Effect == null)
        {
            DefaultTexture = new Texture2D(GraphicsDevice, 1, 1);
            DefaultTexture.SetData(new Color[] { Color.Green });

            DefaultRenderState.Effect = new BasicEffect(GraphicsDevice)
            {
                VertexColorEnabled = true,
                TextureEnabled = true
            };
        }

        _layers.Add(new Layer(this) { ClearColor = Color.Black });
    }

    public void Draw(GameTime gameTime)
    {
        _layers[0].BeginDraw();
        _layers[0].DrawImmediate(gameTime);
        _layers[0].EndDraw();
    }


    public void DrawBatched(IDrawable drawable)
    {
        _layers[0].Drawables.Add(drawable);
    }
}

public interface IDrawable
{
    bool Visible { get; }
}
public interface ISelfDrawable : IDrawable
{
    void DrawImmediate(GameTime gameTime);
}
public interface ILayer
{
    public Color? ClearColor { get; set; }
    public IList<IDrawable> Drawables { get; }
    void BeginDraw();
    void DrawImmediate(GameTime gameTime);
    void EndDraw();
}
public class Layer : ILayer
{
    protected IList<IDrawable> _drawables;
    protected IRenderService _renderService;
    protected RenderState _renderState;
    protected ViewState _viewState;
    protected SpriteBatch _spriteBatch;
    public IList<IDrawable> Drawables => _drawables;
    public IOrderedEnumerable<IDrawable> OrderedVisibleDrawables => _drawables.Where(n => n.Visible).OrderBy(n => DrawableComparer ?? Comparer<IDrawable>.Default);
    public IComparer<IDrawable> DrawableComparer { get; set; }
    public Color? ClearColor { get; set; }
    public bool Visible { get; set; }

    public Layer(IRenderService renderService)
    {
        _renderService = renderService;
        _renderState = new RenderState();
        _drawables = new List<IDrawable>();
        _spriteBatch = new SpriteBatch(_renderService.GraphicsDevice);

        Matrix.CreateOrthographicOffCenter(0f, _renderService.GraphicsDevice.Viewport.Width, _renderService.GraphicsDevice.Viewport.Height, 0f, 0f, -1f, out Matrix projection);

        _viewState = new ViewState()
        {
            View = Matrix.Identity,
            Projection = projection,
            World = Matrix.Identity
        };

        Visible = true;
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

        foreach (IDrawable drawable in OrderedVisibleDrawables)
        {
            if (drawable is ISelfDrawable selfDrawable)
            {
                if (batchStarted)
                {
                    _spriteBatch.End();
                    batchStarted = false;
                }
                selfDrawable.DrawImmediate(gameTime);
            }
            else if (drawable is IBatchDrawable batchDrawable)
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

                if (drawable is ISpriteBatchDrawable spriteBatchDrawable)
                {
                    _spriteBatch.Draw(
                        spriteBatchDrawable.SourceData ?? _renderService.DefaultTexture,
                        spriteBatchDrawable.DestinationBounds,
                        spriteBatchDrawable.SourceBounds,
                        spriteBatchDrawable.Color,
                        spriteBatchDrawable.Rotation,
                        Vector2.Zero,//drawable.Origin,
                        spriteBatchDrawable.Effect,
                        0);

                }
                else if (drawable is IBitmapFontBatchDrawable fontDrawable)
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
                            null);
                    }
                    else
                    {
                        _spriteBatch.DrawString(
                            fontDrawable.SourceData,
                            fontDrawable.Text,
                            fontDrawable.Position,
                            fontDrawable.Color);
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
}

public class YPositionDrawableComparer : IComparer<IBatchDrawable>
{
    public int Compare(IBatchDrawable x, IBatchDrawable y)
    {
        if (x == null || y == null)
        {
            return Comparer<IBatchDrawable>.Default.Compare(x, y);
        }
        else
        {
            return x.Position.Y.CompareTo(y.Position.Y);
        }
    }
}

