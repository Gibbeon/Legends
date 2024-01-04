using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Legends.Engine.Animation;
/*
public class KeyframeAnimation<TType> : IAnimation
{
    public class KeyframeAnimationData
    {
        public IList<Keyframe<TType>> Frames = new List<Keyframe<TType>>();
        public string Name = string.Empty;
        public LoopType LoopType = LoopType.None;
    }

    public class KeyframeEventArgs: EventArgs
    {
        public Keyframe<TType> OldValue;
        public Keyframe<TType> NewValue;
    }

    public event EventHandler<AnimationMessageCallbackEventArgs> MessageCallback;
    public event EventHandler<KeyframeEventArgs> FrameChanged;
    public string Name { get; private set; }
    protected List<Keyframe<TType>> _frames;
    
    public IReadOnlyList<Keyframe<TType>> Frames => _frames.AsReadOnly();
    public int CurrentIndex => _currentIndex;
    protected float _lastElapsedTime;
    protected int _currentIndex;
    public float ElapsedTime  { get; private set; }
    public float Duration => _frames.Sum(n => n.Duration);
    public int Direction { get; private set; }
    public LoopType LoopType { get; set; }
    public bool IsComplete  => CurrentIndex >= _frames.Count;
    public Keyframe<TType> Current => IsComplete ? null : Frames[CurrentIndex];

    public KeyframeAnimation(KeyframeAnimationData data) : this (data.Name, LoopType.Reverse, data.Frames)
    {

    }
    public KeyframeAnimation(string name, LoopType type, IList<Keyframe<TType>> frames)
    {
        Name = name;
        Direction = 1;
        LoopType = type;
        _frames = (frames ?? new List<Keyframe<TType>>()).ToList();
    }

    public void Initialize()
    {
        ElapsedTime = 0;
        Direction = 1;
        SetCurrentFrameIndex(0);
    }

    public void Update(GameTime gameTime)
    {
        if(Current != null)
        {
            _lastElapsedTime = ElapsedTime;
            ElapsedTime     += gameTime.GetElapsedSeconds();
            ProcessMessages();

            while(Current != null && ElapsedTime >= Current.Duration) {
                _lastElapsedTime -= Current.Duration;
                ElapsedTime -= Current.Duration;
                ProcessMessages();
                MoveNext();
            }
        }
    }

    private void ProcessMessages()
    {
        if(Current != null)
        {
            foreach(var kvp in Current.Messages)
            {
                if(kvp.Key >= _lastElapsedTime && kvp.Key < ElapsedTime)
                {
                    foreach(var message in kvp.Value)
                    {
                        MessageCallback?.Invoke(this, new AnimationMessageCallbackEventArgs(this, message));
                    }
                }
            }
        }
    }

    public virtual void SetCurrentFrameIndex(int index)
    {
        var oldIndex = _currentIndex;
        _currentIndex = index;
        FrameChanged?.Invoke(this, new KeyframeEventArgs() { OldValue = oldIndex >= 0 ?_frames[oldIndex] : null, NewValue = index >= 0 ? _frames[index] : null });
        ProcessMessages();
    }

    protected virtual void MoveNext()
    {
        var newFrameIndex = CurrentIndex + Direction;

        if(newFrameIndex >= _frames.Count || newFrameIndex < 0)
        {
            switch(LoopType)
            {
                case LoopType.None: 
                    newFrameIndex = _frames.Count - 1; // is complete
                    break;
                case LoopType.Reverse: 
                    Direction = -Direction;
                    newFrameIndex = newFrameIndex <= 0 ? 1 : _frames.Count - 2;
                    break;
                case LoopType.Loop: 
                    newFrameIndex = 0;
                break;
            }
        }

        newFrameIndex = Math.Max(0, newFrameIndex);
        newFrameIndex = Math.Min(_frames.Count - 1, newFrameIndex);

        if(newFrameIndex != CurrentIndex)
        {
            SetCurrentFrameIndex(newFrameIndex);
        }
    }
}
*/