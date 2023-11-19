using System;
using System.Linq;
using System.Collections.Generic;

namespace LitEngine.Framework.Collections
{
    public abstract class BaseListIndex<TType> : IList<TType>, IReadOnlyList<TType>
    {
        protected List<TType> _list;
        protected List<int> _index;
        protected IComparer<int>? _comparer;
        public TType this[int index]
        {
            get => GetValue(index);
            set => SetValue(index, value);
        }

        public int Count
        {
            get => _index.Count;
        }

        bool ICollection<TType>.IsReadOnly
        {
            get => (_index as ICollection<int>).IsReadOnly;
        }

        public abstract void SetValue(int index, TType value);

        protected abstract void AddOnChangeEvent(TType item);
        protected abstract void RemoveOnChangeEvent(TType item);

        public virtual void Add(TType item)
        {
            var index = _list.IndexOf(item);
            
            if (index < 0)
            {
                index = _list.Count;
                _list.Add(item);
            }
                            
            AddOnChangeEvent(item);

            _index.Add(index);
            OrderHasChanged = true;
        }

        public virtual void RemoveAt(int itemIndex)
        {
            if (itemIndex < 0) return;
            
            RemoveIndexOnlyAt(itemIndex);
                     
            RemoveOnChangeEvent(_list[itemIndex]);    
            _list.RemoveAt(itemIndex);
        }

        public TType GetValue(int index)
        {
            return _list[_index[index]];
        }
        public int IndexOf(TType item)
        {
            return _list.IndexOf(item);

        }
        public void Insert(int index, TType item)
        {
            Add(item);
        }

        public bool OrderHasChanged
        {
            get;
            protected set;
        }
        public void Clear()
        {
            _index.Clear();
        }
        public bool Contains(TType item)
        {
            return _list.Contains(item);
        }
        public bool Remove(TType item)
        {
            var itemIndex = _list.IndexOf(item);
            RemoveAt(itemIndex);
            return itemIndex > -1;
        }

        public int CountOrphans() {
            int count = 0;
            bool keep = false;
            for(var itemIndex = _list.Count - 1; itemIndex >= 0; itemIndex--) {
                for(var indexer = _index.Count - 1; indexer >= 0; indexer--) {
                    keep = _index[indexer] == itemIndex;
                    if(keep) break;
                }
                if(!keep) {
                    count++;
                }
            }
            return count;
        }

        public int CullOrphans() {            
            int count = 0;
            bool keep;
            for(var itemIndex = _list.Count - 1; itemIndex >= 0; itemIndex--) {
                keep = true;
                for(var indexer = _index.Count - 1; indexer >= 0; indexer--) {
                    keep = _index[indexer] == itemIndex;
                    if(keep) break;
                }
                if(!keep) {
                    count++;
                    _list.RemoveAt(itemIndex);
                    ShiftLeft(itemIndex);
                }
            }            
            return count;
        }

        public bool RemoveIndexOnlyAt(int itemIndex)
        {
            if (itemIndex < 0) return false;
            
            _index.RemoveAll((int value)=> { return value == itemIndex; });
            ShiftLeft(itemIndex);
            return true;
        }

        public void ShiftLeft(int index, int count = 1) {
            for(var i = 0; i < _index.Count; i++) {
                if(_index[i] > index) {
                    _index[i] -= count;
                }
            }   
        }
        public void ShiftRight(int index, int count = 1) {
            for(var i = 0; i < _index.Count; i++) {
                if(_index[i] > index) {
                    _index[i] += count;
                }
            }   
        }

        public IEnumerator<TType> GetEnumerator()
        {
            Update();
            return new IndexedEnumerator<TType>(_list, _index);
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        public void CopyTo(TType[] array, int arrayIndex)
        {
            // should this be ordered copy? probably
            //_list.CopyTo(array, arrayIndex);

            foreach (var item in this)
            {
                array[arrayIndex] = item;
                arrayIndex++;
            }
        }
        protected void OnOrderHasChanged(object? sender = null, EventArgs? args = null)
        {
            OrderHasChanged = true;
        }
        public BaseListIndex(List<TType> list)
        {
            _list = list;
            _index = new List<int>();

            if(list.Count > 0) {
                _index.AddRange(Enumerable.Range(0, list.Count));
                OrderHasChanged = true;
            }
        }
        public void Update()
        {
            if (OrderHasChanged)
            {
                _index.Sort(_comparer);
                OrderHasChanged = false;
            }
        }
    }
}

