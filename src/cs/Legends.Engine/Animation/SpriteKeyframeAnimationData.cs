using Legends.Engine.Graphics2D.Components;

namespace Legends.Engine.Animation;

public struct SpriteKeyframe : IKeyframe
{
    public int      Frame       { get; set; }
    public float    Duration    { get; set; }
}

public class SpriteKeyframeAnimationData : KeyframeAnimationData<SpriteKeyframe>
{
    protected override void UpdateFrame(AnimationChannel channel, SpriteKeyframe current)
    {
        channel
            .Controller
            .Parent
            .GetComponent<Sprite>()
            .TextureRegion
            .SetFrame(current.Frame);
    }
}
