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

        private MenuUI _prefab;
        private MenuUI _instance;

        public MenuState(GameStateManager gameStateManager, MenuUI prefab) : base(gameStateManager)
        {
            _prefab = prefab;
        }

        public override void Enter()
        {
            _instance = UIManager.Instance.SpawnUI(_prefab);
            _instance.OnStartClicked += HandleStart;
            _instance.OnQuitClicked += HandleQuit;
        }

        public override void Tick()
        {
        }

        public override void Exit()
        {
            if (_instance != null)
            {
                _instance.OnStartClicked -= HandleStart;
                _instance.OnQuitClicked -= HandleQuit;
                UIManager.Instance.DestroyUI(_instance);
                _instance = null;
            }
        }

        private void HandleStart()
        {
            GameStateManager.PushState(new IdleState(GameStateManager, UIManager.Instance.idleUIPrefab));
        }

        private void HandleQuit()
        {
            Application.Quit();
        }
    }


    public class IdleState : State
    {
        private IdleUI _prefab;
        private IdleUI _instance;
        public override GameState GameState => GameState.Idle;

        public IdleState(GameStateManager gameStateManager, IdleUI prefab) : base(gameStateManager)
        {
            _prefab = prefab;
        }

        public override void Enter()
        {
            _instance = UIManager.Instance.SpawnUI(_prefab);
        }

        public override void Exit()
        {
            if (_instance != null)
            {
                UIManager.Instance.DestroyUI(_instance);
                _instance = null;
            }
        }

        public override void Tick()
        {
            // Do we want to allow pausing while idle? Then keep this
            if (InputUtils.WasPausePressedThisFrame())
            {
                GameStateManager.PushState(new PauseState(GameStateManager, UIManager.Instance.pauseUIPrefab));
            }
        }
    }

    public class PlayingState : State
    {
        private PlayingUI _prefab;
        private PlayingUI _instance;
        public override GameState GameState => GameState.Playing;

        public PlayingState(GameStateManager gameStateManager, PlayingUI prefab) : base(gameStateManager)
        {
            _prefab = prefab;
        }

        public override void Enter()
        {
            _instance = UIManager.Instance.SpawnUI(_prefab);
            // Add more UI in the same way

            // state.GameStateManager.Instance.OnScoreChanged += RePlay;


            Debug.Log("Playing state");
        }

        public void RePlay(int scorePlayerOne, int scorePlayerTwo)
        {
            //If max score has not been reached
            if (System.Math.Max(scorePlayerOne, scorePlayerTwo) <= 5)
            {
                GameStateManager.PopState();
            }
            else
            {
                GameStateManager.PushState(new WinState(GameStateManager, UIManager.Instance.winUIPrefab));
            }
        }

        public override void Exit()
        {
            UIManager.Instance.DestroyUI(_instance);
            _instance = null;
        }

        public override void Tick()
        {
            if (InputUtils.WasPausePressedThisFrame())
            {
                GameStateManager.PushState(
                    new PauseState(GameStateManager, UIManager.Instance.pauseUIPrefab));
            }
        }
    }

    public class PauseState : State
    {
        private PauseUI _prefab;
        private PauseUI _instance;
        public override GameState GameState => GameState.Paused;

        public PauseState(GameStateManager gameStateManager, PauseUI prefab) : base(gameStateManager)
        {
            _prefab = prefab;
        }

        public override void Enter()
        {
            _instance = UIManager.Instance.SpawnUI(_prefab);
            _instance.OnResumeClicked += Resume;
            _instance.OnExitToMenuClicked += ExitToMenu;
        }

        public override void Tick()
        {
            if (InputUtils.WasPausePressedThisFrame())
            {
                Resume();
            }
        }

        public override void Exit()
        {
            if (_instance != null)
            {
                _instance.OnResumeClicked -= Resume;
                _instance.OnExitToMenuClicked -= ExitToMenu;

                UIManager.Instance.DestroyUI(_instance);
                _instance = null;
            }
        }

        public void ShowUI()
        {
            _instance?.gameObject.SetActive(true);
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
        private WinUI _prefab;
        private WinUI _instance;

        public override GameState GameState => GameState.Win;

        public WinState(GameStateManager gameStateManager, WinUI prefab) : base(gameStateManager)
        {
            _prefab = prefab;
        }

        public override void Enter()
        {
            Debug.Log($"Enter win state");
            _instance = UIManager.Instance.SpawnUI(_prefab);
        }

        public override void Tick()
        {
        }

        public override void Exit()
        {
            Debug.Log("Exiting Win State");
            if (_instance != null)
            {
                UIManager.Instance.DestroyUI(_instance);
                _instance = null;
            }

            Debug.Log("Exiting to menu");
            GameStateManager.ResetStack();
            GameStateManager.PushState(new MenuState(GameStateManager, UIManager.Instance.menuUIPrefab));
        }
    }
}