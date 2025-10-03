using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace state
{
    [Serializable]
    public class GameSettings
    {
        [SerializeField] private bool coop = true;

        public bool Coop
        {
            get => coop;
            set => coop = value;
        }
    }

    public class GameStateManager : MonoBehaviour
    {
        [Header("UI Prefabs")]
        public MenuUI menuUIPrefab;
        public IdleUI idleUIPrefab;
        public PauseUI pauseUIPrefab;
        public PlayingUI playingUIPrefab;
        public WinUI winUIPrefab;

        private int[] _scores = new int[2]; // Two players

        public int GetScore(int playerIndex)
        {
            return _scores[playerIndex];
        }

        public void SetScore(int playerIndex, int value)
        {
            _scores[playerIndex] = value;
            OnScoreChanged?.Invoke(playerIndex, value);
        }

        public event UnityAction<int, int> OnScoreChanged; 
        
        private int _winner; // Store the winner
        public int Winner
        {
            get => _winner;
            set
            {
                _winner = value; 
                OnWin?.Invoke(_winner);
            }
            
        }
        public event UnityAction<int> OnWin;

        [Header("Optional")] public Transform uiRoot; // parent instantiated UI under this

        public static GameStateManager Instance { get; private set; }

        private Stack<State> _stateStack = new();

        public State CurrentState => _stateStack.Count > 0 ? _stateStack.Peek() : null;

        public GameSettings settings = new();

        // Event when the top-of-stack state changes (useful for systems that want to react)
        public event Action<State> OnTopStateChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            CurrentState?.Tick();
        }

        // Stack API

        public void PushState(State newState)
        {
            _stateStack.Push(newState);
            newState.Enter();
            NotifyTopStateChanged();
        }

        public void PopState()
        {
            if (_stateStack.Count == 0) return;

            var top = _stateStack.Pop();
            try
            {
                top.Exit();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            NotifyTopStateChanged();
        }

        public void ChangeState(State newState)
        {
            if (_stateStack.Count > 0)
            {
                var top = _stateStack.Pop();
                try
                {
                    top.Exit();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            _stateStack.Push(newState);
            newState.Enter();
            NotifyTopStateChanged();
        }

        /// <summary>
        /// Clears all states and replaces them with a new state.
        /// Used for Exit to Menu or similar full-reset transitions.
        /// </summary>
        public void ClearAndChangeState(State newState)
        {
            while (_stateStack.Count > 0)
            {
                var s = _stateStack.Pop();
                try
                {
                    s.Exit();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            PushState(newState);
        }

        private void NotifyTopStateChanged()
        {
            OnTopStateChanged?.Invoke(CurrentState);
        }

        // UI Instantiation Helpers

        public T SpawnUI<T>(T prefab) where T : Component
        {
            if (prefab == null) throw new ArgumentNullException(nameof(prefab));
            var inst = Instantiate(prefab, uiRoot ? uiRoot : null);
            return inst;
        }

        public void DestroyUI(Component ui)
        {
            if (ui == null) return;
            Destroy(ui.gameObject);
        }

        // Convenience / helpers
        public bool IsMenuState => CurrentState.GameState == GameState.Menu;
        public bool IsIdleState => CurrentState.GameState == GameState.Idle;
        public bool IsPlayingState => CurrentState.GameState == GameState.Playing;
        public bool IsPausedState => (CurrentState.GameState == GameState.Paused);
        public bool IsWinState => CurrentState.GameState == GameState.Win;

        [SerializeField] private bool startWithMenu = true;

        private void Start()
        {
            if (startWithMenu && menuUIPrefab != null)
            {
                ChangeState(new MenuState(menuUIPrefab));
            }
        }

    }
}