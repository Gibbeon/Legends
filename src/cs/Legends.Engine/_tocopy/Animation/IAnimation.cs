using System;
using MonoGame.Extended;

namespace Legends.Engine.Animation;


public interface IAnimation: IUpdate
{
    string Name { get; }
    LoopType LoopType { get; set; }
    IAnimation Clone();
    event EventHandler<AnimationMessageCallbackEventArgs> MessageCallback;
}
