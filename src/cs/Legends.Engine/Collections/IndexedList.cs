using System;
using System.Linq;
using System.Collections.Generic;

namespace Legends.Engine.Collections;
public class IndexedList<TType> : IList<TType>, IReadOnlyList<TType>
{
    protected List<TType>      _list;
    protected List<int>        _index;
    
    public TType this[int index]
    {
        get => GetValue(index);
        set => SetValue(index, value);
    }

    public IReadOnlyList<TType> Items { get => _list.AsReadOnly(); }
    public IReadOnlyList<int>   Index { get => _index.AsReadOnly(); }

    public int Count
    {
        get => _index.Count;
    }

    bool ICollection<TType>.IsReadOnly
    {
        get => (_index as ICollection<int>).IsReadOnly;
    }

    public IndexedList() : this(new List<TType>())
    {

    }
    
    public IndexedList(IList<TType> list)
    {
        _list = list.ToList();
        _index = new List<int>();

        if(list.Count > 0) {
            _index.AddRange(Enumerable.Range(0, list.Count));
        }
    }

    public virtual void Add(TType item)
    {
        var index = _list.IndexOf(item);
        
        if (index < 0)
        {
            index = _list.Count;
            _list.Add(item);
        }

        _index.Add(index);
    }

    public virtual void RemoveAt(int itemIndex)
    {
        if(itemIndex < 0) return;
        _index.RemoveAt(itemIndex); //creates orphins
    }

    public TType GetValue(int index)
    {
        return _list[_index[index]];
    }

    public void SetValue(int index, TType item)
    {
        var itemIndex = _list.IndexOf(item);
        
        if (itemIndex < 0)
        {
            itemIndex = _list.Count;
            _list.Add(item);
        }

        _index[index] = itemIndex;
    }

    public int IndexOf(TType item)
    {
        return _list.IndexOf(item);

    }
    public void Insert(int index, TType item)
    {
        Add(item);
    }

    public void Clear()
    {
        _list.Clear();
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
        return new IndexedListEnumerator<TType>(this);
    }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
    public void CopyTo(TType[] array, int arrayIndex)
    {
        foreach (var item in this)
        {
            array[arrayIndex] = item;
            arrayIndex++;
        }
    }

}