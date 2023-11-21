using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace SlyEngine;

public interface IBehavior : IUpdate, IDraw
{
    GameObject Parent { get; }

    public void Attach(GameObject gameObject);
    void Detatch();
    IBehavior Clone();
}


public SpriteRenderBehavior : IBehavior
{
    Texture2D _texture;

    public void Update(GameTime gameTime)
    {

    }

    public void Draw(GameTime gameTime)
    {
        SpriteBatch _spriteBatch(GraphicsDevice);
        _spriteBatch.Begin();
        _spriteBatch.DrawRectangle(Parent.Bounds, Colors.Red);
        _spriteBatch.End();
    }
}


public class GameObject : SpatialNode
{
    IList<IBehavior> _behaviors;

    public Sprite() : this(null)
    {

    }
    public Sprite(SpatialNode? parent) : base(parent)
    {

    }
}