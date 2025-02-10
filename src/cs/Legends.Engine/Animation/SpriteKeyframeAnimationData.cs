using Legends.Engine.Graphics2D.Components;

namespace Legends.Engine.Animation;

public struct SpriteKeyframe : IKeyframe
{
    public int      FrameIndex          { get; set; }
    public float    Duration            { get; set; }
    public bool     FlipHorizontally    { get; set; }
    public bool     FlipVertically      { get; set; }
}

public class SpriteKeyframeAnimationData : KeyframeAnimationData<SpriteKeyframe>
{
    protected override void UpdateFrame(AnimationChannel channel, SpriteKeyframe current)
    {
        var sprite = channel
            .Controller
            .Parent
            .GetComponent<SpriteRenderable>();

        sprite.FrameIndex       = current.FrameIndex;
        sprite.FlipHorizontally = current.FlipHorizontally;
        sprite.FlipVertically   = current.FlipVertically;
    }
}
