using Legends.Engine;
using Legends.Engine.Graphics2D.Components;
using Legends.Engine.Graphics2D.Noise;
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Legends.Scripts;

public class GenerateWorldBehavior : Behavior
{
    public GenerateWorldBehavior(): base(null, null)
    {

    }
    public GenerateWorldBehavior(IServiceProvider services, SceneObject parent) : base(services, parent)
    {
        
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public override void Initialize()
    {
        //Parent.GetComponent<Map>().CreateMapFromTexture();
        var map = Parent.GetComponent<Map>();
        var size = map.TileCount;
        //var texture = Parent.GetComponent<Map>().TileSet.TextureRegion.Texture;
        //var color = Parent.GetComponent<Map>().TileSet.TextureRegion.Color;
        var array = new ImageGenerator((new Random()).Next()).GenerateHeightMap((int)size.Width, (int)size.Height);

        for(int col = 0; col < array.GetLength(1); col++)
            for(int row = 0; row < array.GetLength(0); row++)
                map.Tiles[row * array.GetLength(1) + col] = GetTileIndex(array, row, col);


        //texture.SetData<Color>(GetColors(color, map).ToArray());
    }

    public ushort GetTileIndex(float[,] array, int row, int col)
    {
        StringBuilder tag = new StringBuilder("00000000");
        int index = -1;

        for(int x = row - 1; x < row + 1; x++)
        {
            for(int y = col - 1; y < col + 1; y++)
            {
                if(x == row && y == col) continue;
                
                index++;

                if(x < 0 || x >= array.GetLength(0) || y < 0 || y > array.GetLength(1)) continue;
                
                if(array[x, y] < 80)
                {
                    tag[index] = '1';
                }
            }
        }

        return Parent.GetComponent<Map>().TileSet.GetByTag(tag.ToString()).FirstOrDefault();   
    }

    public IEnumerable<Color> GetColors(Color color, float[,] array)
    {
        for(int col = 0; col < array.GetLength(1); col++)
            for(int row = 0; row < array.GetLength(0); row++)
                yield return array[col, row] < 80 ? color : Color.Black;
    }

    public override void Reset()
    {
        
    }
}