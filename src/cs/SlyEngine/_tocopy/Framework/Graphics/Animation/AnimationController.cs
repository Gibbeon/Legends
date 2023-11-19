using System;
using System.Collections.Generic;
using LitEngine.Framework.Collections;
using Microsoft.Xna.Framework;

namespace LitEngine.Framework.Graphics.Animation
{
    public class AnimationController : IUpdateable
    {
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
        public IReadOnlyList<IAnimation> Animations
        {
            get => _animations;
        }
        protected bool _enabled;
        protected int _updateOrder; 
        protected List<IAnimation> _animations;
        protected UpdateableListIndex<IAnimation> _animationsIndex;
        public AnimationController()
        {
            _animations = new List<IAnimation>();
            _animationsIndex = new UpdateableListIndex<IAnimation>(_animations);
            _enabled = true;
        }
        public void Update(GameTime gameTime)
        {
            if (!Enabled) return;

            foreach (var animation in _animations)
            {
                animation.Update(gameTime);
            }

            _animations.RemoveAll((IAnimation item) => { return item.Completed; });
        }

        public void Add(IAnimation animation)
        {
            _animationsIndex.Add(animation);
        }

        public void Remove(IAnimation animation)
        {
            _animationsIndex.Remove(animation);
        }
        protected void SetEnabled(bool value)
        {
            if (_enabled != value)
            {
                _enabled = value;
                if (EnabledChanged != null)
                {
                    EnabledChanged(this, EventArgs.Empty);
                }
            }
        }
        protected void SetUpdateOrder(int value)
        {
            if (_updateOrder != value)
            {
                _updateOrder = value;
                if (UpdateOrderChanged != null)
                {
                    UpdateOrderChanged(this, EventArgs.Empty);
                }
            }
        }
    }
}
