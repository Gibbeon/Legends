﻿using System;
using MonoGame.Extended;

namespace Legends.Engine.Animation;


public interface IAnimation: IUpdate
{
    string Name { get; }
    LoopType LoopType { get; set; }

    public void Initialize();
    event EventHandler<AnimationMessageCallbackEventArgs> MessageCallback;
}
