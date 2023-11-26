using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.BitmapFonts;
using Legends.Engine.Graphics2D;
namespace Legends.Engine;

public class RenderService : IRenderService
{
    public class Frame
    {
        private List<Layer> _layers;

        public IReadOnlyList<Layer> Layers => _layers.AsReadOnly();

        public Frame(int layers)
        {
            _layers = new List<Layer>(Enumerable.Repeat(new Layer(), layers));
        }

        public void Reset()
        {
            foreach(var layer in _layers)
            {
                layer.Reset();
            }
        }
    }

    public class PositionDrawableComparer : IComparer<IBatchDrawable>
    {
        public int Compare(IBatchDrawable? x, IBatchDrawable? y)
        {
            return x.Position.Y.CompareTo(y.Position.Y);
        }
    }
   

    public class Layer
    {
        private IList<IBatchDrawable> _drawables;

        public IList<IBatchDrawable> Drawables => _drawables;

        public IOrderedEnumerable<IBatchDrawable> OrderedDrawables => _drawables.Where(n=> n.IsVisible || true).OrderBy(n => DrawableComparer);

        public IComparer<IBatchDrawable> DrawableComparer { get; set; }

        public Layer()
        {
            _drawables = new List<IBatchDrawable>();
            DrawableComparer = new PositionDrawableComparer();
        }

        public void Reset()
        {
            _drawables.Clear();
        } 
    }

    public Color? ClearColor;

    public Texture2D DefaultTexture { get; private set; }

    public int LayerCount { get; set; }

    public SystemServices Services { get; private set; }

    public Frame Current { get; private set; }

    public RenderState DefaultRenderState { get; set; }

    public Effect DefaultEffect { get; set; }

    public Matrix DefaultProejctionMatrix { get; set; }

    private SpriteBatch _spriteBatch;

    private RenderState _renderState;

    public RenderService(SystemServices services, int layerCount = 1)
    {
        Services = services;
        services.Services.AddService<IRenderService>(this);
        LayerCount = layerCount;
        Current = new Frame(LayerCount);
        DefaultRenderState = new RenderState();
        _renderState = new RenderState();
        ClearColor = Color.Black;
    }

    public void DrawBatched(IBatchDrawable drawable)
    {
        Current.Layers[0].Drawables.Add(drawable);
    }

    public void Initialize()
    {
        if(DefaultTexture == null || _spriteBatch == null || DefaultEffect == null) // initialize
        {            
            _spriteBatch = new SpriteBatch(Services.GraphicsDevice);

            DefaultTexture = new Texture2D(Services.GraphicsDevice, 1, 1);
            DefaultTexture.SetData<Color>(new Color[] { Color.Green });

            DefaultEffect = new BasicEffect (Services.GraphicsDevice)
            {
                VertexColorEnabled = true,
                TextureEnabled = true
            };

            Matrix projection; 
            Matrix.CreateOrthographicOffCenter(0f, Services.GraphicsDevice.Viewport.Width, Services.GraphicsDevice.Viewport.Height, 0f, 0f, -1f, out projection);

            DefaultRenderState.Projection = projection;//Camera.Projection;
            
        }  
    }

    public void SetCamera(Camera camera)
    {
        DefaultRenderState.View = camera.View;
        DefaultRenderState.Projection = camera.Projection;        
    }

    public void Draw(GameTime gameTime)
    {        
        DefaultRenderState.CopyTo(_renderState);

        if(ClearColor != null)
        {
            //Services.GraphicsDevice.Clear(ClearColor.Value);
        }

        bool batchStarted = false;

        foreach(var layer in Current.Layers)
        {
            foreach(var drawable in layer.OrderedDrawables)
            {
                if(!batchStarted|| ((drawable.RenderState ?? DefaultRenderState) != _renderState))
                {
                    if(batchStarted)
                    {
                        _spriteBatch.End();
                    }

                    if((_renderState.Effect ?? DefaultEffect) is IEffectMatrices mtxEffect)
                    {
                        mtxEffect.View         = _renderState.View.GetValueOrDefault(Matrix.Identity);
                        mtxEffect.Projection   = _renderState.Projection.GetValueOrDefault(Matrix.Identity);
                    } 

                    _spriteBatch.Begin(
                        _renderState.SpriteSortMode,
                        _renderState.BlendState,
                        _renderState.SamplerState,
                        _renderState.DepthStencilState,
                        _renderState.RasterizerState,
                        _renderState.Effect ?? DefaultEffect,
                        _renderState.World
                    );

                    batchStarted = true;
                }

                if(drawable is ISpriteBatchDrawable)
                {
                    _spriteBatch.Draw(
                        (drawable as ISpriteBatchDrawable).SourceData ?? DefaultTexture,
                        (drawable as ISpriteBatchDrawable).DestinationBounds,
                        drawable.SourceBounds,
                        drawable.Color,
                        drawable.Rotation,
                        Vector2.Zero,//drawable.Origin,
                        drawable.Effect,
                        0);

                } 
                else if (drawable is IBitmapFontBatchDrawable)
                {
                    var fontDrawable = drawable as IBitmapFontBatchDrawable;

                    if(fontDrawable.Rotation > 0 || fontDrawable.Scale != Vector2.One)
                    {
                        _spriteBatch.DrawString(
                            fontDrawable.SourceData,
                            fontDrawable.Text,
                            drawable.Position,
                            drawable.Color,
                            drawable.Rotation,
                            drawable.Origin,
                            drawable.Scale,
                            drawable.Effect,
                            0,
                            null);
                    } else 
                    {
                        _spriteBatch.DrawString(
                            fontDrawable.SourceData,
                            fontDrawable.Text,
                            drawable.Position,
                            drawable.Color);
                    }
                }
            }
        }

        if(batchStarted)
        {
            _spriteBatch.End();
        }

        Current.Reset();
    }
    
}
