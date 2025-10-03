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
        public float launchAngleRange = 60f;
        private bool _autoLaunch = true; // new flag

        protected override void Awake()
        {
            base.Awake();
            _body = GetComponent<Rigidbody2D>();

            transform.position = Vector3.zero;
            transform.GetChild(0).localScale = new Vector3(diameter, diameter, 1);
            GetComponent<CircleCollider2D>().radius = diameter / 2;

            if (_autoLaunch)
                LaunchBall();
        }

        public void SetAutoLaunch(bool autoLaunch)
        {
            _autoLaunch = autoLaunch;
            if (!_autoLaunch)
                _body.linearVelocity = Vector2.zero;
        }

        public void LaunchBall()
        {
            float angle = Random.Range(-launchAngleRange, launchAngleRange) * Mathf.Deg2Rad;
            int direction = Random.value < 0.5f ? 1 : -1;
            Vector2 dir = new Vector2(Mathf.Cos(angle) * direction, Mathf.Sin(angle));
            _body.linearVelocity = dir.normalized * speed;
            _velocity = _body.linearVelocity;
        }

        public void FreezeBall()
        {
            _velocity = _body.linearVelocity;
            _body.linearVelocity = Vector2.zero;
        }

        public void ResetBall()
        {
            transform.position = Vector3.zero;
            if (_autoLaunch)
                LaunchBall();
            else
                FreezeBall();
        }

        public override void OnPause()
        {
            base.OnPause();
            _velocity = _body.linearVelocity;
            _body.linearVelocity = Vector2.zero;
        }

        public override void OnResume()
        {
            base.OnResume();
            _body.linearVelocity = _velocity;
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            Vector2 normal = collision.contacts[0].normal;
            Vector2 reflected = Vector2.Reflect(_velocity, normal);
            _body.linearVelocity = reflected.normalized * speed;
            _velocity = _body.linearVelocity;
        }
    }

}
