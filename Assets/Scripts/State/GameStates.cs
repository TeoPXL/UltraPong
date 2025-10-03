using Shared;
using UI;
using UnityEngine;
using Objects;

namespace state
{
    public abstract class State
    {
        protected readonly GameStateManager GameStateManager;
        protected State(GameStateManager gameStateManager) => GameStateManager = gameStateManager;

        public abstract GameState GameState { get; }
        public abstract void Enter();
        public abstract void Tick();
        public abstract void Exit();
    }

    #region MenuState
    public class MenuState : State
    {
        public override GameState GameState => GameState.Menu;

        private MenuUI _menuUI;
        private int _selectedArenaIndex = 0;

        public MenuState(GameStateManager gsm, MenuUI menuUI) : base(gsm)
        {
            _menuUI = menuUI;
        }

        public override void Enter()
        {
            _menuUI.gameObject.SetActive(true);
            _menuUI.OnStartClicked += HandleStart;
            _menuUI.OnQuitClicked += HandleQuit;
            _menuUI.OnArenaSelected += HandleArenaSelected;

            var arenaManager = GameStateManager.Context.ArenaManager;
            if (GameStateManager.Context.MenuBackgroundArena != null)
            {
                var arena = arenaManager.SpawnArena(GameStateManager.Context.MenuBackgroundArena);
                arena.ballPrefab.LaunchBall();
            }
        }

        public override void Tick() { }

        public override void Exit()
        {
            _menuUI.gameObject.SetActive(false);
            _menuUI.OnStartClicked -= HandleStart;
            _menuUI.OnQuitClicked -= HandleQuit;
            _menuUI.OnArenaSelected -= HandleArenaSelected;

            GameStateManager.Context.ArenaManager.RemoveCurrentArena();
        }

        private void HandleStart()
        {
            if (_selectedArenaIndex >= 0 && _selectedArenaIndex < GameStateManager.Context.ArenaPrefabs.Count)
            {
                GameStateManager.Context.SelectedArena = GameStateManager.Context.ArenaPrefabs[_selectedArenaIndex];
            }

            GameStateManager.PopState();
            GameStateManager.PushState(new IdleState(GameStateManager, UIManager.Instance.idleUIPrefab));
        }

        private void HandleQuit() => Application.Quit();

        private void HandleArenaSelected(int index)
        {
            _selectedArenaIndex = index;
        }
    }
    #endregion

    #region IdleState
    public class IdleState : State
    {
        public override GameState GameState => GameState.Idle;

        private readonly IdleUI _idleUI;
        private float _timer;
        private const float Delay = 3f;

        public IdleState(GameStateManager gsm, IdleUI idleUI) : base(gsm)
        {
            _idleUI = idleUI;
        }

        public override void Enter()
        {
            _idleUI.gameObject.SetActive(true);
            _timer = 0f;

            var arenaManager = GameStateManager.Context.ArenaManager;

            if (arenaManager.CurrentArena == null)
            {
                var prefab = GameStateManager.Context.SelectedArena ?? GameStateManager.Context.ArenaPrefabs[0];
                arenaManager.SpawnArena(prefab);
            }

            arenaManager.CurrentArena.ResetGame();
            arenaManager.CurrentArena.OnScoreChanged += HandleScoreChanged;
        }

        public override void Tick()
        {
            _timer += Time.deltaTime;
            if (_timer >= Delay)
            {
                _timer = 0f;
                Play();
            }

            if (InputManager.Instance.pauseAction.action.WasPressedThisFrame())
                GameStateManager.PushState(new PauseState(GameStateManager, UIManager.Instance.pauseUIPrefab));
        }

        public override void Exit()
        {
            _idleUI.gameObject.SetActive(false);
            var arena = GameStateManager.Context.ArenaManager.CurrentArena;
            arena.OnScoreChanged -= HandleScoreChanged;
            GameStateManager.Context.ArenaManager.RemoveCurrentArena();
        }

