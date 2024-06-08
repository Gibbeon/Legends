using Microsoft.Xna.Framework;
using Legends.Engine;
using System;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Legends.Engine.Graphics2D.Components;

public class Map : Component, IRenderable
{
    [JsonIgnore]
    public int RenderLayerID => 0;

    public Size2        TileCount { get; set; }

    [JsonIgnore]
    public TileSet      TileSet => TileSetReference.Get();

    [JsonProperty(nameof(TileSet))]
    protected Ref<TileSet> TileSetReference { get; set; }
    public ushort[]     Tiles { get; set; }
    
    [JsonIgnore]
    protected bool IsDirty { get; set; }

    [JsonIgnore]
    public bool Visible => Parent.Visible;

    public RenderState RenderState { get; set; }

    [JsonIgnore]
    public IViewState ViewState => Parent.Scene.Camera;

    private VertexPositionColorTexture[] _vertices;
    private uint[] _indicies;
    private DynamicVertexBuffer _vertexBuffer;
    private DynamicIndexBuffer _indexBuffer;
    private Effect _currentEffect;
    public Map(): this(null, null) {}
    public Map(IServiceProvider services, SceneObject parent) : base(services, parent)
    {
        IsDirty = true;
    }

    public void CreateMapFromTexture()
    {
        var x_count = TileSet.TextureRegion.Size.Width /    TileSet.TileSize.Width;        
        var y_count = TileSet.TextureRegion.Size.Height /   TileSet.TileSize.Height;

        TileCount = new Size2(x_count, y_count);

        Tiles = new ushort[(int)(TileCount.Height * TileCount.Width)];
        
        for(var y = 0; y < TileCount.Height; y++)
        {
            for(var x = 0; x < TileCount.Width; x++)
            {
                Tiles[(int)(y * TileCount.Width) + x] = (ushort)(x % x_count + y % y_count * x_count);
            }
        }

        Initialize();
    }

    public void BuildMap()
    {
        _vertices = BuildVerticies().ToArray();
        _indicies = BuildIndicies().ToArray();
        
        _vertexBuffer = new DynamicVertexBuffer(Services.GetGraphicsDevice(), VertexPositionColorTexture.VertexDeclaration, _vertices.Length, BufferUsage.WriteOnly);
        if(_vertices.Length > 0) _vertexBuffer.SetData(_vertices, 0, _vertices.Length);

        _indexBuffer = new DynamicIndexBuffer(Services.GetGraphicsDevice(), IndexElementSize.ThirtyTwoBits, _indicies.Length, BufferUsage.WriteOnly);
        if(_indicies.Length > 0) _indexBuffer.SetData(_indicies);

    }

    public int SelectTileAt(Vector2 position)
    {
        position = Vector2.Transform(position, Matrix.Invert(Parent.LocalMatrix));

        int x_ofs = (int)position.X / (int)TileSet.TileSize.Width;
        int y_ofs = (int)position.Y / (int)TileSet.TileSize.Height;

        return y_ofs * (int)TileCount.Width + x_ofs;
    }

    public void ChangeTile(int tile, int tileSetId)
    {
        Console.WriteLine ("{0}", tileSetId);
        Tiles[tile] = (ushort)tileSetId;
        BuildMap();
    }

    public override void Initialize()
    {   
        TileSet.Initialize();

        Parent.Size = new Size2(TileCount.Width * TileSet.TileSize.Width, TileCount.Height * TileSet.TileSize.Height);
        Parent.OriginNormalized = new Vector2(.5f, .5f);

        ushort[] tiles = new ushort[(int)TileCount.Width * (int)TileCount.Height]; 

        if(Tiles != null) {
            for(int y = 0; y < TileCount.Height; y++) {
                for(int x = 0; x < TileCount.Width; x++) { //no multi dimentianal array
                    tiles[(int)(y * TileCount.Width) + x] = Tiles[((int)(y * TileCount.Width) + x) % Tiles.Length];
                }
            }
        }

        Tiles = tiles;
        
        BuildMap();
 
        _currentEffect = new BasicEffect(Services.GetGraphicsDevice()) {
            TextureEnabled = true,
            VertexColorEnabled = true
        };

        //(_currentEffect as IEffectMatrices).Projection = Matrix.CreateOrthographicOffCenter(0f, Services.GetGraphicsDevice().Viewport.Width, Services.GetGraphicsDevice().Viewport.Height, 0f, 0f, -1f);
        //(_currentEffect as IEffectMatrices).View = Matrix.Identity;
        
        if (_currentEffect is BasicEffect textureEffect)
        {
            textureEffect.Texture = TileSet.TextureRegion.Texture;
        }

        IsDirty = false;
    }

    public override void Update(GameTime gameTime)
    {
        TileSet.Update(gameTime);     
        BuildMap();   
    }

    public override void Draw(GameTime gameTime)
    {
        if(IsDirty) Initialize();

        base.Draw(gameTime);

        Services.Get<IRenderService>().DrawBatched(this);
    }

    public void DrawImmediate(GameTime gameTime, GraphicsResource target = null)
    {
        (_currentEffect as IEffectMatrices).View        = ViewState.View;
        (_currentEffect as IEffectMatrices).Projection  = ViewState.Projection;
        (_currentEffect as IEffectMatrices).World       = ViewState.World * Parent.LocalMatrix;//* Matrix2.CreateTranslation(-Parent.Origin * Parent.Scale);

        var rasterizerState = new RasterizerState() {
            CullMode = CullMode.CullCounterClockwiseFace,
            FillMode = FillMode.Solid
        };
        
        Services.GetGraphicsDevice().SetVertexBuffer(_vertexBuffer);
        Services.GetGraphicsDevice().Indices = _indexBuffer;
        Services.GetGraphicsDevice().BlendState = BlendState.AlphaBlend;
        Services.GetGraphicsDevice().SamplerStates[0] = SamplerState.PointClamp;
        Services.GetGraphicsDevice().DepthStencilState = DepthStencilState.Default;
        Services.GetGraphicsDevice().RasterizerState = rasterizerState;
        
        if(_indicies.Length > 0)
        {
            foreach (EffectPass pass in _currentEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Services.GetGraphicsDevice().DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _indicies.Length / 3);
            }
        }
    }

    protected IEnumerable<uint> BuildIndicies()
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

    protected IEnumerable<VertexPositionColorTexture> BuildVerticies()
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
    }

    public override void Reset()
    {

    }
}