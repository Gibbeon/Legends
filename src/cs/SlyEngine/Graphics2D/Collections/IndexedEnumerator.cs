using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SlyEngine.Graphics.Collections;

public struct IndexedEnumerator<TType> : IEnumerator<TType>
{
    private IList<TType> _items;
    private IList<int> _index;

    // Enumerators are positioned before the first element
    // until the first MoveNext() call.
    private int _current;

    internal IndexedEnumerator(IList<TType> list, IList<int> index)
    {
        _items = list;
        _index = index;
        _current = -1;
    }

    public bool MoveNext()
    {
        _current++;
        return (_current < _index.Count);
    }

    public void Reset()
    {
        _current = -1;
    }

    public void Dispose()
    {

    }

    object? System.Collections.IEnumerator.Current
    {
        get
        {
            return Current;
        }
    }

    public TType Current
    {
        get
        {
            return _items[_index[_current]];
        }
    }
}
