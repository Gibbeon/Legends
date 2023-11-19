using System;
using Microsoft.Xna.Framework;

namespace LitEngine.Framework.States
{    public class TimedState : BaseGameState
    {
        public event EventHandler<EventArgs>? Expired;
        public TimeSpan TotalTime { get; protected set; }
        public TimeSpan ElapsedTime { get; protected set; }
        public bool IsExpired
        {
            get;
            private set;
        }

        public override void Update(GameTime gameTime)
        {
            ElapsedTime += gameTime.ElapsedGameTime;

            if (ElapsedTime < TotalTime)
            {
                IsExpired = true;
                if (Expired != null) Expired(this, EventArgs.Empty);
            }
        }

        public override void Draw(GameTime gameTime)
        {

        }
    }
}