using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tweening;
using MonoGame.Extended;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace Legends.Engine.Animation;

public interface IAnimationData
{
    string Name { get; set; }
    LoopType LoopType { get; protected set; }
    float Duration   { get; protected set; }
    float Delay      { get; protected set; }
    int RepeatCount  { get; protected set; }
    float RepeatDelay{ get; protected set; }
    bool Enabled { get; protected set; }
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

    public bool Enabled { get; set; }

    public abstract void InitializeChannel(AnimationChannel channel);
    public abstract void UpdateChannel(AnimationChannel channel, GameTime gameTime);
}