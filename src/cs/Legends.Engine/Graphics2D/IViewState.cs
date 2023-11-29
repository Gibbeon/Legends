using Microsoft.Xna.Framework;
using System;
namespace Legends.Engine;

public interface IViewState
{
    Matrix World { get; }
    Matrix View { get; }
    Matrix Projection { get; }    
}

public class ViewState : IViewState, IComparable<ViewState>
{
    public Matrix World { get; set; }
    public Matrix View { get; set; }
    public Matrix Projection { get; set; }
    public int CompareTo(ViewState? other)
    {
        if(other != null)
        {
            if(this.World != other.World) return -1;
            if(this.View != other.View) return -1;
            if(this.Projection != other.Projection) return -1;
            return 0;
        }

        return -1;   
    }

    public void CopyFrom(IViewState? state)
    {
        if(state != null)
        {
            this.World = state.World;
            this.View = state.View;
            this.Projection = state.Projection;
        }
    }
}