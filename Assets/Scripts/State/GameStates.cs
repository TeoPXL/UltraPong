using UI;
using UnityEngine;

namespace state
{
    public abstract class State
    {
        protected readonly GameStateManager GameStateManager;

        protected State(GameStateManager gameStateManager)
        {
            this.GameStateManager = gameStateManager;
        }

        public abstract GameState GameState { get; }
        public abstract void Enter();
        public abstract void Tick();
        public abstract void Exit();
    }

    public class MenuState : State
    {
        public override GameState GameState => GameState.Menu;

        private MenuUI _menuUI;

        public MenuState(GameStateManager gameStateManager, MenuUI menuUI) : base(gameStateManager)
        {
            _menuUI = menuUI;
        }

        public override void Enter()
        {
            Debug.Log("Enter menu");
            _menuUI.gameObject.SetActive(true);
            _menuUI.OnStartClicked += HandleStart;
            _menuUI.OnQuitClicked += HandleQuit;
        }

        public override void Tick()
        {
        }

        public override void Exit()
        {
            
            Debug.Log("Exit from menu");
            _menuUI.gameObject.SetActive(false);
            _menuUI.OnStartClicked -= HandleStart;
            _menuUI.OnQuitClicked -= HandleQuit;
        }

        private void HandleStart()
        {
            Debug.Log("Handlestart for menu");
            GameStateManager.PopState();
            GameStateManager.PushState(new IdleState(GameStateManager, UIManager.Instance.idleUIPrefab));
        }

        private void HandleQuit()
        {
            Application.Quit();
        }
    }


    public class IdleState : State
    {
        private readonly IdleUI _idleUI;
        private float _timer;
        private const float Delay = 3f;

        public override GameState GameState => GameState.Idle;

        public IdleState(GameStateManager gameStateManager, IdleUI idleUI) : base(gameStateManager)
        {
            _idleUI = idleUI;
        }

        public override void Enter()
        {
            Debug.Log("Entering Idle state");
            _idleUI.gameObject.SetActive(true);
            _timer = 0f;
        }

        public override void Tick()
        {
            _timer += Time.deltaTime;
            
            if (_timer >= Delay)
            {
                _timer = 0f;
                Play();
            }

            if (InputUtils.WasPausePressedThisFrame())
            {
                GameStateManager.PushState(new PauseState(GameStateManager, UIManager.Instance.pauseUIPrefab));
            }
        }

        public override void Exit()
        {
            _idleUI.gameObject.SetActive(false);
        }

        private void Play()
        {
            Debug.Log("Going to play");
            GameStateManager.PushState(new PlayingState(GameStateManager, UIManager.Instance.playingUIPrefab));
        }
    }


    public class PlayingState : State
    {
        private PlayingUI _playingUI;
        public override GameState GameState => GameState.Playing;

        public PlayingState(GameStateManager gameStateManager, PlayingUI playingUI) : base(gameStateManager)
        {
            _playingUI = playingUI;
        }

        public override void Enter()
        {
            _playingUI.gameObject.SetActive(true);

            Debug.Log("Playing state");
        }

        public override void Tick()
        {
        }

        public override void Exit()
        {
            _playingUI.gameObject.SetActive(false);
        }
    }

    public class PauseState : State
    {
        private PauseUI _pauseUI;
        public override GameState GameState => GameState.Paused;

        public PauseState(GameStateManager gameStateManager, PauseUI pauseUI) : base(gameStateManager)
        {
            _pauseUI = pauseUI;
        }

        public override void Enter()
        {
            _pauseUI.gameObject.SetActive(true);
            _pauseUI.OnResumeClicked += Resume;
            _pauseUI.OnExitToMenuClicked += ExitToMenu;
        }

        public override void Tick()
        {
        }

        public override void Exit()
        {
            _pauseUI.gameObject.SetActive(false);
            _pauseUI.OnResumeClicked -= Resume;
            _pauseUI.OnExitToMenuClicked -= ExitToMenu;
        }

        private void Resume()
        {
            GameStateManager.PopState();
        }

        private void ExitToMenu()
        {
            Debug.Log("Exiting to menu");
            GameStateManager.ResetStack();
            GameStateManager.PushState(new MenuState(GameStateManager, UIManager.Instance.menuUIPrefab));
        }
    }


    public class WinState : State
    {
        private WinUI _winUI;

        public override GameState GameState => GameState.Win;

        public WinState(GameStateManager gameStateManager, WinUI winUI) : base(gameStateManager)
        {
            _winUI = winUI;
        }

        public override void Enter()
        {
            Debug.Log($"Enter win state");
            _winUI.gameObject.SetActive(true);
        }

        public override void Tick()
        {
        }

        public override void Exit()
        {
            Debug.Log("Exiting Win State");
            _winUI.gameObject.SetActive(false);

            Debug.Log("Exiting to menu");
            GameStateManager.ResetStack();
            GameStateManager.PushState(new MenuState(GameStateManager, UIManager.Instance.menuUIPrefab));
        }
    }
}