using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MonoGame.Extended.ViewportAdapters;

namespace Legends.Engine.Graphics2D;

public interface IRenderService : IInitalizable
{
    GraphicsDevice  GraphicsDevice { get; }
    RenderState     DefaultRenderState { get; }
    Texture2D       DefaultTexture { get; }
    Viewport        DefaultViewport { get; }
    RenderTarget2D  RenderTarget { get; }
    Color?          ClearColor { get; }
    void DrawRenderable(IRenderable drawable);
    void Update(GameTime gameTime);
    void Draw(GameTime gameTime);
    void SwapBuffers();
}

public class DefaultRenderService: IRenderService
{
    GraphicsDevice  GraphicsDevice { get; }
    RenderState     DefaultRenderState { get; }
    Texture2D       DefaultTexture { get; }
    ViewportAdapter ViewportAdapter { get; }
    RenderTarget2D  RenderTarget { get; }
    ClearOptions    ClearOptions { get; }
    Color           ClearColor { get; }
    int             ClearDepth { get; }
    int             StencilDepth { get; }

    public void Begin()
    {
        GraphicsDevice.SetRenderTarget(RenderTarget);
        GraphicsDevice.Viewport = ViewportAdapter.Viewport;
        GraphicsDevice.Clear(ClearOptions, ClearColor, ClearDepth, StencilDepth);
        GraphicsDevice.ApplyState(DefaultRenderState);
    }
}

public static class GraphicDeviceExtensions
{
    public static void ApplyState(this GraphicsDevice device, RenderState renderState)
    {
        if(renderState == null) return;

        device.BlendState           = renderState.BlendState;
        device.SamplerStates[0]     = renderState.SamplerState;
        device.DepthStencilState    = renderState.DepthStencilState;
        device.RasterizerState      = renderState.RasterizerState;
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
