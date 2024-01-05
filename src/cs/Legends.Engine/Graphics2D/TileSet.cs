using MonoGame.Extended;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Legends.Engine.Graphics2D;

public class TileSet
{
    public Size2 TileSize;
    public Ref<Texture2D> Texture;
    public RectangleF GetUV(int tileIndex)
    {        
        var x = tileIndex % (int)((~Texture).Width / TileSize.Width);     
        var y = tileIndex / (int)((~Texture).Width / TileSize.Width);

        var result =  new RectangleF(
            (float)x * TileSize.Width / (float)(~Texture).Width,
            (float)y * TileSize.Height / (float)(~Texture).Height,
            (float)(TileSize.Width) / (float)(~Texture).Width,
            (float)(TileSize.Height) / (float)(~Texture).Height);

        return result;
    }
}