using System;
using UnityEngine;

namespace Objects
{
    public class Goal : MonoBehaviour
    {
        public int scoringPlayer; 

        // Event triggered when a goal is scored
        public event Action<int> OnGoalScored;

        void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log($"Goal trigger entered by {other.name}");
            var ball = other.GetComponent<Ball>();
            if (ball != null)
            {
                Debug.Log($"Scoring goal for player {scoringPlayer}");
                OnGoalScored?.Invoke(scoringPlayer);
            }
        }
    }
}