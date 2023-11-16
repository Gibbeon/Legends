using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Legends.App.Collections
{  
    public class DrawableOrderComparerIndexed<TType> : IComparer<int>
    where TType : IDrawable
    {
        IList<TType> _array;

        public DrawableOrderComparerIndexed(IList<TType> array) 
        {
            _array = array;
        }

        public int Compare(int a, int b)
        {
            return _array[a].DrawOrder.CompareTo(_array[b].DrawOrder);
        }
    }
}
