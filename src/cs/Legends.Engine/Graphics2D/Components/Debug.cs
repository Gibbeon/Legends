using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Legends.Engine.Content;
using Legends.Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
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

    public Vector2 Position { get => _position; set => _position = value; }
    public Color Color { get => Color.WhiteSmoke; set {}}

    public bool ShowFPS { get => Options.GetValueOrDefault("showfps")  == 1; set => Options["showfps"] = value ? 1 : 0; }
    public bool Select { get => Options.GetValueOrDefault("select") == 1; set => Options["select"] = value ? 1 : 0; }

    public Dictionary<string, int> Options { get; set; }

    private List<SceneObject> _objects = new();
    private List<IComponent> _components = new();

    private int _objectIndex;
    
    private int _componentIndex;

    private InputCommandSet _commands;

    private Vector2 _position;
    private Vector2 _positionStart = new (32, 32);

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

        _commands.Add("SELECT", EventType.MouseClicked, MonoGame.Extended.Input.MouseButton.Left, Microsoft.Xna.Framework.Input.Keys.LeftShift);
        _commands.Add("DESELECT", EventType.KeyReleased, Microsoft.Xna.Framework.Input.Keys.Escape, Microsoft.Xna.Framework.Input.Keys.LeftShift);
        _commands.Add("SELECT_NEXT", EventType.KeyReleased, Microsoft.Xna.Framework.Input.Keys.Right, Microsoft.Xna.Framework.Input.Keys.LeftShift);
        _commands.Add("SELECT_PREV", EventType.KeyReleased, Microsoft.Xna.Framework.Input.Keys.Left, Microsoft.Xna.Framework.Input.Keys.LeftShift);
        _commands.Add("SELECT_UP", EventType.KeyReleased, Microsoft.Xna.Framework.Input.Keys.Up, Microsoft.Xna.Framework.Input.Keys.LeftShift);
        _commands.Add("SELECT_DOWN", EventType.KeyReleased, Microsoft.Xna.Framework.Input.Keys.Down, Microsoft.Xna.Framework.Input.Keys.LeftShift);
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
                case "SELECT":   SetFocus(Parent.Scene.GetObjectsAt(Parent.Scene.Camera.TransformWorldToLocal(command.MouseEventArgs.Position.ToVector2())).ToList()); break;
                case "DESELECT": ClearFocus(); break;
                case "SELECT_NEXT": SetObjectFocus(Math.Min(_objects.Count - 1, _objectIndex + 1)); break;
                case "SELECT_PREV": SetObjectFocus( Math.Max(0, _objectIndex - 1)); break;
                case "SELECT_UP":   _componentIndex = Math.Min(_components.Count, _componentIndex + 1); break;
                case "SELECT_DOWN": _componentIndex = Math.Max(0, _componentIndex - 1); break;
                default:
                    Console.WriteLine("Unknown Command: {0}", command.Name); break;             
            }
        }  
    }

    protected void SetFocus(List<SceneObject> objects)
    {
        if(objects.Count > 0)
        {
            _objects = objects;
            SetObjectFocus(0);
        }
    }

    protected void SetObjectFocus(int index)
    {
        _objectIndex = index;
        _componentIndex = 0;
        _components = Enumerable.Concat(_objects[_objectIndex].Components,  _objects[_objectIndex].Behaviors).ToList(); 
    }

    public void ClearFocus()
    {
        _objects.Clear();
        _components.Clear();
        _objectIndex = 0;
        _componentIndex = 0;
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
        GC.SuppressFinalize(this);
    }

    public override void Reset()
    {
    }

    public void DrawImmediate(GameTime gameTime, GraphicsResource target = null)
    {
        var spriteBatch = this.GetSpriteBatch(target);
        _position = _positionStart;

        _frameCounter++;
        var stringDisplay = string.Format("fps: {0}", _frameRate);

        spriteBatch.DrawString(Font, stringDisplay, Position, Color, 0, Vector2.Zero, .8f, SpriteEffects.None, 0);
        
        _position.Y += Font.MeasureString(stringDisplay).Height;

        StringBuilder sb = new ();
        if(_objects.Count > 0) {

            spriteBatch.DrawRectangle(_objects[_objectIndex].Scene.Camera.TransformLocalToWorld(_objects[_objectIndex].BoundingRectangle), Color.Red);

            if(_componentIndex == 0)
            {
                sb.AppendLine(_objects[_objectIndex].ToString());
                sb.AppendLine(string.Format("{0} out of {1}", _objectIndex + 1, _objects.Count));  
                stringDisplay = sb.ToString();
                spriteBatch.DrawString(Font, stringDisplay, Position, Color, 0, Vector2.Zero, .8f, SpriteEffects.None, 0);
                _position.Y += Font.MeasureString(stringDisplay).Height;          
            }
            else
            {
                DrawObject<IComponent>(spriteBatch, _components[_componentIndex - 1]);
            }
        }

        if(target is not SpriteBatch)
            spriteBatch?.End();
    }

    protected static string MemberToString(string name, object value)
    {
        if(value is string || value is not IEnumerable enumerable)
        {
            return $"{name} = {value}";
        }

        var index = 0;
        StringBuilder sb = new();
        sb.AppendLine($"{name} of type {value.GetType().Name}");
        foreach(var item in enumerable)
        {
            sb.AppendLine($"[{index}] {item}");
        }

        return sb.ToString();
    }

    public static IEnumerable<MemberInfo> GetMembers(Type derivedType)
    {
        return Enumerable.Concat<MemberInfo>(
                    derivedType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty)
                        .Where(n => 
                            n.CanRead && 
                            (
                                n.GetAccessors().Any(n => n.IsPublic) ||
                                n.IsDefined(typeof(JsonPropertyAttribute))
                            )
                    ),
                    derivedType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        .Where(n => n.IsPublic ||
                                    n.IsDefined(typeof(JsonPropertyAttribute))));
    }

    public void DrawObject<TType>(SpriteBatch spriteBatch, object instance)
    {
        StringBuilder sb = new ();
        var derivedType = instance?.GetType() ?? typeof(TType);
        sb.AppendLine($"Type: {derivedType.Name}");
            
        TextureRegion region = null;
        if(instance == null)
        {
            sb.AppendLine("instance is null");
        }
        else
        {
            foreach(var member in GetMembers(derivedType))
            {
                if(member is PropertyInfo pi)    
                {   
                    if(pi.PropertyType.IsAssignableTo(typeof(TextureRegion)))
                    {
                        region = pi.GetValue(instance) as TextureRegion;
                    } 
                    else
                    {
                        sb.AppendLine(MemberToString(pi.Name, pi.GetValue(instance)));
                    }
                }
                else if(member is FieldInfo fi)    
                { 
                    if(fi.FieldType.IsAssignableTo(typeof(TextureRegion)))
                    {
                        region = fi.GetValue(instance) as TextureRegion;
                    } 
                    else
                    {
                        sb.AppendLine(MemberToString(fi.Name, fi.GetValue(instance)));
                    }
                }
            }
        }

        var stringDisplay = sb.ToString();
        spriteBatch.DrawString(Font, stringDisplay, Position, Color, 0, Vector2.Zero, .8f, SpriteEffects.None, 0);
        _position.Y += Font.MeasureString(stringDisplay).Height;

        if(region != null)
        {
            DrawTextureRegion(spriteBatch, region);
        }
    }

    protected void DrawTextureRegion(SpriteBatch spriteBatch, TextureRegion textureRegion)
    {
        var scale = 3;

        spriteBatch.Draw(
            textureRegion.Texture,
            Position,
            new Rectangle(textureRegion.LocalPosition.ToPoint(), (Size)textureRegion.Size),
            Color.White,
            0,
            Vector2.Zero,
            Vector2.One * scale,
            SpriteEffects.None,
            0);

        
        for(var row = 0; row < textureRegion.TileCount.Height; row++)
        {
            for(var col = 0; col < textureRegion.TileCount.Width; col++)
            {
                var color = textureRegion.Frame == (row * textureRegion.TileCount.Width + col) ? Color.Blue : Color.Red;

                var pos = Position + 
                    new Vector2(textureRegion.Slice.Width  * col * scale, 
                                textureRegion.Slice.Height   * row * scale);
                
                spriteBatch.DrawRectangle(
                    new RectangleF(pos, textureRegion.Slice * scale),
                    color
                );

                

                spriteBatch.DrawString(Font, $"{row * textureRegion.TileCount.Width + col}", pos + new Vector2(2, 2), color);
            }
        }

        _position.Y += textureRegion.Size.Height;
    }
}