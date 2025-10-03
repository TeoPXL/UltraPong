using UnityEngine;
using TMPro;
using state;

namespace UI
{
    public class IdleUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText; // Assign in Inspector

        public void Awake()
        {
            scoreText.text = "0 – 0";
        }

        public void UpdateScoreText(int scorePlayerOne, int scorePlayerTwo)
        {
            scoreText.text = $"{scorePlayerOne} – {scorePlayerTwo}";
        }
    }
}