using System;
using Microsoft.Xna.Framework;

namespace LitEngine.Framework.States
{
    public abstract class BaseGameState : IGameState
    {
        public event EventHandler<EventArgs>? EnabledChanged;
        public event EventHandler<EventArgs>? UpdateOrderChanged;
        public event EventHandler<EventArgs>? VisibleChanged;
        public event EventHandler<EventArgs>? DrawOrderChanged;

        public bool Enabled
        {
            get;
            private set;
        }

        public int UpdateOrder
        {
            get;
            private set;
        }

        public bool Visible
        {
            get;
            private set;
        }

        public int DrawOrder
        {
            get;
            private set;
        }


        public virtual void Initialize() { }

        public virtual void Dispose() { }
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);


        public void SetEnabled(bool value)
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
        public void SetUpdateOrder(int value)
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

        protected void SetVisible(bool value)
        {
            if (Visible != value)
            {
                Visible = value;
                if (VisibleChanged != null)
                {
                    VisibleChanged(this, EventArgs.Empty);
                }
            }
        }
        protected void SetDrawOrder(int value)
        {
            if (DrawOrder != value)
            {
                DrawOrder = value;
                if (DrawOrderChanged != null)
                {
                    DrawOrderChanged(this, EventArgs.Empty);
                }
            }
        }
    }

}