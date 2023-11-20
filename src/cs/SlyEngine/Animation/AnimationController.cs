using System;
using System.Linq;
using System.Collections.Generic;

namespace SlyEngine.Animation;

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
