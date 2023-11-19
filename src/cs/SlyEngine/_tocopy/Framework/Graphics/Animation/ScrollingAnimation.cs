using System;
using Microsoft.Xna.Framework;

namespace LitEngine.Framework.Graphics.Animation
{
    public class ScrollingAnimation<TType> : BaseAnimation
    {
        public TType Step
        {
            get;
            protected set;
        }
        public TimeSpan Interval
        {
            get;
            protected set;
        }
        public Action<TType> StepValueFunc
        {
            get;
            protected set;
        }

        private System.Reflection.MethodInfo? _multiply;
        
        public ScrollingAnimation(TType step, TimeSpan interval, Action<TType> stepValueFunc)
        {
            Step = step;
            Interval = interval;
            StepValueFunc = stepValueFunc;
            _multiply = typeof(TType).GetMethod("op_Multiply", new[] { typeof(TType), typeof(float) });
        }

        public override void Update(GameTime gameTime)
        {
            var result = (_multiply != null) ?
                _multiply.Invoke(null, new object[] { Step, (gameTime.ElapsedGameTime.TotalMilliseconds / Interval.TotalMilliseconds) } )
                : 
                (Step as dynamic) * (float)(gameTime.ElapsedGameTime.TotalMilliseconds / Interval.TotalMilliseconds);

            StepValueFunc(
                result
            );
        }
    }
}
