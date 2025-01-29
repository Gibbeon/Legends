using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

public abstract class Asset : IAsset
{
    public string    AssetName { get; protected set; }
    public AssetType AssetType { get; protected set; }
}

public class AssetWrapper<TType> : Asset
{
    protected TType  Instance { get; set; }
    protected static AssetWrapper<TType> Wrap(TType instance) 
    {
        return new AssetWrapper<TType>() { Instance = instance, AssetType = AssetType.Dynamic };
    }

    public static implicit operator TType(AssetWrapper<TType> resource) => resource.Instance;
    public static implicit operator AssetWrapper<TType>(TType resource) => Wrap(resource);
}
