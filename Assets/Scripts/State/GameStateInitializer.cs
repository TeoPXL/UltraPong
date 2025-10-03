using System;
using Shared;
using state;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace State
{
    public struct Context
    {
    }

    public class GameStateInitializer : Singleton<GameStateInitializer>
    {
        private GameStateManager _gameStateManager;
        public Context Context => _gameStateManager.Context;

        #region References

        [SerializeField] UIManager uiManager;

        #endregion

        #region Events

        public event UnityAction<GameState, GameState> OnGameStateChanged;
        public event UnityAction<GameState> OnStateEntered;
        public event UnityAction<GameState> OnStateExited;

        #endregion

        private void Start()
        {
            _gameStateManager = new GameStateManager(new Context
            {
            });

            _gameStateManager.OnStateChanged += (p, n) => OnGameStateChanged?.Invoke(p, n);
            _gameStateManager.OnStateEntered += s => OnStateEntered?.Invoke(s);
            _gameStateManager.OnStateExited += s => OnStateExited?.Invoke(s);

            _gameStateManager.PushState(new MenuState(_gameStateManager, UIManager.Instance.menuUIPrefab));
        }
    }
}