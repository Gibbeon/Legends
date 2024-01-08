using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Legends.Engine.Animation;

public class AnimationController : Component
{
    protected IList<AnimationChannel> _channels;
    
    [JsonProperty(nameof(Animations))]
    protected Ref<AnimationCollection> _data;

    [JsonIgnore]
    public AnimationCollection Animations => _data.Get();

    [JsonIgnore]
    public IList<AnimationChannel> Channels => _channels;

    public AnimationController(IServiceProvider services, SceneObject parent) : base(services, parent)
    {
        _data = new Ref<AnimationCollection>(new AnimationCollection());
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
        _data.Get().Clear();
        Initialize();
    }
}

