using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.ComponentModel;
using System;
using MonoGame.Extended;

namespace Legends.Engine.Graphics2D.Components;

public class Sprite : Component, ISpriteRenderable
{
    [JsonProperty(nameof(TextureRegion))]
    protected Ref<TextureRegion> TextureRegionReference { get; set; }

    public Color Color { get; set; }

    public RenderState RenderState { get; set; }

    public SpriteEffects SpriteEffect  { get; set; }

    [JsonIgnore]
    public bool Visible   => Parent.Visible;

    [JsonIgnore]
    public Vector2 Position => Parent.Position;
    
    [JsonIgnore]
    public TextureRegion TextureRegion => TextureRegionReference.Get();
    
    [JsonIgnore]
    public IViewState ViewState => Parent.Scene.Camera;


    public Sprite(): this (null, null)
    {

    }
    public Sprite(IServiceProvider services, SceneObject parent) : base(services, parent)
    {
        Color = Color.White;
    }

    public override void Draw(GameTime gameTime)
    {
        if(Visible)
        {
            Parent.Services.Get<IRenderService>().DrawBatched(this);
        }
    }

     public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public override void Initialize()
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

        spriteBatch.Draw(
            TextureRegion.Texture,
            (Rectangle)Parent.BoundingRectangle,
            (Rectangle)TextureRegion.BoundingRectangle,
            Color,
            Parent.Rotation,
            Vector2.Zero,//drawable.Origin,
            SpriteEffect,
            0);

        if(target == null && spriteBatch != null)
            spriteBatch.End();
    }
}