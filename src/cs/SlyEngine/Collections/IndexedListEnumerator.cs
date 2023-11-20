using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SlyEngine.Collections;

public struct IndexedListEnumerator<TType> : IEnumerator<TType>
{
    private IndexedList<TType> _list;
    // Enumerators are positioned before the first element
    // until the first MoveNext() call.
    private int _current;

    public IndexedListEnumerator(IndexedList<TType> list)
    {
        _list = list;
        _current = -1;
    }

    public bool MoveNext()
    {
        _current++;
        return (_current < _list.Count);
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
            return _list[_current];
        }
    }
}
