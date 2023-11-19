using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace LitEngine.Framework.Graphics.Animation
{
    public struct KeyFrame<TType>
    {
        public TimeSpan TimeSpan { get; set; }
        public TType Frame { get; set; }
    }

    public class KeyFrameTimedAnimation<TType> : BaseTimedAnimation
    {
        public event EventHandler CurrentChanged;
        public IReadOnlyList<KeyFrame<TType>> Frames
        {
            get => _frames;
        }
        public int Current
        {
            get;
            protected set;
        }

        protected KeyFrame<TType>[] _frames;

        public KeyFrameTimedAnimation(TimeSpan totalTime, KeyFrame<TType>[] frames) : base(totalTime)
        {
            Current = 0;
            _frames = frames;
        }

        protected override void DoAction()
        {
            for(var i = 0; i < _frames.Length; i++)
             {
                if(_frames[i].TimeSpan > ElapsedTime) {
                    if(Current != i) {
                        SetCurrent(i);
                    }
                }
            }
        }

        protected void SetCurrent(int current) {
            if(Current != current) {
                Current = current;

                if(CurrentChanged != null) {
                    CurrentChanged(this, EventArgs.Empty);
                }
            }
            
        }
    }
}
