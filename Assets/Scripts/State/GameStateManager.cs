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

        private int score; // We probably want to keep score
        public int Score
        {
            get => score;
            set
            {
                score = value; 
                OnScoreChanged?.Invoke(score);
            }
            
        }
        public event UnityAction<int> OnScoreChanged;

        [Header("Optional")] public Transform uiRoot; // parent instantiated UI under this

        public static GameStateManager Instance { get; private set; }

        private Stack<State> stateStack = new();

        public State CurrentState => stateStack.Count > 0 ? stateStack.Peek() : null;

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
            stateStack.Push(newState);
            newState.Enter();
            NotifyTopStateChanged();
        }

        public void PopState()
        {
            if (stateStack.Count == 0) return;

            var top = stateStack.Pop();
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
            if (stateStack.Count > 0)
            {
                var top = stateStack.Pop();
                try
                {
                    top.Exit();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            stateStack.Push(newState);
            newState.Enter();
            NotifyTopStateChanged();
        }

        /// <summary>
        /// Clears all states and replaces them with a new state.
        /// Used for Exit to Menu or similar full-reset transitions.
        /// </summary>
        public void ClearAndChangeState(State newState)
        {
            while (stateStack.Count > 0)
            {
                var s = stateStack.Pop();
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
        public bool IsMenu => CurrentState.GameState == GameState.Menu;
        public bool IsIdle => CurrentState.GameState == GameState.Idle;
        public bool IsPlaying => CurrentState.GameState == GameState.Playing;
        public bool IsPaused => (CurrentState.GameState == GameState.Paused);
        public bool IsWin => CurrentState.GameState == GameState.Win;

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