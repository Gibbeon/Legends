using System;

namespace Legends.Engine.Graphics2D.Noise;

public enum PixelBlendOperation 
{
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
    public float[,] GenerateHeightMap(int width, int height)
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
        float[,] array = null, int offsetX = 0, int offsetY = 0, PixelBlendOperation blendOperation = PixelBlendOperation.Overwrite)
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
        float[,] array = null, int offsetX = 0, int offsetY = 0, PixelBlendOperation blendOperation = PixelBlendOperation.Overwrite)
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
        return blendOperation switch
        {
            PixelBlendOperation.Add => (float[,] array, int x, int y, float value) =>
                {
                    array[x, y] += value;
                },
            PixelBlendOperation.Subtract => (float[,] array, int x, int y, float value) =>
                {
                    array[x, y] -= value;
                },
            PixelBlendOperation.Multiply => (float[,] array, int x, int y, float value) =>
                {
                    array[x, y] *= value;
                },
            PixelBlendOperation.Divide => (float[,] array, int x, int y, float value) =>
                {
                    array[x, y] /= value;
                },
            _ => (float[,] array, int x, int y, float value) =>
                {
                    array[x, y] = value;
                }
        };
    }

    protected Action<float[], int, float> GetBlendOperation1D(PixelBlendOperation blendOperation) {
        return blendOperation switch
        {
            PixelBlendOperation.Add => (float[] array, int x, float value) =>
                {
                    array[x] += value;
                },
            PixelBlendOperation.Subtract => (float[] array, int x, float value) =>
                {
                    array[x] -= value;
                },
            PixelBlendOperation.Multiply => (float[] array, int x, float value) =>
                {
                    array[x] *= value;
                },
            PixelBlendOperation.Divide => (float[] array, int x, float value) =>
                {
                    array[x] /= value;
                },
            _ => (float[] array, int x, float value) =>
                {
                    array[x] = value;
                }
        };
    }
}