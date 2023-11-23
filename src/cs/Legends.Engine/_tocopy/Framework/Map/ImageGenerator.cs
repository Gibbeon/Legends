using System;

namespace LitEngine.Framework.Map
{
    public enum PixelBlendOperation {
        Overwrite,
        Add,
        Subtract,
        Divide,
        Multiply
    }

    public class ImageGenerator
    {
        public ImageGenerator(int? seed = null)
        {
            if (seed != null)
            {
                SimplexNoise.Seed = seed.Value;
            }
        }
        public float[,] GenerateHeightMap(int width, int height, float min = 0f, float max = 1.0f)
        {
            return SimplexNoise.Calc2D(width, height, .03f);
        }

        public float[] GenerateGradient(int length, float min = 0f, float max = 1.0f, PixelBlendOperation blendOperation = PixelBlendOperation.Overwrite)
        {
            var blendFunc = GetBlendOperation1D(blendOperation);
            float[] result = new float[length];

            for (var i = 0; i < length; i++)
            {
                var index = min < max ? i : (length - i - 1);
                result[index] = min + (max / ((float)length - 1)) * i;
            }

            return result;
        }

        public float[,] GenerateHorizontalGradient(int width, int height, float min = 0f, float max = 1.0f, 
            float[,]? array = null, int offsetX = 0, int offsetY = 0, PixelBlendOperation blendOperation = PixelBlendOperation.Overwrite)
        {
            var blendFunc = GetBlendOperation2D(blendOperation);
            float[,] result = array ?? new float[width, height];
           
            for (var x = offsetX; x < width; x++)
            {
                var index = min < max ? x : (width - x - 1);
                var value = min + (max / ((float)width - 1)) * x;

                for (var y = offsetY; y < height; y++)
                {
                    blendFunc(result, x, index, value);
                }
            }

            return result;
        }
        
        public float[,] GenerateVertialGradient(int width, int height, float min = 0f, float max = 1.0f, 
            float[,]? array = null, int offsetX = 0, int offsetY = 0, PixelBlendOperation blendOperation = PixelBlendOperation.Overwrite)
        {
            var blendFunc = GetBlendOperation2D(blendOperation);
            float[,] result = array ?? new float[width, height];
           
            for (var y = offsetY; y < height; y++)
            {
                var index = min < max ? y : width - y;
                var value = min + (max / ((float)width - 1)) * y;
                
                for (var x = offsetX; x < width; x++)
                {                 
                    blendFunc(result, x, index, value);   
                }
            }

            return result;
        }

        protected Action<float[,], int, int, float> GetBlendOperation2D(PixelBlendOperation blendOperation) {
            switch(blendOperation) {

                case PixelBlendOperation.Add:
                return (float[,] array, int x, int y, float value) => {
                    array[x, y] += value;
                };

                case PixelBlendOperation.Subtract:
                return (float[,] array, int x, int y, float value) => {
                    array[x, y] -= value;
                };

                case PixelBlendOperation.Multiply:
                return (float[,] array, int x, int y, float value) => {
                    array[x, y] *= value;
                };
                
                case PixelBlendOperation.Overwrite:
                default:
                return (float[,] array, int x, int y, float value) => {
                    array[x, y] = value;
                };
            }
        }

        protected Action<float[], int, float> GetBlendOperation1D(PixelBlendOperation blendOperation) {
            switch(blendOperation) {

                case PixelBlendOperation.Add:
                return (float[] array, int x, float value) => {
                    array[x] += value;
                };

                case PixelBlendOperation.Subtract:
                return (float[] array, int x, float value) => {
                    array[x] -= value;
                };

                case PixelBlendOperation.Multiply:
                return (float[] array, int x, float value) => {
                    array[x] *= value;
                };
                
                case PixelBlendOperation.Overwrite:
                default:
                return (float[] array, int x, float value) => {
                    array[x] = value;
                };
            }
        }
    }
}
/*
            var xbuf = TileCount.X / 3f;
            var ybuf = TileCount.Y / 3f;

            var tilemap = new int[7];

            var floatValue = 120;

            tilemap[0] = (int)(floatValue / 7 * 0);
            tilemap[1] = (int)(floatValue / 7 * .75);
            tilemap[2] = (int)(floatValue / 7 * 1.25);
            tilemap[3] = (int)(floatValue / 7 * 1.7);
            tilemap[4] = (int)(floatValue / 7 * 2.0);
            tilemap[5] = (int)(floatValue / 7 * 9);
            tilemap[6] = (int)(floatValue / 7 * 12);


            for (int y = 0; y < TileCount.Y; y++)
            {
                for (int x = 0; x < TileCount.X; x++)
                {
                    var factor = Math.Min(1, x / xbuf) * Math.Min(1, (TileCount.X - x - 1) / xbuf) * Math.Min(1, y / ybuf) * Math.Min(1, (TileCount.Y - y - 1) / ybuf);
                    var value = 0;

                    for (int i = 0; i < 7; i++)
                    {
                        if (tilemap[i] < (heightmap[x, y] * factor))
                        {
                            value = i;
                        }
                    }

                    value = Math.Max(0, Math.Min(7, value));

                    for (int i = 0; i < (int)8; i++)
                    {
                        Tiles[TileCount.X * y + x].TextureCoordinates = new Rectangle(0, (int)(value) * TILE_SIZE, TILE_SIZE, TILE_SIZE);
                    }
                }
           }
    }
}

// private void GenCloudMap()
// {
//     var heightmap = SimplexNoise.Noise.Calc2D(_clouds.Width, _clouds.Height, .02f);


//     for (int y = 0; y < _worldMap.Height; y++)
//     {
//         for (int x = 0; x < _worldMap.Width; x++)
//         {
//             var factor = 1;
//             var value = 0;

//             for (int i = 0; i < 7; i++)
//             {
//                 if (200 < (heightmap[x, y] * factor))
//                 {
//                     value = i;
//                 }
//             }

//             value = Math.Max(0, Math.Min(7, value));
//             if (value > 0)
//                 _clouds.Tiles[_clouds.Width * y + x].TextureCoordinates = new Rectangle(0, (int)(value) * TILE_SIZE, TILE_SIZE, TILE_SIZE);
//         }
//     }

// }
*/
