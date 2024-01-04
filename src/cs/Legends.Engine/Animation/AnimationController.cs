using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Dynamic;
using MonoGame.Extended.Tweening;
using MonoGame.Extended;
using Newtonsoft.Json;
using Legends.Engine.Graphics2D;
using System.Linq.Expressions;
using System.Reflection;

namespace Legends.Engine.Animation;

public interface IAnimationData
{
    string Name { get; set; }
    LoopType LoopType { get; protected set; }
    float Duration   { get; protected set; }
    float Delay      { get; protected set; }
    int RepeatCount  { get; protected set; }
    float RepeatDelay{ get; protected set; }
    void InitializeChannel(AnimationChannel channel);
    void UpdateChannel(AnimationChannel channel, GameTime gameTime);
}


public abstract class AnimationData : IAnimationData
{
    public string Name { get; set; }

    public LoopType LoopType { get; set; }
    public float Duration   { get; set; }
    public float Delay      { get; set; }
    public int RepeatCount  { get; set; }
    public float RepeatDelay{ get; set; }

    public abstract void InitializeChannel(AnimationChannel channel);
    public abstract void UpdateChannel(AnimationChannel channel, GameTime gameTime);
}

public class TranslateAnimationData : AnimationData
{
    public bool Relative    { get; set; }
    public Vector2 To       { get; set; }

    public override void InitializeChannel(AnimationChannel channel) 
    {
        var tween = new Tweener()
                .TweenTo(target: channel.Controller.Parent, 
                        expression: player => player.Position, 
                        toValue: To + (Relative ? channel.Controller.Parent.Position : Vector2.Zero), 
                        duration: Duration, 
                        delay: 0)
                .Easing(EasingFunctions.Linear);
        switch(LoopType)
        {
            case LoopType.Loop: 
                tween.Repeat(RepeatCount <= 0 ? -1 : RepeatCount, RepeatDelay);
                break;
            case LoopType.Reverse:
                tween.Repeat(RepeatCount <= 0 ? -1 : RepeatCount, RepeatDelay)
                     .AutoReverse();
                break;
        }

        channel.State = tween;
    }

    public override void UpdateChannel(AnimationChannel channel, GameTime gameTime) 
    {
        ((Tween)channel.State).Update(gameTime.GetElapsedSeconds());
    }
}

public class ScaleAnimationData : AnimationData
{
    public bool Relative    { get; set; }
    public Vector2 To       { get; set; }

    public override void InitializeChannel(AnimationChannel channel) 
    {
        var tween = new Tweener()
                .TweenTo(target: channel.Controller.Parent, 
                        expression: player => player.Scale, 
                        toValue: To * (Relative ? channel.Controller.Parent.Scale : Vector2.One), 
                        duration: Duration, 
                        delay: 0)
                .Easing(EasingFunctions.Linear);
        switch(LoopType)
        {
            case LoopType.Loop: 
                tween.Repeat(RepeatCount <= 0 ? -1 : RepeatCount, RepeatDelay);
                break;
            case LoopType.Reverse:
                tween.Repeat(RepeatCount <= 0 ? -1 : RepeatCount, RepeatDelay)
                     .AutoReverse();
                break;
        }

        channel.State = tween;
    }

    public override void UpdateChannel(AnimationChannel channel, GameTime gameTime) 
    {
        ((Tween)channel.State).Update(gameTime.GetElapsedSeconds());
    }
}

public class Keyframe
{
    public int      Frame       { get; set; }
    public float    Duration    { get; set; }
    
    public Keyframe()
    {

    }
}

public class SpriteKeyframeAnimationData : AnimationData
{
    protected class KeyState
    {
        public float    LastElapsedTime;
        public float    ElapsedTime;
        public int      Direction;
        public int      CurrentIndex;
    }

    public Keyframe[] Frames { get; set; }

    public override void InitializeChannel(AnimationChannel channel) 
    {
        channel.State = new KeyState();
        this.Duration = Frames.Sum(n => n.Duration);
    }

    public override void UpdateChannel(AnimationChannel channel, GameTime gameTime) 
    {
        var state       = (KeyState)channel.State;
        var current     = GetFrame(state.CurrentIndex);
        
        state.LastElapsedTime = state.ElapsedTime;
        state.ElapsedTime     += gameTime.GetElapsedSeconds();

        while(current != null && state.ElapsedTime >= current.Duration) {
            state.LastElapsedTime   -= current.Duration;
            state.ElapsedTime       -= current.Duration;
            MoveNext(state);
            current     = GetFrame(state.CurrentIndex);

            if(current != null)
            {
                channel.Controller.Parent.GetComponent<Sprite>().TextureRegion.Get().SetFrame(current.Frame);
            }
        }
    }

    protected Keyframe GetFrame(int index)
    {
        if(index < 0 || index >= Frames.Length) return null;
        return Frames[index];
    }

    protected void MoveNext(KeyState state)
    {
        var newFrameIndex = state.CurrentIndex + state.Direction;

        if(newFrameIndex >= Frames.Length || newFrameIndex < 0)
        {
            switch(LoopType)
            {
                case LoopType.None: 
                    newFrameIndex = Frames.Length - 1; // is complete
                    break;
                case LoopType.Reverse: 
                    state.Direction = -state.Direction;
                    newFrameIndex = newFrameIndex <= 0 ? 1 : Frames.Length - 2;
                    break;
                case LoopType.Loop: 
                    newFrameIndex = 0;
                break;
            }
        }

        newFrameIndex = Math.Max(0, newFrameIndex);
        newFrameIndex = Math.Min(Frames.Length - 1, newFrameIndex);

        if(newFrameIndex != state.CurrentIndex)
        {
            state.CurrentIndex = newFrameIndex;
        }
    }
}

