using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System;
using Microsoft.Xna.Framework.Content;

namespace Legends.Engine.Graphics2D;

public class Asset
{
    public string Name { get; protected set; }

    public Asset() : this(String.Empty)
    {
        
    }
    public Asset(string name)
    {
        Name = name;
    }
}
public class Asset<TType> : Asset
{
    TType? _local;
    public TType? Get() => _local;
    public void Set(TType? local) => _local = local;

    public void Load(ContentManager manager)
    {
        _local = manager.Load<TType>(Name);
    }

    public Asset() : this(String.Empty)
    {
        
    }

    public Asset(string name) : base(name)
    {
    }

    public static implicit operator TType?(Asset<TType> Asset) => Asset.Get();
}