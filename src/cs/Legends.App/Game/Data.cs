using Microsoft.Xna.Framework;
using Legends.Engine.Animation;

public static class Data
{
    public static KeyframeAnimation<Rectangle>.KeyframeAnimationData[] AnimationData = {
        new KeyframeAnimation<Rectangle>.KeyframeAnimationData() {
            Name = "walk_left",
            LoopType = LoopType.Reverse,
            Frames = new Keyframe<Rectangle>[] {
                new Keyframe<Rectangle>( duration: 0.2f, value: new Rectangle(new Point(24 * 0, 36 * 1), new Point(24, 36) )),
                new Keyframe<Rectangle>( duration: 0.2f, value: new Rectangle(new Point(24 * 1, 36 * 1), new Point(24, 36) )),
                new Keyframe<Rectangle>( duration: 0.2f, value: new Rectangle(new Point(24 * 2, 36 * 1), new Point(24, 36) ))
            }
        },new KeyframeAnimation<Rectangle>.KeyframeAnimationData() {
            Name = "walk_right",
            LoopType = LoopType.Reverse,
            Frames = new Keyframe<Rectangle>[] {
                new Keyframe<Rectangle>( duration: 0.2f, value: new Rectangle(new Point(24 * 0, 36 * 2), new Point(24, 36) )),
                new Keyframe<Rectangle>( duration: 0.2f, value: new Rectangle(new Point(24 * 1, 36 * 2), new Point(24, 36) )),
                new Keyframe<Rectangle>( duration: 0.2f, value: new Rectangle(new Point(24 * 2, 36 * 2), new Point(24, 36) ))
            }
        },new KeyframeAnimation<Rectangle>.KeyframeAnimationData() {
            Name = "walk_up",
            LoopType = LoopType.Reverse,
            Frames = new Keyframe<Rectangle>[] {
                new Keyframe<Rectangle>( duration: 0.2f, value: new Rectangle(new Point(24 * 0, 36 * 3), new Point(24, 36) )),
                new Keyframe<Rectangle>( duration: 0.2f, value: new Rectangle(new Point(24 * 1, 36 * 3), new Point(24, 36) )),
                new Keyframe<Rectangle>( duration: 0.2f, value: new Rectangle(new Point(24 * 2, 36 * 3), new Point(24, 36) ))
            }
        },new KeyframeAnimation<Rectangle>.KeyframeAnimationData() {
            Name = "walk_down",
            LoopType = LoopType.Reverse,
            Frames = new Keyframe<Rectangle>[] {
                new Keyframe<Rectangle>( duration: 0.2f, value: new Rectangle(new Point(24 * 0, 0), new Point(24, 36) )),
                new Keyframe<Rectangle>( duration: 0.2f, value: new Rectangle(new Point(24 * 1, 0), new Point(24, 36) )),
                new Keyframe<Rectangle>( duration: 0.2f, value: new Rectangle(new Point(24 * 2, 0), new Point(24, 36) ))
            }
        },new KeyframeAnimation<Rectangle>.KeyframeAnimationData() {
            Name = "idle_left",
            LoopType = LoopType.None,
            Frames = new Keyframe<Rectangle>[] {                
                new Keyframe<Rectangle>( duration: 0.5f, value: new Rectangle(new Point(24 * 1, 36 * 1), new Point(24, 36) ))
            }
        },new KeyframeAnimation<Rectangle>.KeyframeAnimationData() {
            Name = "idle_right",
            LoopType = LoopType.None,
            Frames = new Keyframe<Rectangle>[] {
                new Keyframe<Rectangle>( duration: 0.5f, value: new Rectangle(new Point(24 * 1, 36 * 2), new Point(24, 36) ))
            }
        },new KeyframeAnimation<Rectangle>.KeyframeAnimationData() {
            Name = "idle_up",
            LoopType = LoopType.None,
            Frames = new Keyframe<Rectangle>[] {
                new Keyframe<Rectangle>( duration: 0.5f, value: new Rectangle(new Point(24 * 1, 36 * 3), new Point(24, 36) ))
            }
        },new KeyframeAnimation<Rectangle>.KeyframeAnimationData() {
            Name = "idle_down",
            LoopType = LoopType.None,
            Frames = new Keyframe<Rectangle>[] {
                new Keyframe<Rectangle>( duration: 0.5f, value: new Rectangle(new Point(24 * 1, 0), new Point(24, 36) ))
            }
        }
    };

}