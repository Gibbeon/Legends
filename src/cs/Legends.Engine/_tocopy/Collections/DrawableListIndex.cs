using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Legends.Engine.Graphics.Collections;

/*
public class DrawableListIndex<TType> : BaseListIndex<TType>
    where TType : IDrawable
{
    public DrawableListIndex(List<TType> list) : base(list)
    {
        _comparer = new DrawableOrderComparerIndexed<TType>(_list);
    }

    protected override void AddOnChangeEvent(TType item) {
        item.DrawOrderChanged += OnOrderHasChanged;
    }
    protected override void RemoveOnChangeEvent(TType item) {
        
        item.DrawOrderChanged -= OnOrderHasChanged;
    }
    
    public override void SetValue(int index, TType value)
    {
        OrderHasChanged = (_list[_index[index]].DrawOrder != value.DrawOrder);
        
        var indexOf = _list.IndexOf(value);
        //var oldValue = _index[index]; things can memory leak here, see optimize

        if(indexOf > -1) {
            _index[index] = indexOf;
        } else {
            _index[index] = _list.Count;
            _list.Add(value);
            AddOnChangeEvent(value);
        }
    }

    private int FindDrawableIndex(TType item)
    {
        return FindDrawableIndex(item, 0, _list.Count);
    }

    private int FindDrawableIndex(TType item, int min, int max)
    {
        if (min >= max)
        {
            return min;
        }
        else
        {
            var current = (max - min) / 2;

            if (_list[current].DrawOrder == item.DrawOrder)
            {
                return current;
            }
            else if (_list[current].DrawOrder < item.DrawOrder)
            {
                return FindDrawableIndex(item, current, max);
            }
            else //if (array[min].UpdateOrder > child.UpdateOrder) this is alwasy true
            {
                return FindDrawableIndex(item, min, current);
            }
        }
    }
}
*/