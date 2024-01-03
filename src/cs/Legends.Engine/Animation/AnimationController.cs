using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Dynamic;
using MonoGame.Extended.Tweening;
using MonoGame.Extended;
using Newtonsoft.Json;

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
    public Vector2 From     { get; set; }
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

public class AnimationController : Component<IComponent>
{
    protected IList<AnimationChannel> _channels;
    
    public IDictionary<string, IAnimationData> Animations 
    { 
        set {
            foreach(var item in value) item.Value.Name = item.Key;
            _channels = value.Select(n => new AnimationChannel(this, n.Value)).ToList();
        } 
        get => _channels.ToDictionary(n => n.AnimationData.Name, n => n.AnimationData);
        //protected set {

        //} 
    }

    [JsonIgnore]
    public IList<AnimationChannel> Channels 
    { 
        get => _channels;
    }

    public AnimationController(IServiceProvider services, SceneObject parent) : base(services, parent)
    {
        _channels = new List<AnimationChannel>();
    }
    
    public override void Update(GameTime gameTime)
    {
        foreach(var anim in Channels)//.Where(n => n.Enabled))
        {
            anim.Update(gameTime);
        }
    }
}