        private void HandleScoreChanged(int p1, int p2) => _idleUI.UpdateScoreText(p1, p2);

        private void Play() => GameStateManager.PushState(new PlayingState(GameStateManager, UIManager.Instance.playingUIPrefab));
    }
    #endregion

    #region PlayingState
    public class PlayingState : State
    {
        public override GameState GameState => GameState.Playing;
        private PlayingUI _playingUI;
        private const int WinScore = 5;

        public PlayingState(GameStateManager gsm, PlayingUI playingUI) : base(gsm)
        {
            _playingUI = playingUI;
        }

        public override void Enter()
        {
            _playingUI.gameObject.SetActive(true);

            var arena = GameStateManager.Context.ArenaManager.CurrentArena;
            arena.OnScoreChanged += HandleScoreChanged;
            arena.StartGame();
            arena.ballPrefab.LaunchBall();
        }

        public override void Tick()
        {
            if (InputManager.Instance.pauseAction.action.WasPressedThisFrame())
                GameStateManager.PushState(new PauseState(GameStateManager, UIManager.Instance.pauseUIPrefab));
        }

        public override void Exit()
        {
            _playingUI.gameObject.SetActive(false);
            var arena = GameStateManager.Context.ArenaManager.CurrentArena;
            arena.ResetGame();
            arena.OnScoreChanged -= HandleScoreChanged;
        }

        private void HandleScoreChanged(int p1, int p2)
        {
            if (p1 >= WinScore)
                GameStateManager.PushState(new WinState(GameStateManager, UIManager.Instance.winUIPrefab, 1));
            else if (p2 >= WinScore)
                GameStateManager.PushState(new WinState(GameStateManager, UIManager.Instance.winUIPrefab, 2));
            else
                GameStateManager.PopState();
        }
    }
    #endregion

    #region PauseState
    public class PauseState : State
    {
        public override GameState GameState => GameState.Paused;
        private PauseUI _pauseUI;

        public PauseState(GameStateManager gsm, PauseUI pauseUI) : base(gsm) => _pauseUI = pauseUI;

        public override void Enter()
        {
            _pauseUI.gameObject.SetActive(true);
            _pauseUI.OnResumeClicked += Resume;
            _pauseUI.OnExitToMenuClicked += ExitToMenu;
        }

        public override void Tick()
        {
            if (InputManager.Instance.pauseAction.action.WasPressedThisFrame())
                Resume();
        }

        public override void Exit()
        {
            _pauseUI.gameObject.SetActive(false);
            _pauseUI.OnResumeClicked -= Resume;
            _pauseUI.OnExitToMenuClicked -= ExitToMenu;
        }

        private void Resume() => GameStateManager.PopState();

        private void ExitToMenu()
        {
            GameStateManager.Context.ArenaManager.RemoveCurrentArena();
            GameStateManager.ResetStack();
            GameStateManager.PushState(new MenuState(GameStateManager, UIManager.Instance.menuUIPrefab));
        }
    }
    #endregion

    #region WinState
    public class WinState : State
    {
        public override GameState GameState => GameState.Win;
        private WinUI _winUI;
        private int _winner;
        private float _timer;
        private const float Delay = 3f;

        public WinState(GameStateManager gsm, WinUI winUI, int winner) : base(gsm)
        {
            _winUI = winUI;
            _winner = winner;
        }

        public override void Enter()
        {
            _winUI.UpdateWinText(_winner);
            _winUI.gameObject.SetActive(true);
            _timer = 0f;
        }

        public override void Tick()
        {
            _timer += Time.deltaTime;
            if (_timer >= Delay)
            {
                GameStateManager.ResetStack();
                GameStateManager.PushState(new MenuState(GameStateManager, UIManager.Instance.menuUIPrefab));
            }
        }

        public override void Exit() { }
    }
    #endregion
}
