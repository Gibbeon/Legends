using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Content;

namespace Legends.Engine.Content;

public class AssetLoader<TType> : IContentLoader<Asset<TType>>
{
    public Asset<TType> Load(ContentManager contentManager, string path)
    {
        return contentManager.Load<Asset<TType>>(path);
    }
}