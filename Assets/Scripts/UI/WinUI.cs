using UnityEngine;
using TMPro;
using state;

namespace UI
{
    public class WinUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI winText; // Assign in Inspector

        public void Awake()
        {
            winText.text = "";
        }

        public void UpdateWinText(int player)
        {
            winText.text = $"PLAYER {player} HAS WON";
        }
    }
}