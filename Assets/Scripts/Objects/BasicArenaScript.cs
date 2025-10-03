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
        private Player[] players;

        public InputActionReference up1;
        public InputActionReference down1;
        public InputActionReference up2;
        public InputActionReference down2;

        public bool isAi = false;

        public virtual void SpawnObjects()
        {
            SpawnWalls();
            SpawnBall();
            SpawnPlayers();
        }

        public void StartGame()
        {
            _ball.speed = 10;
            _ball.Init();

            foreach (Player player in players)
            {
                player.speed = 2;
            }
        }

        private void SpawnWalls()
        {
            GameObject northWall = Instantiate(wallPrefab);
            northWall.transform.localPosition = new Vector3(0, height / 2, 0);
            northWall.transform.localScale = new Vector3(width, 0.1f, 1);

            GameObject southWall = Instantiate(wallPrefab);
            southWall.transform.localPosition = new Vector3(0, -height / 2, 0);
            southWall.transform.localScale = new Vector3(width, 0.1f, 1);
        }

        private void SpawnBall()
        {
            GameObject ball = Instantiate(ballPrefab);
            ball.transform.localPosition = Vector3.zero;

            _ball = ball.GetComponent<Ball>();
            _ball.ChangeDiameter(ballDiameter);
        }

        private void SpawnPlayers()
        {
            players = new Player[2];

            GameObject player1 = Instantiate(playerPrefab);
            player1.transform.position = new Vector3(-7, 0, 0);

            Player p1script = player1.GetComponent<Player>();
            p1script.arenaHeight = height;
            p1script.speed = 5;

            p1script.up = up1;
            p1script.down = down1;

            players[0] = p1script;


            GameObject player2 = Instantiate(playerPrefab);
            player2.transform.position = new Vector3(7, 0, 0);

            if (isAi)
            {
                PlayerAi aiscript = player2.AddComponent<PlayerAi>();
                aiscript.arenaHeight = height;
                aiscript.speed = 3;
                aiscript.targetBall = _ball; 
                players[1] = aiscript;
            }
            else
            {
                Player p2script = player2.GetComponent<Player>();
                p2script.arenaHeight = height;
                p2script.up = up2;
                p2script.down = down2;

                players[1] = p2script;
            }
        }
    }
}
