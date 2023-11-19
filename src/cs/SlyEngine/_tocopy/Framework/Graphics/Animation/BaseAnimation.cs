using System;
using Microsoft.Xna.Framework;

namespace LitEngine.Framework.Graphics.Animation
{
    public abstract class BaseAnimation : IAnimation
    {
        public event EventHandler<EventArgs>? CompletedChanged;
        public event EventHandler<EventArgs>? EnabledChanged;
        public event EventHandler<EventArgs>? UpdateOrderChanged;

        public bool Enabled
        {
            get => _enabled;
            set => SetEnabled(value);
        }
        public int UpdateOrder
        {
            get => _updateOrder;
            set => SetUpdateOrder(value);
        }

        public bool Completed
        {
            get => _completed;
            protected set => SetCompleted(value);
        }
        protected bool _enabled;
        protected bool _completed;
        protected int _updateOrder;
        public abstract void Update(GameTime gameTime);
        protected void SetCompleted(bool value)
        {
            if (Completed != value)
            {
                Completed = value;
                if (CompletedChanged != null)
                {
                    CompletedChanged(this, EventArgs.Empty);
                }
            }
        }
        protected void SetEnabled(bool value)
        {
            if (Enabled != value)
            {
                Enabled = value;
                if (EnabledChanged != null)
                {
                    EnabledChanged(this, EventArgs.Empty);
                }
            }
        }
        protected void SetUpdateOrder(int value)
        {
            if (UpdateOrder != value)
            {
                UpdateOrder = value;
                if (UpdateOrderChanged != null)
                {
                    UpdateOrderChanged(this, EventArgs.Empty);
                }
            }
        }
    }
}
