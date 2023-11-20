using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SlyEngine.Graphics.Collections;
public class UpdateableOrderComparerIndexed<TType> : IComparer<int>
where TType : IUpdateable
{
    IList<TType> _array;

    public UpdateableOrderComparerIndexed(IList<TType> array) 
    {
        _array = array;
    }

    public int Compare(int a, int b)
    {
        return _array[a].UpdateOrder.CompareTo(_array[b].UpdateOrder);
    }
}
