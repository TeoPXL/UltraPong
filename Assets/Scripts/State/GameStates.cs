using UI;
using UnityEngine;

namespace state
{
    public abstract class State
    {
        public abstract GameState GameState { get; }
        public abstract void Enter();
        public abstract void Tick();
        public abstract void Exit();
    }

    public class MenuState : State
    {
        public override GameState GameState => GameState.Menu;

        private MenuUI prefab;
        private MenuUI instance;

        public MenuState(MenuUI prefab)
        {
            this.prefab = prefab;
        }

        public override void Enter()
        {
            instance = GameStateManager.Instance.SpawnUI(prefab);
            instance.OnStartClicked += HandleStart;
            instance.OnQuitClicked += HandleQuit;
        }

        public override void Tick()
        {
        }

        public override void Exit()
        {
            if (instance != null)
            {
                instance.OnStartClicked -= HandleStart;
                instance.OnQuitClicked -= HandleQuit;
                GameStateManager.Instance.DestroyUI(instance);
                instance = null;
            }
        }

        private void HandleStart()
        {
            GameStateManager.Instance.ChangeState(
                new IdleState(GameStateManager.Instance.idleUIPrefab)
            );
        }

        private void HandleQuit()
        {
            Application.Quit();
        }
    }


    public class IdleState : State
    {
        private IdleUI prefab;
        private IdleUI instance;
        public override GameState GameState => GameState.Idle;

        public IdleState(IdleUI prefab)
        {
            this.prefab = prefab;
        }

        public override void Enter()
        {
            instance = GameStateManager.Instance.SpawnUI(prefab);
        }

        public override void Tick()
        {
        }

        public override void Exit()
        {
            if (instance != null)
            {
                GameStateManager.Instance.DestroyUI(instance);
                instance = null;
            }
        }

        public void Tick(float deltaTime)
        {
            // Do we want to allow pausing while idle? Then keep this
            if (InputUtils.WasPausePressedThisFrame())
            {
                GameStateManager.Instance.PushState(new PauseState(GameStateManager.Instance.pauseUIPrefab));
            }
        }
    }

    public class PlayingState : State
    {
        private PlayingUI playingUIPrefab;
        private PlayingUI playingUIInstance;
        public override GameState GameState => GameState.Playing;

        public PlayingState(PlayingUI playingUIPrefab)
        {
            this.playingUIPrefab = playingUIPrefab;
        }

        public override void Enter()
        {
            playingUIInstance = GameStateManager.Instance.SpawnUI(playingUIPrefab);
            // Add more UI in the same way

            Debug.Log("Playing state");
        }

        public override void Exit()
        {
            GameStateManager.Instance.DestroyUI(playingUIInstance);
            playingUIInstance = null;
        }

        public override void Tick()
        {
            if (InputUtils.WasPausePressedThisFrame())
            {
                GameStateManager.Instance.PushState(new PauseState(GameStateManager.Instance.pauseUIPrefab));
            }
        }
    }

    public class PauseState : State
    {
        private PauseUI prefab;
        private PauseUI instance;
        public override GameState GameState => GameState.Paused;

        public PauseState(PauseUI prefab)
        {
            this.prefab = prefab;
        }

        public override void Enter()
        {
            instance = GameStateManager.Instance.SpawnUI(prefab);
            instance.OnResumeClicked += Resume;
            instance.OnExitToMenuClicked += ExitToMenu;
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
            if (instance != null)
            {
                instance.OnResumeClicked -= Resume;
                instance.OnExitToMenuClicked -= ExitToMenu;

                GameStateManager.Instance.DestroyUI(instance);
                instance = null;
            }
        }

        public void ShowUI()
        {
            instance?.gameObject.SetActive(true);
        }

        private void Resume()
        {
            GameStateManager.Instance.PopState();
        }

        private void ExitToMenu()
        {
            Debug.Log("Exiting to menu");
            GameStateManager.Instance.ClearAndChangeState(new MenuState(GameStateManager.Instance.menuUIPrefab));
        }
    }


    public class WinState : State
    {

        public override GameState GameState => GameState.Win;

        public override void Enter()
        {
            Debug.Log($"Enter win state");
        }

        public override void Tick()
        {
        }

        public override void Exit()
        {
            Debug.Log("Exiting Win State");
            Debug.Log("Exiting to menu");
            GameStateManager.Instance.ClearAndChangeState(new MenuState(GameStateManager.Instance.menuUIPrefab));
        }

        public void Tick(float deltaTime)
        {
            // Probably count the seconds here
        }

    }

}