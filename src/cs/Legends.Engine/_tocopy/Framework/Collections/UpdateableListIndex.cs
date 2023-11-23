using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace LitEngine.Framework.Collections
{
    public class UpdateableListIndex<TType> : BaseListIndex<TType>
        where TType : IUpdateable
    {
        public UpdateableListIndex(List<TType> list) : base(list)
        {
            _comparer = new UpdateableOrderComparerIndexed<TType>(_list);
        }
        public override void SetValue(int index, TType value)
        {
            if (_list[_index[index]].UpdateOrder != value.UpdateOrder)
            {
                OrderHasChanged = true;
            }
        }

        protected override void AddOnChangeEvent(TType item) {
            item.UpdateOrderChanged += OnOrderHasChanged;
        }
        protected override void RemoveOnChangeEvent(TType item) {
            
            item.UpdateOrderChanged -= OnOrderHasChanged;
        }
       
        private int FindUpdateableIndex(TType item)
        {
            return FindUpdateableIndex(item, 0, _list.Count);
        }

        private int FindUpdateableIndex(TType item, int min, int max)
        {
            if (min >= max)
            {
                return min;
            }
            else
            {
                var current = (max - min) / 2;

                if (_list[current].UpdateOrder == item.UpdateOrder)
                {
                    return current;
                }
                else if (_list[current].UpdateOrder < item.UpdateOrder)
                {
                    return FindUpdateableIndex(item, current, max);
                }
                else //if (array[min].UpdateOrder > child.UpdateOrder) this is alwasy true
                {
                    return FindUpdateableIndex(item, min, current);
                }
            }
        }
    }
}
