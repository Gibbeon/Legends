using System;
using System.Linq;
using System.Collections.Generic;
using Legends.Engine.Graphics2D;
using Microsoft.Xna.Framework;
using Microsoft.VisualBasic;

namespace Legends.Engine.Animation;

public class AnimationBehavior : BaseBehavior
{
    private List<AnimationChannel> _channels;
    public EventHandler<AnimationMessageCallbackEventArgs> MessageCallback;
    public IList<IAnimation> Animations { get; private set; }
    public IReadOnlyList<AnimationChannel> AnimationChannels  => _channels.AsReadOnly();

    public AnimationBehavior(SceneObject parent, int channels = 1) : base(parent)
    {
        Animations = new List<IAnimation>();

        _channels = new List<AnimationChannel>();
        _channels.AddRange(Enumerable.Repeat(new AnimationChannel(), channels));
    }
    public AnimationChannel Play(int channel, string name) 
    {
        if(_channels.Count < channel)
        {
            _channels.AddRange(Enumerable.Repeat(new AnimationChannel(), channel - _channels.Count + 1));
        }

        return AnimationChannels[channel].Play(Animations.Single(n => n.Name == name), MessageCallback);
    }

    public override void Update(GameTime gameTime)
    {
        foreach(var channel in _channels)
        {
            channel.Update(gameTime);
        }
    }
}

