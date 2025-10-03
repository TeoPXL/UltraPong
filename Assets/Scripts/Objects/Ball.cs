using UnityEngine;

public class Ball : MonoBehaviour
{
    public float speed;
    public float diameter = 3f;
    private Rigidbody2D body;
    private Vector2 velocity;
    public Vector2 randomnessRange = new Vector2(-1f, 1f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }

    void Init()
    {
        transform.GetChild(0).localScale = new Vector3(diameter, diameter, 1);
        GetComponent<CircleCollider2D>().radius = diameter / 2;
        body = GetComponent<Rigidbody2D>();
        body.linearVelocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * speed;
        velocity = body.linearVelocity;        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 normal = collision.contacts[0].normal;
        Vector2 reflected = Vector2.Reflect(velocity, normal);
        Vector2 randomBoost = new Vector2(Random.Range(randomnessRange.x, randomnessRange.y), Random.Range(randomnessRange.x, randomnessRange.y));
        body.linearVelocity = (reflected + randomBoost).normalized * speed;
        velocity = body.linearVelocity;
        Debug.Log(velocity);
    }

    void OnPause()
    {
        velocity = body.linearVelocity;
        body.linearVelocity = new Vector2(0, 0);
    }

    void OnUnPause()
    {
        body.linearVelocity = velocity;
    }

    void OnExitPlay()
    {
        Destroy(gameObject);
    }
}
