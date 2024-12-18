using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System;
using MonoGame.Extended.ViewportAdapters;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Legends.Engine.Graphics2D;

public interface IRenderService : IInitalizable
{
    GraphicsDevice  GraphicsDevice { get; }
    RenderState     DefaultRenderState { get; }
    Texture2D       DefaultTexture { get; }
    void DrawBatched(IRenderable drawable);
}

public interface IRenderPass : IInitalizable
{
    ViewportAdapter ViewportAdapter { get; } 

    void Apply();
}

public class RenderPass : IRenderPass
{
    public ViewportAdapter ViewportAdapter { get; set; }
    public RenderPass(RenderService renderService)
    {
        ViewportAdapter = new WindowViewportAdapter(renderService.Services.Get<IGameManagementService>().Game.Window, renderService.Services.GetGraphicsDevice());
    }

    public void Apply()
    {
        ViewportAdapter.GraphicsDevice.Viewport = ViewportAdapter.Viewport;
    }

    public void Initialize()
    {
        
    }

    public void Reset()
    {
        
    }

    public void Dispose()
    {
        
    }
}

public class RenderService : IRenderService
{
    public IServiceProvider Services { get; private set; }
    public RenderState DefaultRenderState { get; set; }
    public Texture2D DefaultTexture { get; private set; }
    public GraphicsDevice GraphicsDevice => Services.GetGraphicsDevice();
    private readonly List<IRenderLayer> _layers;
    private readonly List<IRenderPass> _passes;

    public RenderService(IServiceProvider services)
    {
        Services = services;
        Services.Add<IRenderService>(this);
        DefaultRenderState = new RenderState();

        _layers = new List<IRenderLayer>();
        _passes = new List<IRenderPass>();
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

        _layers.Add(new RenderLayer(this) { ClearColor = Color.Black });
        _layers[0].Initialize();

        _passes.Add(new RenderPass(this));
        _passes[0].Initialize();
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
        foreach(var pass in _passes)
        {
            pass.Apply();

            foreach(var layer in _layers)
            {
                layer.BeginDraw();
                layer.DrawImmediate(gameTime);
                layer.EndDraw();
            }
        }
    }


    public void DrawBatched(IRenderable drawable)
    {
        GetLayer(drawable.RenderLayerID).Enqueue(drawable);
    }

    public void Reset()
    {

    }

    public void Dispose()
    {
        
    }
}
