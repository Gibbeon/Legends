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

    public Size        TileCount { get; set; }

    public Color Color { get; set; } = Color.White;

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

        TileCount = new Size((int)x_count, (int)y_count);

        Tiles = new ushort[TileCount.Height * TileCount.Width];
        
        for(var y = 0; y < TileCount.Height; y++)
        {
            for(var x = 0; x < TileCount.Width; x++)
            {
                Tiles[(int)(y * TileCount.Width) + x] = (ushort)(x % x_count + y % y_count * x_count);
            }
        }

        Initialize();
    }

    public void BuildMap(Camera camera)
    {        
        var size_x  = (int)(camera.AbsoluteBoundingRectangle.Width    / TileSet.TileSize.Width) ;
        var size_y  = (int)(camera.AbsoluteBoundingRectangle.Height   / TileSet.TileSize.Height);

        int max_vertex_count    = (int)(size_x * (size_y + 4) + 1) * 4;
        int max_index_count     = (int)(size_x * (size_y + 4) + 1) * 6;

        if(_vertices == null || _vertices.Length < max_vertex_count)
        {
            _vertices = new VertexPositionColorTexture[max_vertex_count];
            _vertexBuffer = new DynamicVertexBuffer(Services.GetGraphicsDevice(), VertexPositionColorTexture.VertexDeclaration, _vertices.Length, BufferUsage.WriteOnly);
        }

        if(_indicies == null || _indicies.Length < max_index_count)
        {            
            _indicies = new uint[max_index_count];
            _indexBuffer = new DynamicIndexBuffer(Services.GetGraphicsDevice(), IndexElementSize.ThirtyTwoBits, _indicies.Length, BufferUsage.WriteOnly);
        }

        if(_indicies.Length > 16)
        {
            BuildIndicies(_indicies, size_x, size_y);
            BuildVerticies(_vertices, camera.TopLeft, size_x, size_y);
        }

        if(_vertices.Length > 0 && _vertexBuffer != null) _vertexBuffer.SetData(_vertices, 0, _vertices.Length);
        if(_indicies.Length > 0 && _indexBuffer != null) _indexBuffer.SetData(_indicies);

        /*

        // bottom right
        index_offset  = BuildIndicies (_indicies, index_offset, vertex_offset, (int)(size_x + ofs_x) + 1, (int)(size_y + ofs_y) + 1);
        vertex_offset = BuildVerticies(_vertices, vertex_offset, (int)0, (int)0, (int)0, (int)0, (int)(size_x + ofs_x) + 1, (int)(size_y + ofs_y) + 1);



        // top left
        index_offset  = BuildIndicies (_indicies, index_offset, vertex_offset, (int)(-ofs_x)    , (int)(-ofs_y));
        vertex_offset = BuildVerticies(_vertices, vertex_offset, (int)camera.AbsoluteBoundingRectangle.X, (int)camera.AbsoluteBoundingRectangle.Y,
                                  (int)0, (int)0, (int)(-ofs_x)    , (int)(-ofs_y));

        // bottom left
        index_offset  = BuildIndicies (_indicies, index_offset, vertex_offset, (int)(-ofs_x)    , (int)(-ofs_y));
        vertex_offset = BuildVerticies(_vertices, vertex_offset, (int)camera.AbsoluteBoundingRectangle.X, 0,
                                  (int)0, (int)0, (int)(size_x + ofs_x) + 1 , (int)(-ofs_y));

                // top left
        index_offset  = BuildIndicies (_indicies, index_offset, vertex_offset, (int)(-ofs_x)    , (int)(-ofs_y));
        vertex_offset = BuildVerticies(_vertices, vertex_offset, 0, (int)camera.AbsoluteBoundingRectangle.Y,
                                  (int)0, (int)0, (int)(-ofs_x)    , (int)(size_y + ofs_y) + 1);

*/



       

        IsDirty = false;
    }

    public int GetTileIndexAt(Vector2 position)
    {
        position = Vector2.Transform(position, Matrix.Invert(Parent.LocalMatrix));

        int x_ofs = (int)position.X / (int)TileSet.TileSize.Width;
        int y_ofs = (int)position.Y / (int)TileSet.TileSize.Height;

        return y_ofs * (int)TileCount.Width + x_ofs;
    }

    public void SetTile(int tile, int tileSetId)
    {
        Console.WriteLine ("{0}", tileSetId);
        Tiles[tile] = (ushort)tileSetId;
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

        BuildMap(Parent.Scene.Camera);
    }

    public override void Update(GameTime gameTime)
    {        
        base.Update(gameTime);
        TileSet.Update(gameTime); 
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        BuildMap(Parent.Scene.Camera); 
        Services.Get<IRenderService>().DrawBatched(this);
    }

    public void DrawImmediate(GameTime gameTime, GraphicsResource target = null)
    {
        var graphicsDevice = target != null ? target.GraphicsDevice : Services.GetGraphicsDevice();

        (_currentEffect as IEffectMatrices).View        = ViewState.View;
        (_currentEffect as IEffectMatrices).Projection  = ViewState.Projection;
        (_currentEffect as IEffectMatrices).World       = ViewState.World;// * Parent.LocalMatrix;//* Matrix2.CreateTranslation(-Parent.Origin * Parent.Scale);


        graphicsDevice.SetVertexBuffer(_vertexBuffer);
        graphicsDevice.Indices = _indexBuffer;
        graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

        var renderState = RenderState ?? new RenderState();


        graphicsDevice.BlendState             = renderState.BlendState ?? BlendState.AlphaBlend;
        graphicsDevice.DepthStencilState      = renderState.DepthStencilState ?? DepthStencilState.Default;
        graphicsDevice.RasterizerState        = renderState.RasterizerState ?? new RasterizerState() {
                                                    CullMode = CullMode.CullCounterClockwiseFace,
                                                    FillMode = FillMode.Solid
                                                };
   
            
        if(_indicies.Length > 0)
        {
            foreach (EffectPass pass in _currentEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Services.GetGraphicsDevice().DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _indicies.Length / 3);
            }
        }
    }

    protected void BuildIndicies(uint[] indicies, int width, int height)
    {
        //height =    height >= 0 ? height : TileCount.Height;
        //width =     width >= 0 ? width : TileCount.Width;

        int index_offset = 0;

        for(int y = 0; y < height + 2; y++)
        {
            for(int x = 0; x < width + 2; x++)
            {
                int offset = y * (width + 2) + x;

                indicies[index_offset++] = (uint)(offset * 4 + 0);
                indicies[index_offset++] = (uint)(offset * 4 + 1);
                indicies[index_offset++] = (uint)(offset * 4 + 2);
                indicies[index_offset++] = (uint)(offset * 4 + 1);
                indicies[index_offset++] = (uint)(offset * 4 + 3);
                indicies[index_offset++] = (uint)(offset * 4 + 2);
            }
        }
    }

    protected void BuildVerticies(VertexPositionColorTexture[] vertices, Vector2 position, int width, int height)
    {       
        int vertex_offset = 0;

        int x_abs = (int)(((int)position.X / (int)TileSet.TileSize.Width) * TileSet.TileSize.Width);
        int y_abs = (int)(((int)position.Y / (int)TileSet.TileSize.Height) * TileSet.TileSize.Height);

        for(int y = -1; y < height + 1; y++)
        {
            for(int x = -1; x < width + 1; x++)
            {            
                var y_tile = y >= 0 ? y % TileCount.Height : TileCount.Height + y;
                var x_tile = x >= 0 ? x % TileCount.Width  : TileCount.Width  + x;

                var tileIndex = (int)(y_tile * TileCount.Width) + x_tile;

                var uvCoords = TileSet.GetUV(Tiles[tileIndex]);

                vertices[vertex_offset++] = new VertexPositionColorTexture(
                    new Vector3(x_abs + x * TileSet.TileSize.Width, y_abs + y * TileSet.TileSize.Height, 0),
                    Color,
                    uvCoords.TopLeft
                );

                vertices[vertex_offset++] = new VertexPositionColorTexture(
                    new Vector3(x_abs + x * TileSet.TileSize.Width + TileSet.TileSize.Width,  y_abs + y * TileSet.TileSize.Height, 0),
                    Color,
                    uvCoords.TopRight
                );

                vertices[vertex_offset++] = new VertexPositionColorTexture(
                    new Vector3(x_abs + x * TileSet.TileSize.Width,  y_abs + y * TileSet.TileSize.Height + TileSet.TileSize.Height, 0),
                    Color,
                    uvCoords.BottomLeft
                );

                vertices[vertex_offset++] =  new VertexPositionColorTexture(
                    new Vector3(x_abs + x * TileSet.TileSize.Width + TileSet.TileSize.Width,  y_abs + y * TileSet.TileSize.Height + TileSet.TileSize.Height, 0),
                    Color,
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