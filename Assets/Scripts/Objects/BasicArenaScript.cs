using System;
using System.Collections;
using Shared;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Objects
{
    public class BasicArenaScript : MonoBehaviour
    {
        public byte height = 8;
        public byte width = 16;
        public GameObject wallPrefab;
        public GameObject goalPrefab;
        public GameObject ballPrefab;
        public GameObject playerPrefab;
        public float ballDiameter = 0.3f;
        private Ball _ball;
        private Player[] _players = new Player[2];

        // Scores: index 0 = player1, index 1 = player2
        private readonly int[] _scores = new int[2];

        // Event: (playerNumber, newScore) - playerNumber is 1 or 2
        public event Action<int, int> OnScoreChanged;


        public void SpawnObjects() // callen bij start idle
        {
            SpawnWalls();
            SpawnGoals(); // spawn goals first so their Goal components are present
            SpawnPlayers();
            SpawnBall();

            // reset scores
            _scores[0] = 0;
            _scores[1] = 0;

            // notify UI
            OnScoreChanged?.Invoke(_scores[0], _scores[1]);
        }

        public void StartGame() // callen bij start play
        {
            if (_ball == null)
            {
                SpawnBall();
            }

            _ball.speed = 4;
            _ball.Init();
            foreach (Player player in _players)
            {
                if (player != null) player.speed = 2;
            }
        }

        void SpawnWalls()
        {
            GameObject northWall = Instantiate(wallPrefab);
            northWall.transform.position = new Vector3(0f, height / 2f, 0f);
            northWall.transform.localScale = new Vector3(width, 0.1f, 1f);

            GameObject southWall = Instantiate(wallPrefab);
            southWall.transform.position = new Vector3(0f, -height / 2f, 0f);
            southWall.transform.localScale = new Vector3(width, 0.1f, 1f);
        }

        void SpawnGoals()
        {
            float goalThickness = 0.1f;

            // West / left goal -> scores player 2
            GameObject westGoal = Instantiate(goalPrefab);
            westGoal.transform.position = new Vector3(-width / 2f, 0f, 0f);
            westGoal.transform.localScale = new Vector3(goalThickness, height, 1f);
            // IMPORTANT FIX: set arena + scoringPlayer on ALL Goal components in the instantiated hierarchy
            AssignGoalInfoToAllInHierarchy(westGoal, 2);

            // East / right goal -> scores player 1
            GameObject eastGoal = Instantiate(goalPrefab);
            eastGoal.transform.position = new Vector3(width / 2f, 0f, 0f);
            eastGoal.transform.localScale = new Vector3(goalThickness, height, 1f);
            AssignGoalInfoToAllInHierarchy(eastGoal, 1);
        }

        // New helper: ensures every Goal component under the instantiated goal gets configured.
        void AssignGoalInfoToAllInHierarchy(GameObject goalRoot, int scoringPlayer)
        {
            var goalComponents = goalRoot.GetComponentsInChildren<Goal>(true);
            if (goalComponents == null || goalComponents.Length == 0)
            {
                // If none exist in the hierarchy, add one to root just in case.
                var rootGoal = goalRoot.GetComponent<Goal>() ?? goalRoot.AddComponent<Goal>();
                rootGoal.arena = this;
                rootGoal.scoringPlayer = scoringPlayer;
            }
            else
            {
                foreach (var g in goalComponents)
                {
                    g.arena = this;
                    g.scoringPlayer = scoringPlayer;
                }
            }
        }

        void SpawnBall()
        {
            if (_ball != null)
            {
                Destroy(_ball.gameObject);
            }

            GameObject ball = Instantiate(ballPrefab);
            ball.transform.position = Vector3.zero;
            ball.GetComponent<Ball>().ChangeDiameter(ballDiameter);
            _ball = ball.GetComponent<Ball>();
        }

        void SpawnPlayers()
        {
            Player player1 = InstantiatePlayerAtPosition(new Vector3(-7f, 0f, 0f));
            SetPlayerMovement(player1, InputManager.Instance.playerOneUp, InputManager.Instance.playerOneDown);
            _players[0] = player1;

            Player player2 = InstantiatePlayerAtPosition(new Vector3(7f, 0f, 0f));
            SetPlayerMovement(player2, InputManager.Instance.playerTwoUp, InputManager.Instance.playerTwoDown);
            _players[1] = player2;
        }

        private Player InstantiatePlayerAtPosition(Vector3 position)
        {
            GameObject player = Instantiate(playerPrefab);
            player.transform.position = position;
            Player pScript = player.GetComponent<Player>();
            pScript.arenaHeight = height;
            pScript.startPosition = position;
            return pScript;
        }

        private void SetPlayerMovement(Player player, InputActionReference upAction, InputActionReference downAction)
        {
            if (player == null) return;

            player.up = upAction;
            player.down = downAction;
        }

        // Called by a Goal when the ball enters. playerNumber is 1 or 2.
        public void ScoreGoal(int playerNumber)
        {
            if (playerNumber < 1 || playerNumber > 2) return;
            _scores[playerNumber - 1]++;

            // destroy old ball and respawn
            if (_ball != null)
            {
                Destroy(_ball.gameObject);
                _ball = null;
            }

            SpawnBall();
            ResetPlayers();

            // notify with both scores
            OnScoreChanged?.Invoke(_scores[0], _scores[1]);
        }

        void ResetPlayers()
        {
            foreach (Player player in _players)
            {
                if (player != null) player.ResetPosition();
            }
        }

        public void ResetArena()
        {
            // Destroy all children objects in the arena (walls, goals, players, ball)
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            // Also destroy the ball if it’s separately tracked
            if (_ball != null)
            {
                Destroy(_ball.gameObject);
                _ball = null;
            }

            // Clear players array
            if (_players != null)
            {
                for (int i = 0; i < _players.Length; i++)
                {
                    if (_players[i] != null)
                    {
                        Destroy(_players[i].gameObject);
                    }
                }

                _players = null;
            }

            // Reset scores
            _scores[0] = 0;
            _scores[1] = 0;

            // Notify UI about score reset
            OnScoreChanged?.Invoke(_scores[0], _scores[1]);
        }
    }
}