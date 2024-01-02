﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Legends.Engine.Graphics2D;

public class TextureRegion : Spatial
{
    public Ref<Texture2D> Texture { get; set; }

    private int _frame;
    public int Frame { get => _frame; set => _frame = value; }

    public TextureRegion(Texture2D texture, Rectangle region)
        : this(texture, region.X, region.Y, region.Width, region.Height)
    {
    }

    public  TextureRegion()
        : this(null, 0, 0, 0, 0)
    {
    }

    public TextureRegion(Texture2D texture)
        : this(texture, 0, 0, texture.Width, texture.Height)
    {
    }

    public TextureRegion(Texture2D texture, int x, int y, int width, int height) : base()
    {
        Texture = texture;
        Position = new Vector2(x, y);
        Size = new Size2(width, height);
    }

    public void SetFrame(int index)
    {
        if((~Texture) == null) return;
        
        Position = new Vector2(
            (int)(index * Size.Width) % (int)(~Texture).Width, (int)((index * Size.Width) / (int)(~Texture).Width) * Size.Height
        );
    }

    public int GetFrame()
    {
        if(Size.IsEmpty) return 0;
        int x_offset = (int)(Position.X / Size.Width);
        int y_offset = (int)(Position.Y / Size.Width);
        if((~Texture) == null) return x_offset;
        return x_offset + y_offset * ((~Texture).Width / (int)Size.Width);
    }

    public void Update()
    {
        SetFrame(_frame);
    }

    public override string ToString()
    {
        return $"{string.Empty} {BoundingRectangle}";
    }
}