using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Legends.Engine.Graphics2D;

namespace Legends.Engine.Animation;

/*
public class SpriteKeyframeAnimation : KeyframeAnimation<Rectangle>
{
    private Sprite _sprite;

    public SpriteKeyframeAnimation(Sprite sprite, KeyframeAnimationData data) : this (sprite, data.Name, data.LoopType, data.Frames)
    {

    }
    public SpriteKeyframeAnimation(Sprite sprite, string name, LoopType type, IList<Keyframe<Rectangle>> frames) : base(name, type, frames)
    {
        _sprite = sprite;
    }

    public override void SetCurrentFrameIndex(int index)
    {
        if(_currentIndex != index)
        {
            base.SetCurrentFrameIndex(index);
        }
        _sprite.SourceBounds = _frames[index].Value;
    }
}
*/