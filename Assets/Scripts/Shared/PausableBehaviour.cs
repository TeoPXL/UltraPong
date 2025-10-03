using state;
using State;
using UnityEngine;
using UnityEngine.Events;

namespace Shared
{
    public abstract class PausableBehaviour : MonoBehaviour
    {
        private UnityAction<GameState> _enterPaused;
        private UnityAction<GameState> _exitedPaused;

        protected virtual void Awake()
        {
            if (GameStateInitializer.Instance == null) return;

            _enterPaused = HandlePausedEntered;
            _exitedPaused = HandlePausedExited;

            GameStateInitializer.Instance.OnStateEntered += _enterPaused;
            GameStateInitializer.Instance.OnStateExited += _exitedPaused;

            // if (GameStateInitializer.Instance.CurrentGameState == GameState.Paused) OnPause();
            // else OnResume();
        }

        protected virtual void OnDestroy()
        {
            if (GameStateInitializer.Instance == null) return;

            if (_enterPaused != null) GameStateInitializer.Instance.OnStateEntered -= _enterPaused;
            if (_exitedPaused != null) GameStateInitializer.Instance.OnStateExited -= _exitedPaused;
        }

        private void HandlePausedEntered(GameState state)
        {
            if (state == GameState.Paused) OnPause();
        }

        private void HandlePausedExited(GameState state)
        {
            if (state == GameState.Paused) OnResume();
        }

        public virtual void OnPause()
        {
            enabled = false;
        }

        public virtual void OnResume()
        {
            enabled = true;
        }
    }
}