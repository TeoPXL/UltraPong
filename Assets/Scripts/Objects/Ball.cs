using Shared;
using UnityEngine;

namespace Objects
{
    public class Ball : PausableBehaviour
    {
        public float speed = 0.5f;
        public float diameter = 3f;
        private Rigidbody2D _body;
        private Vector2 _velocity;
        public Vector2 randomnessRange = new Vector2(-1f, 1f);


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
            _body = GetComponent<Rigidbody2D>();
            _body.linearVelocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * speed;
            _velocity = _body.linearVelocity;
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            Vector2 normal = collision.contacts[0].normal;
            Vector2 reflected = Vector2.Reflect(_velocity, normal);
            Vector2 randomBoost = new Vector2(Random.Range(randomnessRange.x, randomnessRange.y),
                Random.Range(randomnessRange.x, randomnessRange.y));
            _body.linearVelocity = (reflected + randomBoost).normalized * speed;
            _velocity = _body.linearVelocity;
            Debug.Log(_velocity);
        }

        public override void OnPause()
        {
            base.OnPause();
            _velocity = _body.linearVelocity;
            _body.linearVelocity = new Vector2(0, 0);
        }

        public override void OnResume()
        {
            base.OnResume();
            _body.linearVelocity = _velocity;
        }
    }
}