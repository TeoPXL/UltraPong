using System;
using UnityEngine;

namespace Objects
{
    public class Arena : MonoBehaviour
    {
        public Ball ballPrefab;
        public Player playerOnePrefab;
        public Player playerTwoPrefab;
        public PowerUpSpawner powerUpSpawner;

        // Reference the goal objects in the scene or via prefabs
        public Goal playerOneGoal; // the goal player 2 is scoring in
        public Goal playerTwoGoal; // the goal player 1 is scoring in

        private int _playerOneScore = 0;
        private int _playerTwoScore = 0;

        // Event for score changes
        public event Action<int, int> OnScoreChanged;

        void Start()
        {
            // Subscribe to goals
            if (playerOneGoal != null)
                playerOneGoal.OnGoalScored += HandleGoalScored;

            if (playerTwoGoal != null)
                playerTwoGoal.OnGoalScored += HandleGoalScored;
        }

        void HandleGoalScored(int scoringPlayer)
        {
            if (scoringPlayer == 1)
                _playerOneScore++;
            else if (scoringPlayer == 2)
                _playerTwoScore++;

            Debug.Log($"Score Update: Player 1 = {_playerOneScore}, Player 2 = {_playerTwoScore}");
            
            // Invoke score changed event
            OnScoreChanged?.Invoke(_playerOneScore, _playerTwoScore);

            // Optionally reset ball position
            ballPrefab.ResetBall();
        }

        public void ResetGame()
        {
            _playerOneScore = 0;
            _playerTwoScore = 0;
            OnScoreChanged?.Invoke(_playerOneScore, _playerTwoScore);

            ballPrefab.SetAutoLaunch(false); // freeze in idle
            ballPrefab.ResetBall();

            playerOnePrefab.Reset();
            playerTwoPrefab.Reset();
        }

        public void StartGame()
        {
            //ballPrefab.Play();
            //playerOnePrefab.Play();
            //playerTwoPrefab.Play();
            //powerUpSpawner.Play();
        }
    }
}