using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Newtonsoft.Json;

namespace Legends.Engine;

public enum AssetType
{
    Static,
    Dynamic
}

public interface IAsset
{    
    [JsonIgnore] string     AssetName { get; }
    [JsonIgnore] AssetType  AssetType { get; }
    
}

public interface IAsset<TType> : IAsset
{    
    [JsonIgnore] TType  Instance { get; }
}

public abstract class BaseAsset<TType> : IAsset<TType>
{
    protected class BaseAsset_ : BaseAsset<TType> {}
    public string       AssetName { get; protected set; }
    public TType        Instance { get; protected set;  }
    public AssetType    AssetType { get; protected set; }

    protected static BaseAsset<TType> Wrap(TType instance) 
    {
        return new BaseAsset_() { Instance = instance };
    }
    
    public static implicit operator TType(BaseAsset<TType> resource) => resource.Instance;
    public static implicit operator BaseAsset<TType>(TType resource) => Wrap(resource);
}
