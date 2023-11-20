using System;
using Legends.Content.Pipline;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.Screens;
using SlyEngine.Graphics2D;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework.Audio;
using System.Linq;
using Assimp.Configs;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Reflection.Metadata;

namespace SlyEngine.Graphics2D.Animation;

public class AnimationEventArgs
{
    public IAnimation Animation { get; private set; }

    public AnimationEventArgs(IAnimation animation)
    {
        Animation = animation;
    }
}

public class AnimationMessageCallbackEventArgs : AnimationEventArgs
{
    public string Message { get; private set; }

    public AnimationMessageCallbackEventArgs(IAnimation animation, string message) : base(animation)
    {
        Message = message;
    }
}

public interface IAnimation: IUpdate
{
    string Name { get; }
    LoopType LoopType { get; set; }
    IAnimation Clone();
    event EventHandler<AnimationMessageCallbackEventArgs> MessageCallback;
}

public class Keyframe<TType>
{
    public TType Value { get; private set; }
    public float Duration { get; private set; }
    public IDictionary<float, IList<string>> Messages { get; private set; }

    public Keyframe(TType value, float duration, IDictionary<float, IList<string>> messages)
    {
        Value = value;
        Duration = duration;
        Messages = messages;
    }

    public Keyframe(TType value, float duration) : this (value, duration, new Dictionary<float, IList<string>>())
    {

    }
}

public enum LoopType
{
    None,
    Loop,
    Reverse
}

public class KeyframeAnimation<TType> : IAnimation
{
    public class KeyframeAnimationData
    {
        public IList<Keyframe<TType>> Frames = new List<Keyframe<TType>>();
        public string Name = string.Empty;
    }
    public event EventHandler<AnimationMessageCallbackEventArgs>? MessageCallback;
    public string Name { get; private set; }
    private List<Keyframe<TType>> _frames;
    private int _currentIndex;
    public IReadOnlyList<Keyframe<TType>> Frames => _frames.AsReadOnly();
    public int CurrentIndex => _currentIndex;
    private float _lastElapsedTime;
    public float ElapsedTime  { get; private set; }
    public float Duration => _frames.Sum(n => n.Duration);
    public int Direction { get; private set; }
    public LoopType LoopType { get; set; }
    public bool IsComplete  => CurrentIndex >= _frames.Count;
    public Keyframe<TType>? Current => IsComplete ? null : Frames[CurrentIndex];

    public KeyframeAnimation(KeyframeAnimationData data) : this (data.Name, data.Frames)
    {

    }
    public KeyframeAnimation(string name, IList<Keyframe<TType>> frames)
    {
        Name = name;
        Direction = 1;
        _frames = (frames ?? new List<Keyframe<TType>>()).ToList();
    }
    public void Update(GameTime gameTime)
    {
        if(Current != null)
        {
            _lastElapsedTime = ElapsedTime;
            ElapsedTime     += gameTime.GetElapsedSeconds();
            ProcessMessages();

            while(ElapsedTime >= Current.Duration) {
                _lastElapsedTime -= Current.Duration;
                ElapsedTime -= Current.Duration;
                ProcessMessages();

                MoveNext();
            }
        }
    }

    private void ProcessMessages()
    {
        if(Current != null)
        {
            foreach(var kvp in Current.Messages)
            {
                if(kvp.Key >= _lastElapsedTime && kvp.Key < ElapsedTime)
                {
                    foreach(var message in kvp.Value)
                    {
                        MessageCallback?.Invoke(this, new AnimationMessageCallbackEventArgs(this, message));
                    }
                }
            }
        }
    }

    public void SetCurrentFrameIndex(int index)
    {
        _currentIndex = index;
        ProcessMessages();
    }

    protected void MoveNext()
    {
        var newFrameIndex = CurrentIndex + Direction;

        if(newFrameIndex >= _frames.Count || newFrameIndex < 0)
        {
            switch(LoopType)
            {
                case LoopType.None: 
                    newFrameIndex = _frames.Count; // is complete
                    break;
                case LoopType.Reverse: 
                    Direction = -Direction;
                    newFrameIndex = newFrameIndex < 0 ? 1 : _frames.Count - 2;
                    break;
                case LoopType.Loop: 
                    newFrameIndex = 0;
                break;
            }
        }

        if(newFrameIndex != CurrentIndex)
        {
            SetCurrentFrameIndex(newFrameIndex);
        }
    }

    public IAnimation Clone()
    {
        return new KeyframeAnimation<TType>(Name, _frames);
    }
}
public class AnimationChannel
{
    public IAnimation? Current { get; set; }
    public float Speed { get; set; }
    public bool Enabled { get; set; }
    public AnimationChannel Loop(LoopType type = LoopType.Loop) 
    {   
        if(Current != null)
        {
            Current.LoopType = type; 
        }
        return this; 
    }

    public AnimationChannel AtSpeed(float value) 
    { 
        Speed = value; 
        return this; 
    }

    public AnimationChannel Play(IAnimation? animation, EventHandler<AnimationMessageCallbackEventArgs>? callback = default)
    {
        if(Current == animation || Current?.Name == animation?.Name) return this;
        
        if(Current != null)
        {
            Current.MessageCallback -= callback;
        }
        
        Current = animation?.Clone();

        if(Current != null)
        {   
            Current.MessageCallback += callback;
        }

        return this;
    }

    public AnimationChannel Pause()
    {
        Enabled = false;
        return this;
    }

    public AnimationChannel Resume()
    {
        Enabled = true;
        return this;
    }

