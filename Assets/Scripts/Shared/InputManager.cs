using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Shared
{
    public class InputManager : Singleton<InputManager>
    {
        public InputActionReference playerOneUp;
        public event UnityAction OnPlayerOneUp;
        public InputActionReference playerOneDown;
        public event UnityAction OnPlayerOneDown;
        public InputActionReference playerTwoUp;
        public event UnityAction OnPlayerTwoUp;
        public InputActionReference playerTwoDown;
        public event UnityAction OnPlayerTwoDown;

        public InputActionReference pauseAction;
        public event UnityAction OnPause;

        private void Update()
        {
            if (playerOneUp && playerOneUp.action.WasPressedThisFrame()) OnPlayerOneUp?.Invoke();
            if (playerOneDown && playerOneDown.action.WasPressedThisFrame()) OnPlayerOneDown?.Invoke();
            if (playerTwoUp && playerTwoUp.action.WasPressedThisFrame()) OnPlayerTwoUp?.Invoke();
            if (playerTwoDown && playerTwoDown.action.WasPressedThisFrame()) OnPlayerTwoDown?.Invoke();
            if (pauseAction && pauseAction.action.WasPressedThisFrame()) OnPause?.Invoke();
        }
    }
}