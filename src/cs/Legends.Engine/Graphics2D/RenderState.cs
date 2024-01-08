using Microsoft.Xna.Framework.Graphics;
using System;
namespace Legends.Engine.Graphics2D;

public class RenderState : IComparable<RenderState>
{
    public SpriteSortMode SpriteSortMode { get; set; }
    public BlendState BlendState { get; set; }
    public SamplerState SamplerState { get; set; }
    public DepthStencilState DepthStencilState { get; set; }
    public RasterizerState RasterizerState { get; set; }
    public Effect Effect { get; set; }

    public int CompareTo(RenderState other)
    {
        if(other != null)
        {
            if(this.SpriteSortMode != other.SpriteSortMode) return -1;
            if(this.BlendState != other.BlendState) return -1;
            if(this.SamplerState != other.SamplerState) return -1;
            if(this.DepthStencilState != other.DepthStencilState) return -1;
            if(this.RasterizerState != other.RasterizerState) return -1;
            if(this.Effect != other.Effect) return -1;
            return 0;
        }

        return -1;    
    }

    public void CopyFrom(RenderState other)
    {
        this.SpriteSortMode = other.SpriteSortMode;
        this.BlendState = other.BlendState;
        this.SamplerState = other.SamplerState;
        this.DepthStencilState = other.DepthStencilState;
        this.RasterizerState = other.RasterizerState;
        this.Effect = other.Effect;
    }
}