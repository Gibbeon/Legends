using Microsoft.Xna.Framework;
using Legends.Engine;
using System;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Legends.Engine.Graphics2D;

public class Map : Component<Map>, ISelfDrawable
{
    public Size2        TileCount { get; set; }
    public TileSet      TileSet { get; set; }
    public ushort[]     Tiles { get; set; }
    public bool NeedUpdate { get; set; }

    public bool Visible => Parent.Visible;

    private VertexPositionColorTexture[] _vertices;
    private uint[] _indicies;
    private DynamicVertexBuffer _vertexBuffer;
    private DynamicIndexBuffer _indexBuffer;
    private Effect _currentEffect;
    public Map(): this(null, null) {}
    public Map(IServiceProvider services, SceneObject parent) : base(services, parent)
    {
        NeedUpdate = true;
    }

    public void CreateMapFromTexture()
    {
        var x_count = ((~TileSet.Texture).Width / TileSet.TileSize.Width);        
        var y_count = ((~TileSet.Texture).Height / TileSet.TileSize.Height);

        Tiles = new ushort[(int)(TileCount.Height * TileCount.Width + 1)];
        
        for(var y = 0; y < TileCount.Height; y++)
        {
            for(var x = 0; x < TileCount.Width; x++)
            {
                Tiles[(int)(y * TileCount.Width) + x] = (ushort)(x % x_count + (y % y_count) * x_count);
            }
        }

        Initialize();
    }

    public void Initialize()
    {   
        Parent.Size = new Size2(TileCount.Width * TileSet.TileSize.Width, TileCount.Height * TileSet.TileSize.Height);
        Parent.OriginNormalized = new Vector2(.5f, .5f);

        _vertices = BuildVerticies().ToArray();
        _indicies = BuildIndicies().ToArray();
        
        _vertexBuffer = new DynamicVertexBuffer(Services.GetGraphicsDevice(), VertexPositionColorTexture.VertexDeclaration, _vertices.Length, BufferUsage.WriteOnly);
        _vertexBuffer.SetData(_vertices, 0, _vertices.Length);

        _indexBuffer = new DynamicIndexBuffer(Services.GetGraphicsDevice(), IndexElementSize.ThirtyTwoBits, _indicies.Length, BufferUsage.WriteOnly);
        _indexBuffer.SetData(_indicies);

        _currentEffect = new BasicEffect(Services.GetGraphicsDevice()) {
            TextureEnabled = true,
            VertexColorEnabled = true
        };

        (_currentEffect as IEffectMatrices).Projection = Matrix.CreateOrthographicOffCenter(0f, Services.GetGraphicsDevice().Viewport.Width, Services.GetGraphicsDevice().Viewport.Height, 0f, 0f, -1f);
        (_currentEffect as IEffectMatrices).View = Matrix.Identity;
        
        if (_currentEffect is BasicEffect textureEffect)
        {
            textureEffect.Texture = TileSet.Texture;
        }

        NeedUpdate = false;
    }

    public override void Update(GameTime gameTime)
    {
        
    }

    public override void Draw(GameTime gameTime)
    {
        if(NeedUpdate) Initialize();

        base.Draw(gameTime);

        Services.Get<IRenderService>().DrawBatched(this);
    }

    public void DrawImmediate(GameTime gameTime)
    {
        (_currentEffect as IEffectMatrices).View        = Parent.GetParentScene().Camera.View;
        (_currentEffect as IEffectMatrices).Projection  = Parent.GetParentScene().Camera.Projection;
        (_currentEffect as IEffectMatrices).World = 
            Matrix.Multiply(
                Matrix.CreateTranslation(-Parent.Origin.X, -Parent.Origin.Y, 0) * Parent.LocalMatrix, 
                Parent.GetParentScene().Camera.World);
        
        Services.GetGraphicsDevice().SetVertexBuffer(_vertexBuffer);
        Services.GetGraphicsDevice().Indices = _indexBuffer;
        Services.GetGraphicsDevice().BlendState = BlendState.AlphaBlend;
        Services.GetGraphicsDevice().SamplerStates[0] = SamplerState.PointClamp;
        Services.GetGraphicsDevice().DepthStencilState = DepthStencilState.Default;
        Services.GetGraphicsDevice().RasterizerState = RasterizerState.CullCounterClockwise;
        
        foreach (EffectPass pass in _currentEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            Services.GetGraphicsDevice().DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _indicies.Length / 3);
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
                var uvCoords = TileSet.GetUV(Tiles[(int)(y * TileCount.Width) + x]);
                yield return new VertexPositionColorTexture(
                    new Vector3(x * TileSet.TileSize.Width, y * TileSet.TileSize.Height, 0),
                    Color.White,
                    uvCoords.TopLeft
                );

                yield return new VertexPositionColorTexture(
                    new Vector3(x * TileSet.TileSize.Width + TileSet.TileSize.Width, y * TileSet.TileSize.Height, 0),
                    Color.White,
                    uvCoords.TopRight
                );

                yield return new VertexPositionColorTexture(
                    new Vector3(x * TileSet.TileSize.Width, y * TileSet.TileSize.Height + TileSet.TileSize.Height, 0),
                    Color.White,
                    uvCoords.BottomLeft
                );

                yield return new VertexPositionColorTexture(
                    new Vector3(x * TileSet.TileSize.Width + TileSet.TileSize.Width, y * TileSet.TileSize.Height + TileSet.TileSize.Height, 0),
                    Color.White,
                    uvCoords.BottomRight
                );
            }
        }
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
        base.Dispose();
    }
}