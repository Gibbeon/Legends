using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.ComponentModel;
using System;
using MonoGame.Extended;

namespace Legends.Engine.Graphics2D.Components;

public class Sprite : Component, ITexturedSpriteRenderable
{
    [JsonProperty(nameof(TextureRegion))]
    protected Ref<TextureRegion> TextureRegionReference { get; set; }

    public Color Color { get; set; }

    public RenderState RenderState { get; set; }

    public SpriteEffects Effect  { get; set; }

    [JsonIgnore]
    public TextureRegion SourceData => TextureRegion;

    [JsonIgnore]
    public bool Visible   => Parent.Visible;

    [JsonIgnore]
    public Vector2 Position => Parent.Position;

    [JsonIgnore]
    public float Rotation   => Parent.Rotation;
    
    [JsonIgnore]
    public Vector2 Scale    => Parent.Scale;
    
    [JsonIgnore]
    public Vector2 Origin   =>Parent.Origin;

    [JsonIgnore]
    public TextureRegion TextureRegion => TextureRegionReference.Get();
    
    [JsonIgnore]
    public IViewState ViewState => Parent.Scene.Camera;

    [JsonIgnore]
    public Rectangle SourceBounds{ get => (Rectangle)TextureRegion.BoundingRectangle; }

    [JsonIgnore]
    public Rectangle DestinationBounds => (Rectangle)Parent.BoundingRectangle; 

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

    public override void Update(GameTime gameTime)
    {

    }
}