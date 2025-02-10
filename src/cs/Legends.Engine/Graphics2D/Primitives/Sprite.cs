using Autofac;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Newtonsoft.Json;
using System;

namespace Legends.Engine.Graphics2D.Primitives;

public class Sprite : Drawable
{
    public Texture2DRegion              TextureRegion { get; set; }
    public int                          FrameIndex { get; set;}
    public bool                         FlipHorizontally { get; set; }
    public bool                         FlipVertically { get; set;}
    
    [JsonIgnore] public override RectangleF  BoundingRectangle => new(Vector2.Zero - Origin, Size);

    public Sprite(IServiceProvider services, string assetName): base(services, assetName)
    {

    }

    public override void DrawTo(RenderSurface target, Vector2 drawTo, float rotation = 0.0f)
    {
        target.SpriteBatch.Draw(
            TextureRegion.Texture,
            (Microsoft.Xna.Framework.Rectangle)new RectangleF(drawTo, Size),
            TextureRegion[FrameIndex],
            Color,
            rotation,
            Vector2.Zero,
            (FlipHorizontally ? SpriteEffects.FlipHorizontally : SpriteEffects.None) | 
            (FlipVertically   ? SpriteEffects.FlipVertically   : SpriteEffects.None),
            0
        );
    }

    public override void Initialize()
    {
    }

    public override void Reset()
    {
    }

    public override void Dispose()
    {
    }
}