public class RotateAnimationData : AnimationData
{
    public bool Relative    { get; set; }
    public float To       { get; set; }

    public override void InitializeChannel(AnimationChannel channel) 
    {
        var tween = new Tweener()
                .TweenTo(target: channel.Controller.Parent, 
                        expression: player => player.Rotation, 
                        toValue: To + (Relative ? channel.Controller.Parent.Rotation : 0.0f), 
                        duration: Duration, 
                        delay: 0)
                .Easing(EasingFunctions.Linear);
        switch(LoopType)
        {
            case LoopType.Loop: 
                tween.Repeat(RepeatCount <= 0 ? -1 : RepeatCount, RepeatDelay);
                break;
            case LoopType.Reverse:
                tween.Repeat(RepeatCount <= 0 ? -1 : RepeatCount, RepeatDelay)
                     .AutoReverse();
                break;
        }

        channel.State = tween;
    }

    public override void UpdateChannel(AnimationChannel channel, GameTime gameTime) 
    {
        ((Tween)channel.State).Update(gameTime.GetElapsedSeconds());
    }
}

public class LerpProgress<TType>
{    
    private static Func<TType, TType, float, TType> _defaultLerp;
    private Func<TType, TType, float, TType> _lerp;    

    public TType From     { get; set; }
    public TType To       { get; set; }
    public TType Value    { get => Lerp(From, To, Percentage); }
    public Func<TType, TType, float, TType> Lerp { get => _lerp ?? GetDefaultLerp(); set => _lerp = value; }

    public float Percentage;

    public static Func<TType, TType, float, TType> GetDefaultLerp()
    {
        return _defaultLerp ?? (_defaultLerp = GenerateDefaultLerp());
    }
    public static Func<TType, TType, float, TType> GenerateDefaultLerp()
    {
        var method = typeof(TType)
            .GetMethod("Lerp", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        
        if(method != null)
        {
            return method.CreateDelegate<Func<TType, TType, float, TType>>();
        }
        else
        {
            var value1 = Expression.Parameter(typeof(TType));
            var value2 = Expression.Parameter(typeof(TType));
            var amount = Expression.Parameter(typeof(float));

            return Expression.Lambda<Func<TType, TType, float, TType>>(
                Expression.Convert(Expression.Multiply(Expression.Convert(Expression.Add(value1, Expression.Subtract(value2, value1)), typeof(float)), amount), typeof(TType)),
                value1, 
                value2, 
                amount
            ).Compile();
        }
    }
}


public class SpriteColorAnimationData : AnimationData
{

    public bool Relative    { get; set; }
    public Color To       { get; set; }

    public override void InitializeChannel(AnimationChannel channel) 
    {
        var progress = new LerpProgress<Color>() {
            To = To,
            From = channel.Controller.Parent.GetComponent<IBatchDrawable>().Color
        };

        var tween = new Tweener()
                .TweenTo(target: progress, 
                        expression: player => progress.Percentage, 
                        toValue: 1.0f, 
                        duration: Duration, 
                        delay: 0)
                .Easing(EasingFunctions.Linear);
        switch(LoopType)
        {
            case LoopType.Loop: 
                tween.Repeat(RepeatCount <= 0 ? -1 : RepeatCount, RepeatDelay);
                break;
            case LoopType.Reverse:
                tween.Repeat(RepeatCount <= 0 ? -1 : RepeatCount, RepeatDelay)
                     .AutoReverse();
                break;
        }

        channel.State = tween;
    }

    public override void UpdateChannel(AnimationChannel channel, GameTime gameTime) 
    {
        ((Tween)channel.State).Update(gameTime.GetElapsedSeconds());

        channel.Controller.Parent.GetComponent<IBatchDrawable>().Color = (((Tween)channel.State).Target as LerpProgress<Color>).Value;

    }
}

public class AnimationChannel
{
    public AnimationController Controller { get; protected set; }
    public IAnimationData AnimationData { get; protected set; }
    public object State { get; set; }    
    public bool Enabled { get; set; }    

    public AnimationChannel(AnimationController controller, IAnimationData data)
    {
        Controller = controller;
        AnimationData = data;
        if(controller.Parent != null)
        {
            data.InitializeChannel(this);
        }
    }

    public void Update(GameTime gameTime)
    {
        AnimationData.UpdateChannel(this, gameTime);
    }
}

public class AnimationCollection : Dictionary<string, IAnimationData>
{

}

public class AnimationController : Component<IComponent>
{
    protected IList<AnimationChannel> _channels;
    
    [JsonProperty(nameof(Animations))]
    protected Ref<AnimationCollection> _data;

    [JsonIgnore]
    public AnimationCollection Animations 
    { 
        get => _data.Get();
    }

    [JsonIgnore]
    public IList<AnimationChannel> Channels 
    { 
        get => _channels ?? (_channels = GenerateChannels());
    }

    public AnimationController(IServiceProvider services, SceneObject parent) : base(services, parent)
    {
        _data = new Ref<AnimationCollection>(new AnimationCollection());
    }

    protected IList<AnimationChannel> GenerateChannels()
    {
        foreach(var item in Animations) item.Value.Name = item.Key;
        return Animations.Select(n => new AnimationChannel(this, n.Value)).ToList();
    }
    
    public override void Update(GameTime gameTime)
    {
        foreach(var anim in Channels)//.Where(n => n.Enabled))
        {
            anim.Update(gameTime);
        }
    }
}

