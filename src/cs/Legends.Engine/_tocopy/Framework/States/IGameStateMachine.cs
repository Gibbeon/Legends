using System;
using Appccelerate.StateMachine;
using Microsoft.Xna.Framework;

namespace LitEngine.Framework.States
{
    public interface IGameStateMachine
    {
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
        void SetState(IGameState state);
        void Start();
        void Stop();
        void Fire<TEvent>(TEvent eventId);
    }
}