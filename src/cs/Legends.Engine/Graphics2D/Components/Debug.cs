using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legends.Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using Newtonsoft.Json;

namespace Legends.Engine.Graphics2D.Components;

public class Debug : Component, ISpriteRenderable
{        
    [JsonProperty(nameof(Font))]
    protected Ref<BitmapFont> FontReference { get; set; }

    [JsonIgnore]
    public BitmapFont Font => FontReference.Get();

    private int _frameRate = 0;
    private int _frameCounter = 0;
    private TimeSpan _elapsedTime = TimeSpan.Zero;
    private Camera _camera;

    public bool Visible { get; set; }

    public RenderState RenderState => null;

    [JsonIgnore]
    public IViewState ViewState => _camera;

    public Vector2 Position => new Vector2(32, 32);
    public Color Color { get => Color.WhiteSmoke; set {}}

    public bool ShowFPS { get => Options.GetValueOrDefault("showfps")  == 1; set => Options["showfps"] = value ? 1 : 0; }
    public bool Select { get => Options.GetValueOrDefault("select") == 1; set => Options["select"] = value ? 1 : 0; }

    public Dictionary<string, int> Options { get; set; }

    [JsonIgnore]
    public IList<SceneObject> _objects;

    public int ObjectIndex { get; set; }

    private InputCommandSet _commands;

    public Debug(IServiceProvider services, 
        SceneObject parent) : base (services, parent)
    {
        Visible = true;
        Options = new();
    }

    public override void Initialize()
    {
        _camera = new Camera(Services, null);
        _camera.Initialize();        
        _camera.OriginNormalized = Vector2.Zero;
        _commands = new InputCommandSet(Services);

        _commands.Add("SELECT", EventType.MouseClicked, MonoGame.Extended.Input.MouseButton.Left);
        _commands.Enabled = true;
    }


    public override void Update(GameTime gameTime)
    {
        _elapsedTime += gameTime.ElapsedGameTime;

        if (_elapsedTime > TimeSpan.FromSeconds(1))
        {
            _elapsedTime -= TimeSpan.FromSeconds(1);
            _frameRate = _frameCounter;
            _frameCounter = 0;
        }

        _camera.Update(gameTime);

        foreach(var command in _commands.EventActions)
        {
            switch(command.Name)
            {
                case "SELECT":   SetFocus(Parent.Scene.GetObjectsAt(command.MouseEventArgs.Position.ToVector2()).ToList()); break;
                default:
                    Console.WriteLine("Unknown Command: {0}", command.Name); break;             
            }
        }  
    }

    public void SetFocus(IList<SceneObject> objects)
    {
        if(objects.Count > 0)
        {
            _objects = objects;
        }
    }

    public override void Draw(GameTime gameTime)
    {
        if(Visible)
        {
            Services.Get<IRenderService>().DrawBatched(this);
        }
    }

    public override void Dispose()
    {
    }

    public override void Reset()
    {
    }

    public void DrawImmediate(GameTime gameTime, GraphicsResource target = null)
    {
        if(target is not SpriteBatch spriteBatch)
        {
            spriteBatch = new SpriteBatch(Services.GetGraphicsDevice());
            
            if (RenderState?.Effect is IEffectMatrices mtxEffect)
            {
                mtxEffect.View = ViewState.View;
                mtxEffect.Projection = ViewState.Projection;
                mtxEffect.World = ViewState.World;
            }

            spriteBatch.Begin(
                SpriteSortMode.Immediate,
                RenderState?.BlendState,
                RenderState?.SamplerState,
                RenderState?.DepthStencilState,
                RenderState?.RasterizerState,
                RenderState?.Effect,
                null
            );
        }

        _frameCounter++;

        spriteBatch.DrawString(Font, string.Format("fps: {0}", _frameRate), Position, Color, 0, Vector2.Zero, .8f, SpriteEffects.None, 0);
        StringBuilder sb = new ();
        if(_objects != null) {
            sb.AppendLine(_objects[ObjectIndex].ToString());
            sb.AppendLine(string.Format("{0} out of {1}", ObjectIndex + 1, _objects.Count));
            
            spriteBatch.DrawString(Font, sb, Position + new Vector2(0, 20), Color, 0, Vector2.Zero, .8f, SpriteEffects.None, 0);
        }

        if(target == null && spriteBatch != null)
            spriteBatch.End();
    }
}