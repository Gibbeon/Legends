using Microsoft.Xna.Framework;
using Legends.Engine;
using Legends.Engine.Animation;
using Legends.Engine.Graphics2D;
using Legends.Engine.Resolvers;
using System;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using MonoGame.Extended.Graphics;
using System.Linq;
using MonoGame.Extended.Graphics.Effects;
using MonoGame.Extended.Tiled;

namespace Legends.App;

public class TileSet
{
    public Size TileSize;
    public Texture2D Texture;
    public RectangleF GetUV(int tileIndex)
    {        
        var x = tileIndex / (Texture.Height / TileSize.Height);
        var y = tileIndex % (Texture.Width / TileSize.Width);

        return new RectangleF(
            (float)x / (float)Texture.Width,
            (float)y / (float)Texture.Height,
            (float)(x + TileSize.Width) / (float)Texture.Width,
            (float)(y + TileSize.Height) / (float)Texture.Height);
    }
}

public class Map : GameObject
{
    public struct TileData
    {
        public int TileIndex;
    }
    public Size TileSize;
    public Size TileCount;
    public TileSet TileSet;
    public TileData[,] Tiles;
    public Map(SystemServices services) : base(services)
    {
        TileSize = new Size(32, 32);
        TileCount = new Size(256, 256);
        TileSet = new TileSet()
        {
            Texture = Services.Content.Load<Texture2D>("npc1"),
            TileSize = new Size(32, 32)
        };

        Tiles = new TileData[TileCount.Width, TileCount.Height];
        _vertices = BuildVerticies().ToArray();
        _indicies = BuildIndicies().ToArray();

        _vertexBuffer = new DynamicVertexBuffer(Services.GraphicsDevice, VertexPositionColorTexture.VertexDeclaration, _vertices.Length, BufferUsage.WriteOnly);
        _vertexBuffer.SetData(_vertices, 0, _vertices.Length);

        _indexBuffer = new DynamicIndexBuffer(Services.GraphicsDevice, IndexElementSize.SixteenBits, _indicies.Length, BufferUsage.WriteOnly);
        _indexBuffer.SetData(_indicies);

        _currentEffect = new BasicEffect(Services.GraphicsDevice) {
            TextureEnabled = true,
            VertexColorEnabled = true
        };
        (_currentEffect as IEffectMatrices).Projection = Matrix.CreateOrthographicOffCenter(0f, Services.GraphicsDevice.Viewport.Width, Services.GraphicsDevice.Viewport.Height, 0f, 0f, -1f);
        (_currentEffect as IEffectMatrices).View = Matrix.Identity;
        (_currentEffect as IEffectMatrices).World = Matrix.Identity;
        if (_currentEffect is BasicEffect textureEffect)
        {
            textureEffect.Texture = TileSet.Texture;
        }
    }

    private VertexPositionColorTexture[] _vertices;
    private short[] _indicies;
    private DynamicVertexBuffer _vertexBuffer;
    private DynamicIndexBuffer _indexBuffer;

    private Effect _currentEffect;

    public void Draw(GameTime gameTime)
    {
        Services.GraphicsDevice.SetVertexBuffer(_vertexBuffer);
        Services.GraphicsDevice.Indices = _indexBuffer;
        Services.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        Services.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
        Services.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        Services.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
        
        foreach (EffectPass pass in _currentEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            Services.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _vertices.Length / 2);
        }
    }

    public IEnumerable<short> BuildIndicies()
    {
        for(int y = 0; y < TileCount.Height; y++)
            for(int x = 0; x < TileCount.Width; x++)
            {
                yield return (short)((y * TileCount.Height + x) * 6 + 0);
                yield return (short)((y * TileCount.Height + x) * 6 + 1);
                yield return (short)((y * TileCount.Height + x) * 6 + 2);
                yield return (short)((y * TileCount.Height + x) * 6 + 1);
                yield return (short)((y * TileCount.Height + x) * 6 + 3);
                yield return (short)((y * TileCount.Height + x) * 6 + 2);
            }
    }

    public IEnumerable<VertexPositionColorTexture> BuildVerticies()
    {
        for(int y = 0; y < TileCount.Height; y++)
        {
            for(int x = 0; x < TileCount.Width; x++)
            {
                var uvCoords = TileSet.GetUV(Tiles[x, y].TileIndex);
                yield return new VertexPositionColorTexture(
                    new Vector3(x * TileSize.Width, y * TileSize.Height, 0),
                    Color.White,
                    uvCoords.TopLeft
                );

                yield return new VertexPositionColorTexture(
                    new Vector3(x * TileSize.Width + TileSize.Width, y * TileSize.Height, 0),
                    Color.White,
                    uvCoords.TopRight
                );

                yield return new VertexPositionColorTexture(
                    new Vector3(x * TileSize.Width, y * TileSize.Height + TileSize.Height, 0),
                    Color.White,
                    uvCoords.BottomLeft
                );

                yield return new VertexPositionColorTexture(
                    new Vector3(x * TileSize.Width + TileSize.Width, y * TileSize.Height + TileSize.Height, 0),
                    Color.White,
                    uvCoords.BottomRight
                );
            }
        }
    }
}

public class Actor : GameObject
{
    public Vector2 Facing;
    public float Speed;
    ValueResolver<string, Actor> _resolver;

    public Actor(SystemServices services) : base(services)
    {
        Speed = 1.0f;
        Facing = DirectionConstants.Down;

        Size = new MonoGame.Extended.Size2(24, 36);
        _resolver = new ValueResolver<string, Actor>();
        
        _resolver.Add("walk", (actor) => { return actor.Facing == DirectionConstants.Left; }, "walk_left");
        _resolver.Add("walk", (actor) => { return actor.Facing == DirectionConstants.Right; }, "walk_right");
        _resolver.Add("walk", (actor) => { return actor.Facing == DirectionConstants.Up; }, "walk_up");
        _resolver.Add("walk", (actor) => { return actor.Facing == DirectionConstants.Down; }, "walk_down");

        _resolver.Add("idle", (actor) => { return actor.Facing == DirectionConstants.Left; }, "idle_left");
        _resolver.Add("idle", (actor) => { return actor.Facing == DirectionConstants.Right; }, "idle_right");
        _resolver.Add("idle", (actor) => { return actor.Facing == DirectionConstants.Up; }, "idle_up");
        _resolver.Add("idle", (actor) => { return actor.Facing == DirectionConstants.Down; }, "idle_down");

        AttachBehavior(new SpriteRenderBehavior(this));
        AttachBehavior(new AnimationBehavior(this));
        AttachBehavior(new RandomMovementBehavior(this));

        foreach(var data in Data.AnimationData)
        {
            GetBehavior<AnimationBehavior>().Animations.Add( new SpriteKeyframeAnimation(GetBehavior<SpriteRenderBehavior>(), data));
        }
    }

    bool IsMoving = false;

    public override void Move(Vector2 direction)
    {
        Facing = DirectionConstants.GetNearestFacing(direction);
        GetBehavior<AnimationBehavior>().Play(_resolver.Resolve("walk", this));
        IsMoving = true;
        base.Move(Facing * Speed);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if(!IsMoving)
        {
            GetBehavior<AnimationBehavior>().Play(_resolver.Resolve("idle", this));
        } 
        IsMoving = false;
    }
}
