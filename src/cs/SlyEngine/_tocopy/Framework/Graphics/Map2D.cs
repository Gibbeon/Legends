using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LitEngine.Framework.Graphics
{
    public interface IMapTile
    {
        public Rectangle? TextureCoordinates { get; }
    }

    public struct BasicMapTile : IMapTile
    {
        public Rectangle? TextureCoordinates { get; set; }
    }

    public class Map2D<TTileType> : SpatialNode, IDrawable2D
    where TTileType : IMapTile
    {
        public event EventHandler<EventArgs>? VisibleChanged;
        public event EventHandler<EventArgs>? DrawOrderChanged;
        public Material2D Material
        {
            get;
            set;
        }
        public Point TileCount
        {
            get; set;
        }
        public Vector2 TileSize
        {
            get;
            set;
        }

        public bool Wrap { get; set; }

        public bool Visible
        {
            get => _visible;
            set => SetVisible(value);
        }

        public int DrawOrder
        {
            get => _drawOrder;
            set => SetDrawOrder(value);
        }

        public float Depth
        {
            get;
            set;
        }

        public TTileType[] Tiles
        {
            get => _tiles;
        }

        protected TTileType[] _tiles;
        protected int _drawOrder;
        protected bool _visible;

        public Map2D(Point tileCount, Vector2 tileSize, Material2D material) :
            this(tileCount, tileSize, material, new TTileType[tileCount.X * tileCount.Y])
        {

        }

        public Map2D(Point tileCount, Vector2 tileSize, Material2D material, TTileType[] tiles)
        {
            TileCount = tileCount;
            Material = material;
            TileSize = tileSize;
            Visible = true;
            Wrap = true;

            _tiles = tiles;
        }

        private void DoDraw(SpriteBatch spriteBatch, Rectangle sprite, Rectangle? textureCoordinates, Color ambientColor)
        {
            if (textureCoordinates.HasValue)
            {
                spriteBatch.Draw(
                    Material.SubTexture2D.Texture2D, //texture
                    //new Rectangle((int)(x * TileSize.X), (int)(y * TileSize.Y), (int)(tileCount * TileSize.X), (int)TileSize.Y),
                    sprite, 
                    textureCoordinates, //Material2D.SubTexture2D.Source
                    ambientColor);
            }
        }


        public virtual void DrawBatched(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!Visible) return;

            var worldViewProjectionMatrix = GlobalWorldMatrix;
            
            var dimension = Vector2.Transform(new Vector2(
                    GameEngine.Instance.GraphicsDevice.Viewport.Width,
                    GameEngine.Instance.GraphicsDevice.Viewport.Height), 
                    worldViewProjectionMatrix);
            
            var origin = Vector2.Transform(new Vector2(
                GameEngine.Instance.GraphicsDevice.Viewport.X, 
                GameEngine.Instance.GraphicsDevice.Viewport.Y), 
                worldViewProjectionMatrix);
           
            var adjustedTilesize =  Vector2.Transform(TileSize, worldViewProjectionMatrix) - 
                                    Vector2.Transform(Vector2.Zero, worldViewProjectionMatrix);

            adjustedTilesize.X = Math.Max(1, adjustedTilesize.X);
            adjustedTilesize.Y = Math.Max(1, adjustedTilesize.Y);

            int xOffset = -(int)(origin.X / adjustedTilesize.X);
            int yOffset = -(int)(origin.Y / adjustedTilesize.Y);

            int xCount = (int)(GameEngine.Instance.GraphicsDevice.Viewport.Width / adjustedTilesize.X) + 1;
            int yCount = (int)(GameEngine.Instance.GraphicsDevice.Viewport.Height) + 1;

            var coordinates = Tiles[0].TextureCoordinates;
            var drawTileCount = 0;

            var sprite = new Rectangle(0, 0, (int)adjustedTilesize.X, (int)adjustedTilesize.Y);

            for (var y = yOffset - 1; y < (yCount + yOffset); y++)
            {
                for (var x = xOffset - 1; x < (xCount + xOffset); x++)
                {
                    if (!Wrap && (y >= TileCount.Y || x >= TileCount.X || y < 0 || x < 0))
                    {
                        continue;
                    }

                    int index = (int)((y < 0 ? TileCount.Y - y : y) % TileCount.Y) * TileCount.X + ((x < 0 ? TileCount.X - x : x) % TileCount.X);

                    if (Tiles[index].TextureCoordinates == coordinates && false)
                    {
                        drawTileCount++;
                        continue;
                    }

                    //DoDraw(spriteBatch, x - drawTileCount, y, drawTileCount, coordinates, Color.White);
                    sprite = new Rectangle( 
                        (int)((x - drawTileCount) * adjustedTilesize.X),
                        (int)(y * adjustedTilesize.Y), 
                        (int)(drawTileCount * adjustedTilesize.X),
                        (int)(adjustedTilesize.Y)
                    );

                    DoDraw(spriteBatch, sprite, coordinates, Color.White);

                    coordinates = Tiles[index].TextureCoordinates;
                    drawTileCount = 1;
                }

                if (!Wrap && (y >= TileCount.Y || y < 0))
                    continue;

                //DoDraw(spriteBatch, (xCount + xOffset) - drawTileCount, y, drawTileCount, coordinates, Color.White);

                sprite = new Rectangle( 
                        (int)(((xCount + xOffset) - drawTileCount) * adjustedTilesize.X),
                        (int)(y * adjustedTilesize.Y), 
                        (int)(drawTileCount * adjustedTilesize.X),
                        (int)(adjustedTilesize.Y)
                    );

                DoDraw(spriteBatch, sprite, coordinates, Color.White);


                coordinates = Tiles[((y < 0 ? TileCount.Y - y : y) % TileCount.Y) * TileCount.X].TextureCoordinates;
                drawTileCount = 0;
            }

            //DoDraw(spriteBatch, (xCount + xOffset) - drawTileCount, (yCount + yOffset) + 1, drawTileCount, coordinates, Color.White);

        }

        public void Draw(GameTime gameTime)
        {
            if (!Visible) return;

            //var vertexBuffer = new VertexBuffer(GameEngine.Instance.GraphicsDevice, new VertexDeclaration[] {
            //  new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0)
            // }, 2, BufferUsage.None);
            //vertexBuffer.SetData<Vector3>(-, new Vector3[] { GlobalWorldMatrix.Translation, GlobalWorldMatrix.} )
            //GameEngine.Instance.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            //GameEngine.Instance.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
        }
        public override void Update(GameTime gameTime)
        {
            if (!Enabled) return;

            if (HasChanged)
            {
                base.Update(gameTime);
                //WorldSize = Vector3.Transform(new Vector3(Width, Height, 0), GlobalWorldMatrix) - GlobalWorldMatrix.Translation;
            }
            else
            {
                base.Update(gameTime);
            }
        }

        protected void SetVisible(bool value)
        {
            if (_visible != value)
            {
                _visible = value;
                if (VisibleChanged != null)
                {
                    VisibleChanged(this, EventArgs.Empty);
                }
            }
        }
        protected void SetDrawOrder(int value)
        {
            if (_drawOrder != value)
            {
                _drawOrder = value;
                if (DrawOrderChanged != null)
                {
                    DrawOrderChanged(this, EventArgs.Empty);
                }
            }
        }
    }
}
