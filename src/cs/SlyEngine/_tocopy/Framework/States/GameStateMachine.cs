using System;
using Appccelerate.StateMachine;
using Microsoft.Xna.Framework;

namespace LitEngine.Framework.States
{
    public class GameStateMachine<TState, TEvent> : IGameStateMachine
        where TState : IComparable
        where TEvent : IComparable
    {
        public IGameState? CurrentState { get; protected set; }
        protected IStateMachine<TState, TEvent> _stateMachine;

        public GameStateMachine(IStateMachine<TState, TEvent> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Start()
        {
            _stateMachine.Start();
        }

        public void Stop()
        {
            _stateMachine.Stop();
        }

        public void Update(GameTime gameTime)
        {
            if (CurrentState != null) CurrentState.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            if (CurrentState != null) CurrentState.Draw(gameTime);
        }

        public void Fire<TTEvent>(TTEvent eventId)
            where TTEvent : notnull
        {
            _stateMachine.Fire((TEvent)Convert.ChangeType(eventId, typeof(TEvent)));
        }

        public void SetState(IGameState state)
        {
            if (CurrentState != null)
            {
                CurrentState.Dispose();
            }

            CurrentState = state;

            if (CurrentState != null)
            {
                CurrentState.Initialize();
            }
        }
    }
}