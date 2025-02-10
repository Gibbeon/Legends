using Microsoft.Xna.Framework;
using MonoGame.Extended.Tweening;
using MonoGame.Extended;

namespace Legends.Engine.Animation;

public class RotateAnimationData : AnimationData
{
    public bool Relative    { get; set; }
    public float To       { get; set; }

    public override void InitializeChannel(AnimationChannel channel) 
    {
        var tween = new Tweener()
                .TweenTo(target: channel.Controller.Parent, 
                        expression: player => player.Rotation, 
                        toValue: To + (Relative ? channel.Controller.Parent.Rotation : 0.0f), 
                        duration: Duration, 
                        delay: 0)
                .Easing(EasingFunctions.Linear);
        switch(LoopType)
        {
            case LoopType.Loop: 
                tween.Repeat(RepeatCount <= 0 ? -1 : RepeatCount, RepeatDelay);
                break;
            case LoopType.Reverse:
                tween.Repeat(RepeatCount <= 0 ? -1 : RepeatCount, RepeatDelay)
                     .AutoReverse();
                break;
        }

        channel.State = tween;
    }

    public override void UpdateChannel(AnimationChannel channel, GameTime gameTime) 
    {
        ((Tween)channel.State).Update(gameTime.GetElapsedSeconds());
    }
}