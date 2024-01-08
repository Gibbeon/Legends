using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.BitmapFonts;
using System;

namespace Legends.Engine.Graphics2D;

public interface IRenderService : IInitalizable
{
    GraphicsDevice  GraphicsDevice { get; }
    RenderState     DefaultRenderState { get; }
    Texture2D       DefaultTexture { get; }
    void DrawBatched(IRenderable drawable);
}


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
            DefaultTexture.SetData(new Color[] { Color.Green });

            DefaultRenderState.Effect = new BasicEffect(GraphicsDevice)
            {
                VertexColorEnabled = true,
                TextureEnabled = true
            };
        }

        _layers.Add(new RenderLayer(this) { ClearColor = Color.Black });
    }

    public void Draw(GameTime gameTime)
    {
        _layers[0].BeginDraw();
        _layers[0].DrawImmediate(gameTime);
        _layers[0].EndDraw();
    }


    public void DrawBatched(IRenderable drawable)
    {
        _layers[0].Enqueue(drawable);
    }

    public void Reset()
    {

    }

    public void Dispose()
    {
        
    }
}
