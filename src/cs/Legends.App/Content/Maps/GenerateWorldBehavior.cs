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
        var map = Parent.GetComponent<Map>();
        var size = map.TileCount;
        var array = new ImageGenerator((new Random()).Next()).GenerateHeightMap((int)size.Width, (int)size.Height);

        for(int row = 0; row < array.GetLength(0); row++)
            for(int col = 0; col < array.GetLength(1); col++)
                map.Tiles[row * array.GetLength(0) + col] = GetTileIndex(array, row, col);
        
        map.Initialize();
    }

    public ushort GetTileIndex(float[,] array, int row, int col)
    {
        StringBuilder tag = new ("00000000");
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

        var newTag = array[row, col] < 88 ? "00000000" : "11111111";

        return Parent.GetComponent<Map>().TileSet.GetByTag(newTag).FirstOrDefault();   
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