using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace SlyEngine.Animation;

public class KeyframeAnimation<TType> : IAnimation
{
    public class KeyframeAnimationData
    {
        public IList<Keyframe<TType>> Frames = new List<Keyframe<TType>>();
        public string Name = string.Empty;
    }
    public event EventHandler<AnimationMessageCallbackEventArgs>? MessageCallback;
    public string Name { get; private set; }
    private List<Keyframe<TType>> _frames;
    private int _currentIndex;
    public IReadOnlyList<Keyframe<TType>> Frames => _frames.AsReadOnly();
    public int CurrentIndex => _currentIndex;
    private float _lastElapsedTime;
    public float ElapsedTime  { get; private set; }
    public float Duration => _frames.Sum(n => n.Duration);
    public int Direction { get; private set; }
    public LoopType LoopType { get; set; }
    public bool IsComplete  => CurrentIndex >= _frames.Count;
    public Keyframe<TType>? Current => IsComplete ? null : Frames[CurrentIndex];

    public KeyframeAnimation(KeyframeAnimationData data) : this (data.Name, data.Frames)
    {

    }
    public KeyframeAnimation(string name, IList<Keyframe<TType>> frames)
    {
        Name = name;
        Direction = 1;
        _frames = (frames ?? new List<Keyframe<TType>>()).ToList();
    }
    public void Update(GameTime gameTime)
    {
        if(Current != null)
        {
            _lastElapsedTime = ElapsedTime;
            ElapsedTime     += gameTime.GetElapsedSeconds();
            ProcessMessages();

            while(ElapsedTime >= Current.Duration) {
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

    public void SetCurrentFrameIndex(int index)
    {
        _currentIndex = index;
        ProcessMessages();
    }

    protected void MoveNext()
    {
        var newFrameIndex = CurrentIndex + Direction;

        if(newFrameIndex >= _frames.Count || newFrameIndex < 0)
        {
            switch(LoopType)
            {
                case LoopType.None: 
                    newFrameIndex = _frames.Count; // is complete
                    break;
                case LoopType.Reverse: 
                    Direction = -Direction;
                    newFrameIndex = newFrameIndex < 0 ? 1 : _frames.Count - 2;
                    break;
                case LoopType.Loop: 
                    newFrameIndex = 0;
                break;
            }
        }

        if(newFrameIndex != CurrentIndex)
        {
            SetCurrentFrameIndex(newFrameIndex);
        }
    }

    public IAnimation Clone()
    {
        return new KeyframeAnimation<TType>(Name, _frames);
    }
}