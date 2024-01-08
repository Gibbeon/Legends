using System.Collections.Generic;

namespace Legends.Engine.Graphics2D;
public class PositionRenderableComparer : IComparer<ISpriteRenderable>
{
    public int Compare(ISpriteRenderable x, ISpriteRenderable y)
    {
        if (x == null || y == null)
        {
            return Comparer<ISpriteRenderable>.Default.Compare(x, y);
        }
        else
        {
            return x.Position.Y.CompareTo(y.Position.Y);
        }
    }
}

