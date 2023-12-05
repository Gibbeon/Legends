﻿using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.ComponentModel;
using System;

namespace Legends.Engine.Graphics2D;

public class SpriteRenderBehavior : BaseBehavior, ISpriteBatchDrawable
{
    public TextureRegion2D TextureRegion { get; set; }

    public Color Color { get; set; }

    public RenderState RenderState { get; set; }

    [DefaultValue(SpriteEffects.None)]
    public SpriteEffects Effect  { get; set; }

    [JsonIgnore]
    public Texture2D SourceData => TextureRegion?.Texture;

    [JsonIgnore]
    public bool IsVisible   => Parent != null && Parent.IsVisible;

    [JsonIgnore]
    public Vector2 Position => Parent != null ? Parent.Position : Vector2.Zero;

    [JsonIgnore]
    public float Rotation   => Parent != null ? Parent.Rotation : 0.0f;
    
    [JsonIgnore]
    public Vector2 Scale    => Parent != null ? Parent.Scale : Vector2.One;
    
    [JsonIgnore]
    public Vector2 Origin   => Parent != null ? Parent.Origin : Vector2.Zero;
    
    [JsonIgnore]
    public IViewState ViewState => Parent?.GetParentScene()?.Camera;

    [JsonIgnore]
    public Rectangle SourceBounds{ get => TextureRegion == null ? Rectangle.Empty : TextureRegion.Bounds; set => SetTextureRegionBounnds(value); }

    [JsonIgnore]
    public Rectangle DestinationBounds => Parent == null ? Rectangle.Empty : (Rectangle)Parent.BoundingRectangle; 

    public SpriteRenderBehavior(): this (null, null)
    {

    }
    public SpriteRenderBehavior(IServiceProvider services, SceneObject parent) : base(services, parent)
    {
        Color = Color.White;
    }

    public override void Draw(GameTime gameTime)
    {
        if(IsVisible)
        {
            Parent?.Services?.Get<IRenderService>().DrawBatched(this);
        }
    }

    public override void Update(GameTime gameTime)
    {
        //base.Update(gameTime);
    }


    public void SetTextureRegionBounnds(Rectangle rect)
    {
        TextureRegion = new TextureRegion2D(TextureRegion?.Texture, rect);
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}