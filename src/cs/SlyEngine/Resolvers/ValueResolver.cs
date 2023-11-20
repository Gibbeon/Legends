using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace SlyEngine.Resolvers;

public class ValueResolver<TKeyType, TType> where TKeyType: notnull
{
    public IDictionary<TKeyType, IList<Tuple<Func<TType, bool>, TKeyType>>> _lookup;

    public ValueResolver()
    {
        _lookup = new Dictionary<TKeyType, IList<Tuple<Func<TType, bool>, TKeyType>>>();
    }

    public TKeyType Resolve(TKeyType key, TType value)
    {
        if(!_lookup.ContainsKey(key)) return key;
        foreach(var item in _lookup[key])
        {
            if(item.Item1(value))
            {
                return item.Item2;
            }
        }

        return key;
    }

    public void Add(TKeyType name, Func<TType, bool> resolver, TKeyType result)
    {
        if(!_lookup.ContainsKey(name))
        {
            _lookup.Add(name, new List<Tuple<Func<TType, bool>, TKeyType>>());
        }

        _lookup[name].Add(Tuple.Create(resolver, result));
    }
}
