using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Legends.Engine.Graphics2D;

namespace Legends.Engine.Animation;

public class SpriteKeyframeAnimation : KeyframeAnimation<Rectangle>
{
    public SpriteKeyframeAnimation(KeyframeAnimationData data) : this (data.Name, LoopType.Reverse, data.Frames)
    {

    }
    public SpriteKeyframeAnimation(string name, LoopType type, IList<Keyframe<Rectangle>> frames) : base(name, type, frames)
    {

    }
}