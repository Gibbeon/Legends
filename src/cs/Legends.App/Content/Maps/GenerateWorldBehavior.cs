using Legends.Engine;
using Legends.Engine.Graphics2D.Components;
using Legends.Engine.Graphics2D.Noise;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Legends.Scripts;

public class GenerateWorldBehavior : Behavior
{
    public GenerateWorldBehavior(): this(null, null)
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
        Parent.GetComponent<Map>().CreateMapFromTexture();

        var size = Parent.GetComponent<Map>().TileCount;
        var texture = Parent.GetComponent<Map>().TileSet.TextureRegion.Texture;
        var color = Parent.GetComponent<Map>().TileSet.TextureRegion.Color;
        var map = new ImageGenerator((new Random()).Next()).GenerateHeightMap((int)size.Width, (int)size.Height);

        texture.SetData<Color>(GetColors(color, map).ToArray());
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