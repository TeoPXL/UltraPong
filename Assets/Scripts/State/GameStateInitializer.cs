using System.Collections.Generic;
using Objects;
using Shared;
using state;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace State
{
    public class Context
    {
        public List<Arena> ArenaPrefabs;
        public Arena MenuBackgroundArena;
        public Arena SelectedArena;
        public ArenaManager ArenaManager;
        public bool PlayerTwoUsesAI = false;
    }


    public class GameStateInitializer : Singleton<GameStateInitializer>
    {
        private GameStateManager _gameStateManager;
        public Context Context => _gameStateManager.Context;
        public GameState CurrentGameState => _gameStateManager.CurrentGameState;

        #region References

        [SerializeField] private UIManager uiManager;

        [Header("Arena Prefabs")]
        [SerializeField] private List<Arena> arenaPrefabs;           // assign multiple arenas here
        [SerializeField] private Arena menuBackgroundArena;          // assign menu background arena
        [SerializeField] private ArenaManager arenaManager;          // assign the ArenaManager in scene

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
                ArenaPrefabs = arenaPrefabs,
                MenuBackgroundArena = menuBackgroundArena,
                ArenaManager = arenaManager
            });

            _gameStateManager.OnStateChanged += (p, n) => OnGameStateChanged?.Invoke(p, n);
            _gameStateManager.OnStateEntered += s => OnStateEntered?.Invoke(s);
            _gameStateManager.OnStateExited += s => OnStateExited?.Invoke(s);

            _gameStateManager.PushState(new MenuState(_gameStateManager, UIManager.Instance.menuUIPrefab));
        }

        private void Update() => _gameStateManager.Update();
    }
}