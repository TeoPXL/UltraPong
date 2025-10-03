using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PauseUI : MonoBehaviour
    {
        // Assign these in the Inspector (buttons inside your Pause prefab)
        public Button resumeButton;
        public Button exitToMenuButton;

        // Events the state code subscribes to
        public event Action OnResumeClicked;
        public event Action OnExitToMenuClicked;

        private void Awake()
        {
            if (resumeButton != null) resumeButton.onClick.AddListener(() => OnResumeClicked?.Invoke());
            if (exitToMenuButton != null) exitToMenuButton.onClick.AddListener(() => OnExitToMenuClicked?.Invoke());
        }
    }
}
