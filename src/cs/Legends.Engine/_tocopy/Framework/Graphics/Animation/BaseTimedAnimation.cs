using System;
using Microsoft.Xna.Framework;

namespace LitEngine.Framework.Graphics.Animation
{

    public abstract class BaseTimedAnimation : BaseAnimation
    {
        public TimeSpan TotalTime
        {
            get;
            protected set;
        }
        public TimeSpan ElapsedTime
        {
            get;
            protected set;
        }

        public bool Wrap
        {
            get;
            protected set;
        } 
        public BaseTimedAnimation(TimeSpan totalTime)
        {
            TotalTime = totalTime;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Enabled) return;           
            ElapsedTime += gameTime.ElapsedGameTime;

            if(Wrap) {
                while(TotalTime >= ElapsedTime) {
                    ElapsedTime -= TotalTime;
                }
            }           

            DoAction();

            // never will be completed if WRap is true
            SetCompleted(ElapsedTime.TotalMilliseconds >= TotalTime.TotalMilliseconds);
        }

        protected abstract void DoAction();
    }

}
