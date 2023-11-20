using System;
using MonoGame.Extended;

namespace SlyEngine.Animation;


public interface IAnimation: IUpdate
{
    string Name { get; }
    LoopType LoopType { get; set; }
    IAnimation Clone();
    event EventHandler<AnimationMessageCallbackEventArgs> MessageCallback;
}
