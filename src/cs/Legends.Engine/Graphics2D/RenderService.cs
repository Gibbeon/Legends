using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System;
using MonoGame.Extended;
using System.Text.Json.Serialization;
using SharpFont.PostScript;

namespace Legends.Engine.Graphics2D;

public interface IRenderer : IInitalizable
{
    IServiceProvider Services { get; }
    GraphicsDevice  GraphicsDevice { get; }
    void DrawItem(IRenderable drawable);
}

public interface IRenderService : IRenderer
{
    void Draw(GameTime gameTime);
}

public class DefaultRenderService : IRenderService
{
    public IServiceProvider Services    { get; private set; }
    public GraphicsDevice GraphicsDevice => Services.GetGraphicsDevice();
    public RenderSurface RenderSurface  { get; set; }

    public DefaultRenderService(IServiceProvider services)
    {
        Services = services;
        Services.Add<IRenderService>(this);
    }
    public void Dispose()
    {
    
    }

    public void Initialize()
    {
        Reset();
    }    

    public void Reset()
    {
        RenderSurface = new RenderSurface(Services);
        RenderSurface.Initialize();
    }
    public void Draw(GameTime gameTime)
    {
        RenderSurface.Begin();
        RenderSurface.Draw(gameTime);
        RenderSurface.End();

        //GraphicsDevice.Present();
    }
    public void DrawItem(IRenderable drawable)
    {
        RenderSurface.Drawables.Add(drawable);
    }
}

public class ClearState
{
    public ClearOptions     Options = ClearOptions.Target;
    public Color            Color;
    public int              Depth;
    public int              StencilDepth;
}


public class RenderSurface
{    
    private IComparer<IRenderable> _drawableComparer;
    [JsonIgnore] public IServiceProvider Services   { get; protected set; }
    [JsonIgnore] public GraphicsDevice GraphicsDevice => Services.GetGraphicsDevice();     
    [JsonIgnore] public IList<IRenderable> Drawables{ get; set; }
    [JsonIgnore] public SpriteBatch SpriteBatch     { get; protected set; }

    public ClearState       ClearState              { get; set; }
    public RenderState      RenderState             { get; set; }
    public RenderTarget2D   RenderTarget            { get; set; }
    public Effect           DefaultEffect           { get; set; }       
    public IComparer<IRenderable> DrawableComparer  { get =>_drawableComparer ?? Comparer<IRenderable>.Default; set => _drawableComparer = value; }

    public RenderSurface(IServiceProvider services)
    {
        Services = services;
    }

    public void Initialize()
    {
        Drawables             = new List<IRenderable>();
        RenderState         ??= new ();
        SpriteBatch         ??= new SpriteBatch(GraphicsDevice);
        ClearState          ??= new();
        RenderState.Effect  ??= DefaultEffect ??= new BasicEffect(GraphicsDevice)
        {
            VertexColorEnabled = true,
            TextureEnabled = true
        };
    }

    public void Begin()
    {
        GraphicsDevice.SetRenderTarget(RenderTarget);
        GraphicsDevice.Clear(ClearState.Options, ClearState.Color, ClearState.Depth, ClearState.StencilDepth);
    }

    public void Draw(GameTime gameTime) 
    {
        var currentState = GraphicsDevice.ApplyState(RenderState);
        var batchStarted = false;

        foreach(var drawable in Drawables
            .Where  (n => IsVisible(n))
            .OrderBy(n => n.RenderLayerID)
            .OrderBy(n => DrawableComparer))
        {
            
            var currentEffect       = drawable.RenderState?.Effect ?? DefaultEffect;
            GraphicsDevice.Viewport = drawable.ViewState.Viewport;

            if (currentEffect is IEffectMatrices mtxEffect)
            {
                mtxEffect.View          = drawable.ViewState.View;
                mtxEffect.Projection    = drawable.ViewState.Projection;
                mtxEffect.World         = drawable.ViewState.World;
            }

            if(!batchStarted || currentState != drawable.RenderState)
            {
                if(batchStarted) 
                {
                    SpriteBatch.End();
                    batchStarted = false;
                }

                currentState = GraphicsDevice.ApplyState(drawable.RenderState ?? RenderState);

                SpriteBatch.Begin(
                    currentState.SpriteSortMode,
                    currentState.BlendState,                    
                    currentState.SamplerState,
                    currentState.DepthStencilState,
                    currentState.RasterizerState,
                    currentEffect,
                    null
                );
                batchStarted = true;
            }

           
            drawable.DrawImmediate(gameTime, this);            
        }

        if(batchStarted) 
        {
            SpriteBatch.End();
            batchStarted = false;
        }
    }

    public bool IsVisible(IRenderable renderable)
    {
        return renderable.Visible;
    }

    public void End() 
    {
        Drawables.Clear();
    }
}

public static class GraphicDeviceExtensions
{
    public static RenderState ApplyState(this GraphicsDevice device, RenderState renderState)
    {
        if(renderState == null) return default(RenderState);

        device.BlendState           = renderState.BlendState        ?? BlendState.Opaque;
        device.SamplerStates[0]     = renderState.SamplerState      ?? SamplerState.LinearWrap;
        device.DepthStencilState    = renderState.DepthStencilState ?? DepthStencilState.Default;
        device.RasterizerState      = renderState.RasterizerState   ?? RasterizerState.CullCounterClockwise;

        return renderState;
    }
}


/*
public class RenderService : IRenderService
{
    public IServiceProvider Services { get; private set; }
    public RenderState DefaultRenderState { get; set; }
    public Texture2D DefaultTexture { get; private set; }
    public GraphicsDevice GraphicsDevice => Services.GetGraphicsDevice();
    private readonly List<IRenderLayer> _layers;

    public RenderService(IServiceProvider services)
    {
        Services = services;
        Services.Add<IRenderService>(this);
        DefaultRenderState = new RenderState();

        _layers = new List<IRenderLayer>();
    }

    public void Initialize()
    {
        if (DefaultTexture == null || DefaultRenderState.Effect == null)
        {
            DefaultTexture = new Texture2D(GraphicsDevice, 1, 1);
            DefaultTexture.SetData([Color.Green]);

            DefaultRenderState.Effect = new BasicEffect(GraphicsDevice)
            {
                VertexColorEnabled = true,
                TextureEnabled = true
            };
        }

        _layers.Add(new RenderLayer(this) { ClearColor = Color.Black });
        _layers[0].Initialize();

    }

    public IRenderLayer GetLayer(int layerId)
    {
        if(_layers.Count <= layerId)
        {
            foreach(var layer in Enumerable.Repeat(new RenderLayer(this), layerId - _layers.Count + 1))
            {
                _layers.Add(layer);
                layer.Initialize();
            }
        }

        return _layers[layerId];
    }

    public void Draw(GameTime gameTime)
    {
        foreach(var layer in _layers)
        {
            layer.BeginDraw();
            layer.DrawImmediate(gameTime);
            layer.EndDraw();
        }
    }


    public void DrawBatched(IRenderable renderable)
    {
        GetLayer(renderable.RenderLayerID).Enqueue(renderable);
    }

    public void Reset()
    {

    }

    public void Dispose()
    {
        
    }
}
*/
