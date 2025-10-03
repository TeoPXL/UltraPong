using UnityEngine;

namespace Objects
{
    public class Ball : MonoBehaviour
    {
        public float speed;
        public float diameter = 3f;
        private Rigidbody2D body;
        private Vector2 velocity;
        public Vector2 randomnessRange = new Vector2(-1f, 1f);
        public Player lastHitter;

        public void ChangeDiameter(float newDiameter)
        {
            diameter = newDiameter;
            transform.GetChild(0).localScale = new Vector3(diameter, diameter, 1);
            GetComponent<CircleCollider2D>().radius = diameter / 2;
        }

        public void Init() // start de beweging
        {
            transform.GetChild(0).localScale = new Vector3(diameter, diameter, 1);
            GetComponent<CircleCollider2D>().radius = diameter / 2;
            body = GetComponent<Rigidbody2D>();
            body.linearVelocity = new Vector2(10f * Random.Range(-1f, 1f), 0.1f * Random.Range(-1f, 1f)).normalized * speed;
            velocity = body.linearVelocity;        
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            Vector2 normal = collision.contacts[0].normal;
            Vector2 reflected = Vector2.Reflect(velocity, normal);
            reflected = new Vector2(reflected.x * Random.Range(0.9f, 1.1f), reflected.y * Random.Range(0.9f, 1.1f));
            body.linearVelocity = reflected.normalized * speed;
            velocity = body.linearVelocity;
            Player hitter = collision.gameObject.GetComponent<Player>();
            if (hitter != null)
            {
                lastHitter = hitter;
            }
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
}
