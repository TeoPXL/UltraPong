using Objects;
using Shared;
using state;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace State
{
    public struct Context
    {
        public BasicArenaScript Arena;
    }

    public class GameStateInitializer : Singleton<GameStateInitializer>
    {
        private GameStateManager _gameStateManager;
        public Context Context => _gameStateManager.Context;
        public GameState CurrentGameState => _gameStateManager.CurrentGameState;

        #region References

        [SerializeField] UIManager uiManager;
        [SerializeField] BasicArenaScript[] arena;

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
                Arena = arena[2],
            });

            _gameStateManager.OnStateChanged += (p, n) => OnGameStateChanged?.Invoke(p, n);
            _gameStateManager.OnStateEntered += s => OnStateEntered?.Invoke(s);
            _gameStateManager.OnStateExited += s => OnStateExited?.Invoke(s);

            _gameStateManager.PushState(new MenuState(_gameStateManager, UIManager.Instance.menuUIPrefab));
        }

        private void Update() => _gameStateManager.Update();
    }
}