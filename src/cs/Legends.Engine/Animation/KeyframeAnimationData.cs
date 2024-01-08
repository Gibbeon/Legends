using System;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Legends.Engine.Animation;

public abstract class KeyframeAnimationData<TKeyframe> : AnimationData
    where TKeyframe : IKeyframe
{
    protected class KeyState
    {
        public float    LastElapsedTime;
        public float    ElapsedTime;
        public int      Direction = 1;
        public int      CurrentIndex;
    }

    public TKeyframe[] Frames { get; set; }

    public override void InitializeChannel(AnimationChannel channel) 
    {
        channel.State = new KeyState();
        this.Duration = Frames.Sum(n => n.Duration);
    }

    public override void UpdateChannel(AnimationChannel channel, GameTime gameTime) 
    {
        var state       = (KeyState)channel.State;
        var current     = GetFrame(state.CurrentIndex);
        
        state.LastElapsedTime = state.ElapsedTime;
        state.ElapsedTime     += gameTime.GetElapsedSeconds();

        while(current != null && state.ElapsedTime >= current.Duration) {
            state.LastElapsedTime   -= current.Duration;
            state.ElapsedTime       -= current.Duration;
            MoveNext(state);
            current     = GetFrame(state.CurrentIndex);

            if(current != null)
            {
                UpdateFrame(channel, current);
            }
        }
    }

    protected abstract void UpdateFrame(AnimationChannel channel, TKeyframe current);

    protected TKeyframe GetFrame(int index)
    {
        if(index < 0 || index >= Frames.Length) return default;
        return Frames[index];
    }

    protected void MoveNext(KeyState state)
    {
        var newFrameIndex = state.CurrentIndex + state.Direction;

        if(newFrameIndex >= Frames.Length || newFrameIndex < 0)
        {
            switch(LoopType)
            {
                case LoopType.None: 
                    newFrameIndex = Frames.Length - 1; // is complete
                    break;
                case LoopType.Reverse: 
                    state.Direction = -state.Direction;
                    newFrameIndex = newFrameIndex <= 0 ? 1 : Frames.Length - 2;
                    break;
                case LoopType.Loop: 
                    newFrameIndex = 0;
                break;
            }
        }

        newFrameIndex = Math.Max(0, newFrameIndex);
        newFrameIndex = Math.Min(Frames.Length - 1, newFrameIndex);

        if(newFrameIndex != state.CurrentIndex)
        {
            state.CurrentIndex = newFrameIndex;
        }
    }
}
