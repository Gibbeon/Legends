using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
namespace Legends.Engine;

 public class RenderState : IComparable<RenderState>
{
    public SpriteSortMode SpriteSortMode { get; set; }
    public BlendState? BlendState { get; set; }
    public SamplerState? SamplerState { get; set; }
    public DepthStencilState? DepthStencilState { get; set; }
    public RasterizerState? RasterizerState { get; set; }
    public Effect? Effect { get; set; }
    public Matrix? World { get; set; }
    public Matrix? View { get; set; }
    public Matrix? Projection { get; set; }

    public int CompareTo(RenderState? other)
    {
        if(other != null)
        {
            if(this.SpriteSortMode != other.SpriteSortMode) return -1;
            if(this.BlendState != other.BlendState) return -1;
            if(this.SamplerState != other.SamplerState) return -1;
            if(this.DepthStencilState != other.DepthStencilState) return -1;
            if(this.RasterizerState != other.RasterizerState) return -1;
            if(this.Effect != other.Effect) return -1;
            if(this.World != other.World) return -1;
            if(this.View != other.View) return -1;
            if(this.Projection != other.Projection) return -1;
        }

        return -1;    
    }

    public void CopyTo(RenderState state)
    {
        state.SpriteSortMode = this.SpriteSortMode;
        state.BlendState = this.BlendState;
        state.SamplerState = this.SamplerState;
        state.DepthStencilState = this.DepthStencilState;
        state.RasterizerState = this.RasterizerState;
        state.Effect = this.Effect;
        state.World = this.World;
        state.View = this.View;
        state.Projection = this.Projection;
    }
}