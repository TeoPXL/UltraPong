using UnityEngine;
using TMPro;
using state;

namespace UI
{
    public class WinUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text winText; // Assign in Inspector

        public void UpdateWinText(int player)
        {
            Debug.Log($"Updating win text to {player}");
            winText.text = $"PLAYER {player} HAS WON";
        }
    }
}