using System;
using System.Collections.Generic;
using State;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace state
{
    public class GameStateManager
    {
        private Stack<State> _stateStack = new();

        public State CurrentState => _stateStack.Count > 0 ? _stateStack.Peek() : null;
        public GameState CurrentGameState => CurrentState?.GameState ?? GameState.None;
        public Context Context;

        public event UnityAction<GameState, GameState> OnStateChanged;
        public event UnityAction<GameState> OnStateEntered;
        public event UnityAction<GameState> OnStateExited;

        public GameStateManager(Context context)
        {
            Context = context;
        }

        public void PushState(State newState)
        {
            GameState previousGameState = CurrentGameState;
            _stateStack.Push(newState);
            newState.Enter();
            OnStateChanged?.Invoke(previousGameState, newState.GameState);
            OnStateEntered?.Invoke(newState.GameState);
        }

        public void PopState()
        {
            if (_stateStack.Count == 0) return;
            State previousState = CurrentState;
            GameState previousGameState = CurrentGameState;

            previousState.Exit();
            _stateStack.Pop();

            OnStateChanged?.Invoke(previousGameState, CurrentGameState);
            OnStateExited?.Invoke(previousGameState);
        }

        public void ResetStack()
        {
            while (_stateStack.Count > 0) PopState();
        }

        private void Update() => CurrentState?.Tick();
    }
}