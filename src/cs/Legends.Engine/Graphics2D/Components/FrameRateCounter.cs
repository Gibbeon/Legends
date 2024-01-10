using System;
using System.Transactions;
using Legends.Engine.Graphics2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using Newtonsoft.Json;

namespace Legends.Engine.Graphics2D.Components;

public class FrameRateCounter : Component, ISpriteRenderable
{        
    [JsonProperty(nameof(Font))]
    protected Ref<BitmapFont>      FontReference { get; set; }

    [JsonIgnore]
    public BitmapFont Font => FontReference.Get();

    int _frameRate = 0;
    int _frameCounter = 0;
    TimeSpan _elapsedTime = TimeSpan.Zero;

    public bool Visible { get; set; }

    public RenderState RenderState => null;

    [JsonIgnore]
    public IViewState ViewState => _camera;

    public Vector2 Position => new Vector2(32, 32);
    public Color Color { get => Color.White; set {}}

    private Camera _camera;

    public FrameRateCounter(IServiceProvider services, 
        SceneObject parent) : base (services, parent)
    {
        Visible = true;

    }

    public override void Initialize()
    {
        _camera = new Camera(Services, null);
        _camera.Initialize();        
        _camera.OriginNormalized = Vector2.Zero;
    }


    public override void Update(GameTime gameTime)
    {
        _elapsedTime += gameTime.ElapsedGameTime;

        if (_elapsedTime > TimeSpan.FromSeconds(1))
        {
            _elapsedTime -= TimeSpan.FromSeconds(1);
            _frameRate = _frameCounter;
            _frameCounter = 0;
        }

        _camera.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        if(Visible)
        {
            Services.Get<IRenderService>().DrawBatched(this);
        }
    }

    public override void Dispose()
    {
    }

    public override void Reset()
    {
    }

    public void DrawImmediate(GameTime gameTime, GraphicsResource target = null)
    {
        if(target is not SpriteBatch spriteBatch)
        {
            spriteBatch = new SpriteBatch(Services.GetGraphicsDevice());
            
            if (RenderState?.Effect is IEffectMatrices mtxEffect)
            {
                mtxEffect.View = ViewState.View;
                mtxEffect.Projection = ViewState.Projection;
                mtxEffect.World = ViewState.World;
            }

            spriteBatch.Begin(
                SpriteSortMode.Immediate,
                RenderState?.BlendState,
                RenderState?.SamplerState,
                RenderState?.DepthStencilState,
                RenderState?.RasterizerState,
                RenderState?.Effect,
                null
            );
        }

        _frameCounter++;

        string fps = string.Format("fps: {0}", _frameRate);

        spriteBatch.DrawString(Font, fps, Position, Color);

        if(target == null && spriteBatch != null)
            spriteBatch.End();
    }
}