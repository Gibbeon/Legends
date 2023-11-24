using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Legends.Engine.Animation;

public class SpriteKeyframe : Keyframe<Rectangle>
{
    public SpriteKeyframe(Rectangle value, float duration, IDictionary<float, IList<string>> messages) : base(value, duration, messages)
    {

    }

    public SpriteKeyframe(Rectangle value, float duration) : this (value, duration, new Dictionary<float, IList<string>>())
    {

    }
}
