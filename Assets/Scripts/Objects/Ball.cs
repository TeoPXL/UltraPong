using Shared;
using System.Collections;
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

        [Header("Sounds")]
        public AudioClip[] bounceSounds;

        [Header("Camera Shake")]
        public SimpleCameraShake cameraShake;
        public float maxShakeMagnitude = 0.5f;
        public float shakeDuration = 0.2f;

        private Rigidbody2D _body;
        private Vector2 _velocity;
        private AudioSource _audioSource;

        private Player _lastPlayer; // Track last player to hit the ball
        private SpriteRenderer _spriteRenderer; // For changing ball color

        public Rigidbody2D Body
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
            
            // Ensure AudioSource exists
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
                _audioSource = gameObject.AddComponent<AudioSource>();
            
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            transform.position = Vector3.zero;
            transform.GetChild(0).localScale = new Vector3(diameter, diameter, 1);
            GetComponent<CircleCollider2D>().radius = diameter / 2;

            if (cameraShake == null)
            {
                Camera mainCam = Camera.main;
                if (mainCam != null)
                    cameraShake = mainCam.GetComponent<SimpleCameraShake>();

                if (cameraShake == null)
                    Debug.LogWarning("No SimpleCameraShake found on the main camera!");
            }

            if (AutoLaunch)
                LaunchBall();
        }

        public void LaunchBall()
        {
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
            _lastPlayer = null;
            if (_spriteRenderer != null)
                _spriteRenderer.color = Color.white;

            if (AutoLaunch)
                LaunchBall();
            else
                FreezeBall();
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

            // Factor in player's vertical velocity if hitting a player
            Player player = collision.collider.GetComponent<Player>();
            if (player != null)
            {
                reflected.y += player.VerticalVelocity * 0.2f;

                // Track last player
                _lastPlayer = player;

                // Change ball color to match player
                if (_spriteRenderer != null)
                    _spriteRenderer.color = player.GetComponentInChildren<SpriteRenderer>().color;
            }

            Body.linearVelocity = reflected.normalized * speed;
            _velocity = Body.linearVelocity;

            // Play bounce sound safely
            PlayBounceSound();

            // Camera shake
            if (cameraShake != null)
            {
                float impactFactor = Mathf.Abs(Vector2.Dot(_velocity.normalized, normal));
                float shakeAmount = maxShakeMagnitude * impactFactor;
                StartCoroutine(cameraShake.Shake(shakeDuration, shakeAmount));
            }

            // Check if hit a powerup
            IPowerUp powerUp = collision.collider.GetComponent<IPowerUp>();
            if (powerUp != null)
            {
                powerUp.OnPickup(_lastPlayer != null ? _lastPlayer.gameObject : null);

                // Example effect: temporary speed boost
                StartCoroutine(TemporarySpeedChange(2f, 1.5f)); // 2 seconds, 1.5x speed
            }
        }

        // Helper method to safely play bounce sounds
        private void PlayBounceSound()
        {
            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
                if (_audioSource == null)
                    _audioSource = gameObject.AddComponent<AudioSource>();
            }

            if (bounceSounds != null && bounceSounds.Length > 0)
            {
                AudioClip clip = bounceSounds[Random.Range(0, bounceSounds.Length)];
                _audioSource.PlayOneShot(clip);
            }
        }

        // Example coroutine for temporary speed change
        private IEnumerator TemporarySpeedChange(float duration, float multiplier)
        {
            float originalSpeed = speed;
            speed *= multiplier;
            Body.linearVelocity = Body.linearVelocity.normalized * speed;

            yield return new WaitForSeconds(duration);

            speed = originalSpeed;
            Body.linearVelocity = Body.linearVelocity.normalized * speed;
        }
    }
}