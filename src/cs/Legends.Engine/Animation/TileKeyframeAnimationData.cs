using Legends.Engine.Graphics2D.Components;

namespace Legends.Engine.Animation;


public struct TileSetKeyframe : IKeyframe
{
    public ushort       Frame       { get; set; }
    public float        Duration    { get; set; }
}

public class TileSetAnimationData : KeyframeAnimationData<TileSetKeyframe>
{
    protected override void UpdateFrame(AnimationChannel channel, TileSetKeyframe current)
    {
        (channel as TileAnimationChannel)
            .Parent
            .Index[(channel as TileAnimationChannel).TileIndex] = current.Frame;
    }
}
