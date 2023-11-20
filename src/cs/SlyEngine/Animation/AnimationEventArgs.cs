using System;

namespace SlyEngine.Animation;

public class AnimationEventArgs : EventArgs
{
    public IAnimation Animation { get; private set; }

    public AnimationEventArgs(IAnimation animation)
    {
        Animation = animation;
    }
}
