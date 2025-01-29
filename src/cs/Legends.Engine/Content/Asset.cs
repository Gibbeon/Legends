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

    public Asset(): this (AssetType.Dynamic, "") {}
    protected Asset(AssetType assetType, string assetName)
    {
        AssetName = assetName;
        AssetType = assetType;
    }
}

public class AssetWrapper<TType> : Asset
{
    protected TType  Instance { get; set; }
    public AssetWrapper(): this (AssetType.Dynamic, "") {}
    protected AssetWrapper(AssetType assetType, string assetName): base(assetType, assetName) {}

    protected static AssetWrapper<TType> Wrap(TType instance) 
    {
        return new AssetWrapper<TType>() { Instance = instance };
    }

    public static implicit operator TType(AssetWrapper<TType> resource) => resource.Instance;
    public static implicit operator AssetWrapper<TType>(TType resource) => Wrap(resource);
}
