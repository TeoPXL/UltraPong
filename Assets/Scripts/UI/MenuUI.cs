using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MenuUI : MonoBehaviour
    {
        // Assign these in the Inspector (buttons inside your Menu prefab)
        public Button startButton;
        public Button quitButton;
        public Image arena1Button;
        public Image arena2Button;
        public Image arena3Button;
        public Toggle toggleAI;

        // Events the state code subscribes to
        public event Action OnStartClicked;
        public event Action OnQuitClicked;
        public event Action<int> OnArenaSelected; // index of selected arena

        private Image[] arenaButtons;
        public event Action<bool> OnToggleAIChanged; // true if AI enabled

        private void Awake()
        {
            if (startButton != null) startButton.onClick.AddListener(() => OnStartClicked?.Invoke());
            if (quitButton != null) quitButton.onClick.AddListener(() => OnQuitClicked?.Invoke());

            // Store arena buttons in an array for easier handling
            arenaButtons = new Image[] { arena1Button, arena2Button, arena3Button };

            // Add click listeners to arena buttons
            for (int i = 0; i < arenaButtons.Length; i++)
            {
                int index = i; // Capture index for the closure
                if (arenaButtons[i] != null)
                {
                    Button btn = arenaButtons[i].GetComponent<Button>();
                    if (btn != null)
                    {
                        btn.onClick.AddListener(() => SelectArena(index));
                    }
                }
            }
            
            if (toggleAI != null)
            {
                toggleAI.onValueChanged.AddListener(value => OnToggleAIChanged?.Invoke(value));
            }
        }

        private void SelectArena(int selectedIndex)
        {
            for (int i = 0; i < arenaButtons.Length; i++)
            {
                if (arenaButtons[i] == null) continue;

                Outline outline = arenaButtons[i].GetComponent<Outline>();
                if (outline != null)
                {
                    outline.enabled = (i == selectedIndex); // Enable only for clicked button
                }
            }

            OnArenaSelected?.Invoke(selectedIndex); // Fire event
        }
    }
}
