using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.ComponentModel;
using System;
using MonoGame.Extended;

namespace Legends.Engine.Graphics2D.Components;

public class Sprite : Component2D, ISpriteRenderable
{
    [JsonIgnore]
    public int RenderLayerID => 1;

    [JsonProperty(nameof(TextureRegion))]
    protected Ref<TextureRegion> TextureRegionReference { get; set; }

    public Color Color { get; set; }

    public RenderState RenderState { get; set; }

    public SpriteEffects SpriteEffect  { get; set; }

    [JsonIgnore] public bool            Visible  => Parent.Visible;
    [JsonIgnore] public Vector2         Position => Parent.Position;
    [JsonIgnore] public TextureRegion   TextureRegion => TextureRegionReference.Get();
    [JsonIgnore] public IViewState      ViewState => Parent.Scene.Camera;

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
            Services.Get<IRenderService>().DrawBatched(this);
        }
    }

     public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public override void Initialize()
    {
        SetSize(TextureRegion.Slice);
    }

    public override void Reset()
    {
        
    }

    public void DrawImmediate(GameTime gameTime, GraphicsResource target = null)
    {
        var spriteBatch = this.GetSpriteBatch(target);

        spriteBatch.Draw(
            TextureRegion.Texture,
            Parent.Position - Origin,
            (Microsoft.Xna.Framework.Rectangle)TextureRegion.FrameBoundingRectangle,
            Color,
            Parent.Rotation,
            Origin,
            Parent.Scale,
            SpriteEffect,
            0);

        if(target is not SpriteBatch)
            spriteBatch?.End();
    }
}