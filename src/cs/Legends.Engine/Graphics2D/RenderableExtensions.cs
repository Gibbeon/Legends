using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Legends.Engine.Graphics2D;

/*
public static class RenderableExtensions
{
    public static SpriteBatch GetSpriteBatch(this IRenderable renderable, GraphicsResource target = null)
    {
        if(target is not SpriteBatch spriteBatch)
        {
            spriteBatch = new SpriteBatch(renderable.Services.GetGraphicsDevice());
            
            if (renderable.RenderState?.Effect is IEffectMatrices mtxEffect)
            {
                mtxEffect.View       = renderable.ViewState.View;
                mtxEffect.Projection = renderable.ViewState.Projection;
                mtxEffect.World      = renderable.ViewState.World;
            }

            spriteBatch.Begin(
                SpriteSortMode.Immediate,
                renderable.RenderState?.BlendState,
                renderable.RenderState?.SamplerState,
                renderable.RenderState?.DepthStencilState,
                renderable.RenderState?.RasterizerState,
                renderable.RenderState?.Effect,
                null
            );
        }

        return spriteBatch;
    }
}
*/