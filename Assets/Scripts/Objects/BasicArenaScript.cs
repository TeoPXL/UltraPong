using System.Collections;
using state;
using UnityEngine;
using UnityEngine.InputSystem;

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

    void Start()
    {
        StartCoroutine(initialize());
    }

    IEnumerator initialize()
    {
        SpawnObjects();
        yield return new WaitForSeconds(1);
        StartGame();
    }

    void SpawnObjects() // callen bij start idle
    {
        SpawnWalls();
        SpawnBall();
        SpawnPlayers();
    }

    void StartGame() // callen bij start play
    {
        _ball.speed = 12;
        _ball.Init();
        foreach (Player player in players)
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
        players = new Player[2];

        GameObject player1 = Instantiate(playerPrefab);
        player1.transform.position = new Vector3(-7, 0, 0);
        player1.GetComponent<Player>().arenaHeight = height;
        Player p1script = player1.GetComponent<Player>();
        p1script.up = up1;
        p1script.down = down1;
        players[0] = p1script;

        GameObject player2 = Instantiate(playerPrefab);
        player2.transform.position = new Vector3(7, 0, 0);
        player2.GetComponent<Player>().arenaHeight = height;
        Player p2script = player2.GetComponent<Player>();
        p2script.up = up2;
        p2script.down = down2;
        players[1] = p2script;
    }
}
