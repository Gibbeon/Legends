using MonoGame.Extended;
using Microsoft.Xna.Framework.Graphics;

namespace Legends.App;

public class TileSet
{
    public Size TileSize;
    public Texture2D Texture;
    public RectangleF GetUV(int tileIndex)
    {        
        var x = tileIndex % (Texture.Width / TileSize.Width) * TileSize.Width;     
        var y = tileIndex / (Texture.Width / TileSize.Height) * TileSize.Height;

        var result =  new RectangleF(
            (float)x / (float)Texture.Width,
            (float)y / (float)Texture.Height,
            (float)(TileSize.Width) / (float)Texture.Width,
            (float)(TileSize.Height) / (float)Texture.Height);

        return result;
    }
}