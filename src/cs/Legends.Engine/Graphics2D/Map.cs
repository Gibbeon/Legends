using Microsoft.Xna.Framework;
using Legends.Engine;
using Legends.Engine.Animation;
using Legends.Engine.Graphics2D;
using Legends.Engine.Resolvers;
using System;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Legends.App;

public class Map : SceneObject
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
        TileSize = new Size(8, 8);
        TileSet = new TileSet()
        {
            Texture = Services.Content.Load<Texture2D>("npc1"),
            TileSize = new Size(8, 8)
        };
        
        TileCount = new Size(312 / TileSize.Width, 288 / TileSize.Height);

        TileCount *= 2;

        Tiles = new TileData[TileCount.Width, TileCount.Height];

        var x_count = (TileSet.Texture.Width / TileSize.Width);        
        var y_count = (TileSet.Texture.Height / TileSize.Height);
        
        for(var y = 0; y < TileCount.Height; y++)
        {
            for(var x = 0; x < TileCount.Width; x++)
            {
                Tiles[x, y].TileIndex = x % x_count + (y % y_count) * x_count;
            }
        }

        Size = new Size2(TileCount.Width * TileSize.Width, TileCount.Height * TileSize.Height);
        OriginNormalized = new Vector2(.5f, .5f);

        _vertices = BuildVerticies().ToArray();
        _indicies = BuildIndicies().ToArray();
        
        _vertexBuffer = new DynamicVertexBuffer(Services.GraphicsDevice, VertexPositionColorTexture.VertexDeclaration, _vertices.Length, BufferUsage.WriteOnly);
        _vertexBuffer.SetData(_vertices, 0, _vertices.Length);

        _indexBuffer = new DynamicIndexBuffer(Services.GraphicsDevice, IndexElementSize.ThirtyTwoBits, _indicies.Length, BufferUsage.WriteOnly);
        _indexBuffer.SetData(_indicies);

        _currentEffect = new BasicEffect(Services.GraphicsDevice) {
            TextureEnabled = true,
            VertexColorEnabled = true
        };
        (_currentEffect as IEffectMatrices).Projection = Matrix.CreateOrthographicOffCenter(0f, Services.GraphicsDevice.Viewport.Width, Services.GraphicsDevice.Viewport.Height, 0f, 0f, -1f);
        (_currentEffect as IEffectMatrices).View = Matrix.Identity;
        
        if (_currentEffect is BasicEffect textureEffect)
        {
            textureEffect.Texture = TileSet.Texture;
        }
    }

    private VertexPositionColorTexture[] _vertices;
    private uint[] _indicies;
    private DynamicVertexBuffer _vertexBuffer;
    private DynamicIndexBuffer _indexBuffer;

    private Effect _currentEffect;

    public void Draw(GameTime gameTime)
    {
        (_currentEffect as IEffectMatrices).World = this.LocalMatrix;
        
        Services.GraphicsDevice.SetVertexBuffer(_vertexBuffer);
        Services.GraphicsDevice.Indices = _indexBuffer;
        Services.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        Services.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
        Services.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        Services.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        
        foreach (EffectPass pass in _currentEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            Services.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _indicies.Length / 3);
        }
    }

    public IEnumerable<uint> BuildIndicies()
    {
        for(int y = 0; y < TileCount.Height; y++)
        {
            for(int x = 0; x < TileCount.Width; x++)
            {
                yield return (uint)((y * TileCount.Width + x) * 4 + 0);
                yield return (uint)((y * TileCount.Width + x) * 4 + 1);
                yield return (uint)((y * TileCount.Width + x) * 4 + 2);
                yield return (uint)((y * TileCount.Width + x) * 4 + 1);
                yield return (uint)((y * TileCount.Width + x) * 4 + 3);
                yield return (uint)((y * TileCount.Width + x) * 4 + 2);

            }
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