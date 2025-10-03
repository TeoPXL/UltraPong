using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Objects
{
    public class BasicArenaScript : MonoBehaviour
    {
        public byte height = 8;
        public byte width = 16;
        public GameObject wallPrefab;
        public GameObject ballPrefab;
        public GameObject playerPrefab;
        public float ballDiameter = 0.3f;
        private Ball _ball;
        private Player[] _players;
        public InputActionReference up1;
        public InputActionReference down1;
        public InputActionReference up2;
        public InputActionReference down2;


        public void SpawnObjects() // callen bij start idle
        {
            SpawnWalls();
            SpawnBall();
            SpawnPlayers();
        }

        public void StartGame() // callen bij start play
        {
            _ball.speed = 12;
            _ball.Init();
            foreach (Player player in _players)
            {
                player.speed = 2;
            }
        }

        void SpawnWalls()
        {
            GameObject northWall = Instantiate(wallPrefab);
            northWall.transform.localPosition = new Vector3(0, height / 2, 0);
            northWall.transform.localScale = new Vector3(width, 0.1f, 0);
            GameObject southWall = Instantiate(wallPrefab);
            southWall.transform.localPosition = new Vector3(0, -height / 2, 0);
            southWall.transform.localScale = new Vector3(width, 0.1f, 0);
        }

        void SpawnBall()
        {
            GameObject ball = Instantiate(ballPrefab);
            ball.transform.localPosition = Vector3.zero;
            ball.GetComponent<Ball>().ChangeDiameter(ballDiameter);
            _ball = ball.GetComponent<Ball>();
        }

        void SpawnPlayers()
        {
            _players = new Player[2];

            GameObject player1 = Instantiate(playerPrefab);
            player1.transform.position = new Vector3(-7, 0, 0);
            player1.GetComponent<Player>().arenaHeight = height;
            Player p1Script = player1.GetComponent<Player>();
            p1Script.up = up1;
            p1Script.down = down1;
            _players[0] = p1Script;

            GameObject player2 = Instantiate(playerPrefab);
            player2.transform.position = new Vector3(7, 0, 0);
            player2.GetComponent<Player>().arenaHeight = height;
            Player p2Script = player2.GetComponent<Player>();
            p2Script.up = up2;
            p2Script.down = down2;
            _players[1] = p2Script;
        }
    }
}