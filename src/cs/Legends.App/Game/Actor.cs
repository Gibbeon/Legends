using System.Collections.Generic;
using System.Dynamic;
using SlyEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
/*
namespace Legends.App.Scene
{
    public class Actor
    {
        public DrawableEntity2D Entity { get; private set; }

        public Spatial2D Spatial => Entity.Spatial;

        Actor(Game game)
        {
            Entity = new DrawableEntity2D(game);
        }
    }

    public interface IState
    {
        public bool IsInitialized { get; }
        public enum Status
        {
            Continue,
            Success
        }

        public void Initialize();
        public void Dispose();

        public IState.Status Update(GameTime gameTime);

        public void Draw(GameTime gameTime);
    }

    public class ScriptBuilder
    {        
        protected IList<StateTransition> _stateTransitions;

        public ScriptBuilder Do(IState state)
        {
            _stateTransitions.Add(new StateTransition(){ ActiveState = state });
            return this;
        }

        public ScriptBuilder Then(IState state)
        {
            _stateTransitions[_stateTransitions.Count - 1].SuccessStateIndex = _stateTransitions.Count;
            return Do(state);
        }

        public ScriptBuilder Loop()
        {
            _stateTransitions[_stateTransitions.Count - 1].SuccessStateIndex = 0;
            return this;
        }

        public StateMachine Build()
        {
            return new StateMachine(_stateTransitions);
        }
    }
    
    public class StateTransition
    {
        public IState ActiveState;
        public int SuccessStateIndex;
    }

    public class StateMachine
    {
        protected IList<StateTransition> _stateTransitions;

        public int CurrentIndex { get; private set; }

        public IState Current
        {
            get { return _stateTransitions[CurrentIndex].ActiveState; }
        }

        public StateMachine(IList<StateTransition> transitions)
        {
            _stateTransitions = transitions;
        }

        public void Update(GameTime gameTime)
        {
            if(Current == null) return;

            if(Current.IsInitialized == false)
            {
                Current.Initialize();
            }

            switch(Current.Update(gameTime))
            {
                case IState.Status.Success:
                    SetState(_stateTransitions[CurrentIndex].SuccessStateIndex);
                    break;
                case IState.Status.Continue:
                    break;
            }
        }

        public void SetState(int index)
        {
            Current?.Dispose();
            CurrentIndex = index;
        }

        public void Draw(GameTime gameTime)
        {
            if(Current == null) return;

            if(Current.IsInitialized)
            {
                Current.Draw(gameTime);
            }
        }
    }
}
*/