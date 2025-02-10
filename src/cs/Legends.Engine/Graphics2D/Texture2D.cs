using System;
using System.IO;
using Microsoft.Xna.Framework;
using XnaGraphics = Microsoft.Xna.Framework.Graphics;

namespace Legends.Engine.Graphics2D;

public class Texture2D : AssetWrapper<XnaGraphics.Texture2D> 
{    
    public XnaGraphics.SurfaceFormat Format => Instance.Format;
    public int      LevelCount  => Instance.LevelCount;
    public Rectangle Bounds     => Instance.Bounds;
    public int      Width       => Instance.Width;
    public int      Height      => Instance.Height;
    
    public Texture2D(IServiceProvider services): base(services)
    {
        
    }

    public Texture2D(IServiceProvider services, XnaGraphics.Texture2D instance): base(services)
    {
        Instance = instance;        
    }

    public Texture2D(IServiceProvider services, int width, int height): this(services, new XnaGraphics.Texture2D(services.GetGraphicsDevice(), width, height))
    {

    }

    public Texture2D(IServiceProvider services, int width, int height, bool mipmap, XnaGraphics.SurfaceFormat format): this(services, new XnaGraphics.Texture2D(services.GetGraphicsDevice(), width, height, mipmap, format))
    {

    }

    public Texture2D(IServiceProvider services, int width, int height, bool mipmap,XnaGraphics.SurfaceFormat format, int arraySize): this(services, new XnaGraphics.Texture2D(services.GetGraphicsDevice(), width, height, mipmap, format, arraySize))
    {
        
    }

    public void SetData<T>(int level, int arraySlice, Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct 
        => Instance.SetData<T>(level, arraySlice, rect, data, startIndex, elementCount);

    public void SetData<T>(int level, Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct
        => Instance.SetData<T>(level, rect, data, startIndex, elementCount);

    public void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct
        => Instance.SetData<T>(data, startIndex, elementCount);

    public void SetData<T>(T[] data) where T : struct
        => Instance.SetData<T>(data);

    public void GetData<T>(int level, int arraySlice, Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct
        => Instance.GetData<T>(level, arraySlice, rect, data, startIndex, elementCount);

        
    public void GetData<T>(int level, Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct
        => Instance.GetData<T>(level, rect, data, startIndex, elementCount);

    
    public void GetData<T>(T[] data, int startIndex, int elementCount) where T : struct
        => Instance.GetData<T>(data, startIndex, elementCount);

    
    public void GetData<T>(T[] data) where T : struct
        => Instance.GetData<T>(data);

    
    public static Texture2D FromFile(IServiceProvider services, string path, Action<byte[]> colorProcessor)
        => new(services, XnaGraphics.Texture2D.FromFile(services.GetGraphicsDevice(), path, colorProcessor));

    
    public static Texture2D FromFile(IServiceProvider services, string path)
        => new(services, XnaGraphics.Texture2D.FromFile(services.GetGraphicsDevice(), path));

    
    public static Texture2D FromStream(IServiceProvider services, Stream stream, Action<byte[]> colorProcessor)
        => new(services, XnaGraphics.Texture2D.FromStream(services.GetGraphicsDevice(), stream, colorProcessor));

    
    public static Texture2D FromStream(IServiceProvider services, Stream stream)
        => new(services, XnaGraphics.Texture2D.FromStream(services.GetGraphicsDevice(), stream));

    
    public void SaveAsJpeg(Stream stream, int width, int height)
        => Instance.SaveAsJpeg(stream, width, height);

    
    public void SaveAsPng(Stream stream, int width, int height)
        => Instance.SaveAsJpeg(stream, width, height);

    
    public void Reload(Stream textureStream)
        => Instance.Reload(textureStream);

    public XnaGraphics.GraphicsDevice GraphicsDevice
        => Instance.GraphicsDevice;


    public bool IsDisposed 
        => Instance.IsDisposed;

    public object Tag { get => Instance.Tag; set => Instance.Tag = value; }

    public event EventHandler<EventArgs> Disposing { add => Instance.Disposing += value; remove => Instance.Disposing -= value; }

    public void Dispose()
        => Instance?.Dispose();

    public override string ToString()
        => Instance?.ToString();
}