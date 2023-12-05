using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace LitEngine.Framework.Collections
{   
    public class DrawableOrderComparer : IComparer<IDrawable>
    {
        public static DrawableOrderComparer Default = new DrawableOrderComparer();

        public int Compare(IDrawable a, IDrawable b)
        {
            if(a == null || b == null) {
                return -1;
            }

            return a.DrawOrder.CompareTo(b.DrawOrder);
        }
    }    
}