    public void Update(GameTime gameTime)
    {   
        if(!Enabled) return;
        Current?.Update(new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime * Speed));
    }
}

public class AnimationController
{
    public EventHandler<AnimationMessageCallbackEventArgs>? MessageCallback;
    public IList<IAnimation> Animations { get; private set; }
    private List<AnimationChannel> _channels;
    public IReadOnlyList<AnimationChannel> AnimationChannels  => _channels.AsReadOnly();
    public delegate void AnimationEventHandler(object sender, EventArgs args);
    public AnimationChannel Play(int channel, string name) 
    {
        if(_channels.Count < channel)
        {
            _channels.AddRange(Enumerable.Repeat(new AnimationChannel(), channel - _channels.Count + 1));
        }

        return AnimationChannels[channel].Play(Animations.SingleOrDefault(n => n.Name == name), MessageCallback);
    }
    public AnimationController(int channels = 1)
    {
        Animations = new List<IAnimation>();

        _channels = new List<AnimationChannel>();
        _channels.AddRange(Enumerable.Repeat(new AnimationChannel(), channels));
    }
}

public class Sprite : SpatialNode
{
    public AnimationController Animation;
    //public ParticleEmitter Particles;

    public Sprite()
    {
        Animation = new AnimationController();
    }
    public Sprite(SpatialNode parent) : base(parent)
    {
        //Animation = new AnimationController();
    }
}

public static class DirectionConstants
{
    public static Vector2 Left = new Vector2(-1, 0);
    public static Vector2 Right = new Vector2(1, 0);
    public static Vector2 Up = new Vector2(0, -1);
    public static Vector2 Down = new Vector2(0, 1);

    public static Vector2 GetNearestFacing(Vector2 value)
    {
        if(MathF.Abs(value.Y) >= MathF.Abs(value.X))
        {
            return value.Y > 0 ? Up : Down;
        }
        return value.X > 0 ? Left : Right;
    }
}

public class ValueResolver<TKeyType, TType>
{
    public IDictionary<TKeyType, IList<Tuple<Func<TType, bool>, TKeyType>>> _lookup;
    public TKeyType Resolve(TKeyType key, TType value)
    {
        if(!_lookup.ContainsKey(key)) return key;
        foreach(var item in _lookup[key])
        {
            if(item.Item1(value))
            {
                return item.Item2;
            }
        }

        return key;
    }

    public void Add(TKeyType name, Func<TType, bool> resolver, TKeyType result)
    {
        if(!_lookup.ContainsKey(name))
        {
            _lookup.Add(name, new List<Tuple<Func<TType, bool>, TKeyType>>());
        }

        _lookup[name].Add(Tuple.Create(resolver, result));
    }
}

public class Actor : SpatialNode
{
    public Sprite Body;
    public Vector2 Facing;
    public float Speed;
    
    ValueResolver<string, Actor> _resolver;

    public Actor()
    {
        Speed = 1.0f;
        Body = new Sprite(this);
        Facing = DirectionConstants.Down;
        _resolver = new ValueResolver<string, Actor>();
        _resolver.Add("walk", (actor) => { return actor.Facing == DirectionConstants.Left; }, "walk_left");
        _resolver.Add("walk", (actor) => { return actor.Facing == DirectionConstants.Right; }, "walk_right");
        _resolver.Add("walk", (actor) => { return actor.Facing == DirectionConstants.Up; }, "walk_up");
        _resolver.Add("walk", (actor) => { return actor.Facing == DirectionConstants.Down; }, "walk_left");
    }

    public override void Move(Vector2 direction)
    {
        Facing = DirectionConstants.GetNearestFacing(direction);
        Body.Animation.Play(0, _resolver.Resolve("walk", this)).AtSpeed(Speed);

        base.Move(Facing * Speed);
    }
}

/*

public class MainGame : Game
{
    void Do()
    {
        var _sprite = new Sprite();

        _sprite.Animation.MessageCallback += DoMessageCallback;

        _sprite.Animation.Play(1, "sparkle_shower").Loop();

        if(true) // input left
        {
            _sprite.Move(new Vector2(-1, 0));
            _sprite.Animation.Play(0, "walk_left").Loop();
        }

        if(true) // input running left
        {
            _sprite.Move(new Vector2(-2,0));
            _sprite.Animation.Play(0, "run_left").AtSpeed(2).Loop();
        }

        if(true) // else is idling
        {
            _sprite.Animation.Play(0, "idle_left").Loop();
        }
    }

    void DoMessageCallback(object? sender, AnimationMessageCallbackEventArgs args)
    {
        switch(args.Message)
        {
            case "footstep":
                //AudioEmitter play sound footstep
                //PartialEmitter make footprint (check orientation)
                break;
            case "sparkle_shower":
                //ParticleEmitter play a particle shower
                break;
        }
        
    }
    private GraphicsDeviceManager _graphics;
    private ScreenManager _screenManager;
    
    public MainGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        _screenManager = new ScreenManager();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();   

        Components.Add(_screenManager);        
    }

    protected override void LoadContent()
    {
        _screenManager.LoadScreen(new Screens.TitleScreen(this));

        Global.Fonts.LoadContent(this.Content);
        Global.Defaults.LoadContent(this.Content);

        //var value = Content.Load<SpriteData>("npc1");
        //Console.WriteLine(value.Spatial.Position);
    }

    protected override void Update(GameTime gameTime)
    {
        _screenManager.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _screenManager.Draw(gameTime);

        base.Draw(gameTime);
    }
}

*/