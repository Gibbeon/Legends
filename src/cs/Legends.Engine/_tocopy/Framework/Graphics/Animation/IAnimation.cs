using System;
using Microsoft.Xna.Framework;

namespace LitEngine.Framework.Graphics.Animation
{
    public interface IAnimation : IUpdateable
    {
        bool Completed { get; }
        public event EventHandler<EventArgs>? CompletedChanged;
    }
}
