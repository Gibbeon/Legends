using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Legends.Engine.Animation;

public class AnimationController : Component
{
    protected IList<AnimationChannel> _channels;
    
    protected AnimationCollection _animations;

    public AnimationCollection Animations { get => _animations; set => _animations = value; }

    [JsonIgnore]
    public IList<AnimationChannel> Channels => _channels;

    public AnimationController(IServiceProvider services = null, SceneObject parent = null) : base(services, parent)
    {
        _animations = new AnimationCollection();
    }

    protected IList<AnimationChannel> GenerateChannels()
    {
        foreach(var item in Animations) {
            item.Value.Name = item.Key;
        }

        return Animations.Select(n => new AnimationChannel(this, n.Value)).ToList();
    }
    
    public override void Update(GameTime gameTime)
    {
        foreach(var anim in Channels.Where(n => n.Enabled))
        {
            anim.Update(gameTime);
        }
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public override void Initialize()
    {
        _channels = GenerateChannels();
    }

    public override void Reset()
    {
        _animations.Clear();
        Initialize();
    }
}

