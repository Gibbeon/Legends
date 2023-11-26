/*using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.BitmapFonts;
using Legends.Engine.Graphics2D;
using Autofac.Core.Lifetime;
using MonoGame.Extended.Tiled;
using System;
namespace Legends.Engine;

public class RenderService : IRenderService
{ 
    public SystemServices   Services { get; private set; }
    public RenderState      DefaultRenderState { get; set; }
    public Effect           DefaultEffect { get; set; }
    public Texture2D        DefaultTexture { get; private set; }
    public GraphicsDevice   GraphicsDevice => Services.GraphicsDevice;
    private List<ILayer>    _layers;

    public RenderService(SystemServices services)
    {
        Services = services;
        services.Services.AddService<IRenderService>(this);
        DefaultRenderState = new RenderState();
        _layers = new List<ILayer>();
    }

    public void Initialize()
    {
        if(DefaultTexture == null || DefaultEffect == null)
        {            
            DefaultTexture = new Texture2D(Services.GraphicsDevice, 1, 1);
            DefaultTexture.SetData<Color>(new Color[] { Color.Green });

            DefaultEffect = new BasicEffect (Services.GraphicsDevice)
            {
                VertexColorEnabled = true,
                TextureEnabled = true
            };

            Matrix projection; 
            Matrix.CreateOrthographicOffCenter(0f, Services.GraphicsDevice.Viewport.Width, Services.GraphicsDevice.Viewport.Height, 0f, 0f, -1f, out projection);

            DefaultRenderState.Projection = projection;
        }  
    }

    public void Draw(GameTime gameTime)
    {       
        // foreach layer
        // layer.Draw()
    }    

    public void Enqueue(ILayer layer)
    {
        _layers.Add(layer);
    }

    public void Enqueue(IBatchDrawable drawable)
    {
        //Current.Layers[0].Drawables.Add(drawable);
    }

    public void Enqueue(ISelfDrawable drawable)
    {
        //Current.Layers[0].Drawables.Add(drawable);
    }
}

public interface ISelfDrawable : IHasVisibility
{
    void DrawImmediate(GameTime gameTime);
}

public interface IHasVisibility
{
    bool IsVisible { get; }
}

public interface ILayer : ISelfDrawable
{    
    public Type DrawableType    { get; }
    public Camera? Camera       { get; }
    public Color? ClearColor    { get; set; }
    void BeginDraw();
    void EndDraw();
}

public abstract class Layer<TType> : ILayer where TType : IHasVisibility
{
    protected IList<TType>      _drawables;
    protected IRenderService    _renderService;    
    protected RenderState       _renderState;
    public Type DrawableType => typeof(TType);
    public IList<TType>         Drawables => _drawables;
    public IOrderedEnumerable<TType> OrderedDrawables => _drawables.Where(n=> n.IsVisible || true).OrderBy(n => (DrawableComparer ?? Comparer<TType>.Default));
    public IComparer<TType>?    DrawableComparer { get; set; }
    public Camera? Camera       { get; }
    public Color? ClearColor    { get; set; }
    public bool IsVisible       { get; set; }

    public void BeginDraw()
    {
        if(IsVisible && ClearColor.HasValue)
        {
            _renderService.GraphicsDevice.Clear(ClearColor.Value);
        }
    }

    public abstract void DrawImmediate(GameTime gameTime);    

    public void EndDraw()
    {
        // swap buffers
    }
    public void Reset()
    {
        _drawables.Clear();
    } 
}

public class DrawableLayer : Layer<ISelfDrawable>
{
    public override void DrawImmediate(GameTime gameTime)
    {
        foreach(var drawable in _drawables.Where(n => n.IsVisible))
        {
            drawable.DrawImmediate(gameTime);
        }

        Reset();
    }
}

public class BatchLayer : Layer<IBatchDrawable>
{ 
    private SpriteBatch _spriteBatch;

    public BatchLayer(IRenderService renderService)
    {
        _drawables = new List<IBatchDrawable>();
        _renderService = renderService;

        DrawableComparer = new YPositionDrawableComparer();
    }

    public void Initialize()
    {
        if( _spriteBatch == null) // initialize
        {            
            _spriteBatch = new SpriteBatch(_renderService.GraphicsDevice);
        }  
    }

    public override void DrawImmediate(GameTime gameTime)
    {
        var batchStarted = false;

        foreach(var drawable in OrderedDrawables)
        {
            if(!batchStarted || ((drawable.RenderState ?? _renderService.DefaultRenderState) != _renderState))
            {
                if(batchStarted)
                {
                    _spriteBatch.End();
                }

                if((_renderState.Effect ?? _renderService.DefaultEffect) is IEffectMatrices mtxEffect)
                {
                    mtxEffect.View         = _renderState.View.GetValueOrDefault(Matrix.Identity);
                    mtxEffect.Projection   = _renderState.Projection.GetValueOrDefault(Matrix.Identity);
                } 

                _spriteBatch.Begin(
                    _renderState.SpriteSortMode,
                    _renderState.BlendState,
                    _renderState.SamplerState,
                    _renderState.DepthStencilState,
                    _renderState.RasterizerState,
                    _renderState.Effect ?? _renderService.DefaultEffect,
                    _renderState.World
                );

                batchStarted = true;
            }

            if(drawable is ISpriteBatchDrawable)
            {
                _spriteBatch.Draw(
                    (drawable as ISpriteBatchDrawable).SourceData ?? _renderService.DefaultTexture,
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
                } 
                else 
                {
                    _spriteBatch.DrawString(
                        fontDrawable.SourceData,
                        fontDrawable.Text,
                        drawable.Position,
                        drawable.Color);
                }
            }
        }

        if(batchStarted)
        {
            _spriteBatch.End();
        }

        Reset();
    }
}

public class YPositionDrawableComparer : IComparer<IBatchDrawable>
{
    public int Compare(IBatchDrawable? x, IBatchDrawable? y)
    {
        return x.Position.Y.CompareTo(y.Position.Y);
    }
}   

*/