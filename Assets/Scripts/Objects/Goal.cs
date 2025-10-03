using UnityEngine;

namespace Objects
{
    public class Goal : MonoBehaviour
    {
        [HideInInspector] public BasicArenaScript arena; // assigned in SpawnGoals
        [HideInInspector] public int scoringPlayer; 

        void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log($"Goal trigger entered by {other.name}");
            // Only consider the ball
            var ball = other.GetComponent<Ball>();
            if (ball != null)
            {
                Debug.Log($"Scoring goal for {scoringPlayer}");
                arena?.ScoreGoal(scoringPlayer);
            }
        }
    }
}