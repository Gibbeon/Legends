using System;
using Microsoft.Xna.Framework;

namespace LitEngine.Framework.Graphics.Animation
{
       public class LerpTimedAnimation<TType> : BaseTimedAnimation
    {
        public TType StartValue
        {
            get;
            protected set;
        }
        public TType EndValue
        {
            get;
            protected set;
        }

        public Action<TType> SetValueFunc
        {
            get;
            protected set;
        }

        public Func<TType, TType, float, TType> InterpolationFunc
        {
            get;
            protected set;
        }
        public LerpTimedAnimation(TType startValue, TType endValue, TimeSpan totalTime, Action<TType> setValueFunc, Func<TType, TType, float, TType> interpolationFunc) : base(totalTime)
        {
            StartValue = startValue;
            EndValue = endValue;
            SetValueFunc = setValueFunc;
            InterpolationFunc = interpolationFunc;
        }

        protected override void DoAction()
        {
            SetValueFunc(
                InterpolationFunc(StartValue, EndValue, Math.Min(1.0f, (float)(ElapsedTime.TotalMilliseconds / TotalTime.TotalMilliseconds)))
            );
        }
    }
}
