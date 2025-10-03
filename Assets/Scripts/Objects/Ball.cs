using Shared;
using UnityEngine;

namespace Objects
{
    public class Ball : PausableBehaviour
    {
        public float speed = 0.5f;
        public float diameter = 3f;
        public float launchAngleRange = 60f;

        [Tooltip("If true, ball launches immediately regardless of state.")]
        public bool AutoLaunch = true;

        private Rigidbody2D _body;
        private Vector2 _velocity;

        private Rigidbody2D Body
        {
            get
            {
                if (_body == null)
                    _body = GetComponent<Rigidbody2D>();
                return _body;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            _body = GetComponent<Rigidbody2D>();

            transform.position = Vector3.zero;
            transform.GetChild(0).localScale = new Vector3(diameter, diameter, 1);
            GetComponent<CircleCollider2D>().radius = diameter / 2;

            if (AutoLaunch)
                LaunchBall(); // immediate launch if auto-launch is true
        }

        public void LaunchBall()
        {
            Debug.Log("Launching ball");
            float angle = Random.Range(-launchAngleRange, launchAngleRange) * Mathf.Deg2Rad;
            int direction = Random.value < 0.5f ? 1 : -1;
            Vector2 dir = new Vector2(Mathf.Cos(angle) * direction, Mathf.Sin(angle));
            Body.linearVelocity = dir.normalized * speed;
            _velocity = Body.linearVelocity;
        }

        public void FreezeBall()
        {
            _velocity = Body.linearVelocity;
            Body.linearVelocity = Vector2.zero;
        }

        public void ResetBall()
        {
            transform.position = Vector3.zero;

            if (AutoLaunch)
                LaunchBall(); // immediate launch if auto-launch
            else
                FreezeBall(); // stay still
        }

        public override void OnPause()
        {
            base.OnPause();
            _velocity = Body.linearVelocity;
            Body.linearVelocity = Vector2.zero;
        }

        public override void OnResume()
        {
            base.OnResume();
            Body.linearVelocity = _velocity;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Vector2 normal = collision.contacts[0].normal;
            Vector2 reflected = Vector2.Reflect(_velocity, normal);
            Body.linearVelocity = reflected.normalized * speed;
            _velocity = Body.linearVelocity;
        }
    }
}
