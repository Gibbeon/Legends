using Microsoft.Xna.Framework;
using MonoGame.Extended.Tweening;
using MonoGame.Extended;
using Legends.Engine.Graphics2D;

namespace Legends.Engine.Animation;

public class SpriteColorAnimationData : AnimationData
{

    public bool Relative    { get; set; }
    public Color To       { get; set; }

    public override void InitializeChannel(AnimationChannel channel) 
    {
        var progress = new LerpProgress<Color>() {
            To = To,
            From = channel.Controller.Parent.GetComponent<ISpriteRenderable>().Color
        };

        var tween = new Tweener()
                .TweenTo(target: progress, 
                        expression: player => progress.Percentage, 
                        toValue: 1.0f, 
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

        channel.Controller.Parent.GetComponent<ISpriteRenderable>().Color = (((Tween)channel.State).Target as LerpProgress<Color>).Value;
    }
}