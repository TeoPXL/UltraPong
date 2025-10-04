using Shared;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Objects
{
    public class Ball : PausableBehaviour
    {
        [Header("Ball Settings")]
        public float speed = 0.5f;
        public float diameter = 3f;
        public float launchAngleRange = 60f;

        [FormerlySerializedAs("AutoLaunch")] [Tooltip("If true, ball launches immediately regardless of state.")]
        public bool autoLaunch = true;

        [Header("Sounds")]
        public AudioClip[] bounceSounds;

        [Header("Camera Shake")]
        public SimpleCameraShake cameraShake;
        public float maxShakeMagnitude = 0.5f;
        public float shakeDuration = 0.2f;

        private Rigidbody2D _body;
        private Vector2 _velocity;
        private AudioSource _audioSource;
        private Player _lastPlayer;
        
        [Header("Visuals")]
        public Color originalColor = Color.white; // Set this in the inspector
        private SpriteRenderer _spriteRenderer;
        
        private bool _isFrozen;
        private bool _isPaused;

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

            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
                _audioSource = gameObject.AddComponent<AudioSource>();

            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            
            // Apply the original color at the start
            if (_spriteRenderer != null)
                _spriteRenderer.color = originalColor;

            // Set initial scale and collider
            transform.position = Vector3.zero;
            transform.GetChild(0).localScale = new Vector3(diameter, diameter, 1);
            GetComponent<CircleCollider2D>().radius = diameter / 2;

            // Camera shake reference
            if (cameraShake == null)
            {
                Camera mainCam = Camera.main;
                if (mainCam != null)
                    cameraShake = mainCam.GetComponent<SimpleCameraShake>();

                if (cameraShake == null)
                    Debug.LogWarning("No SimpleCameraShake found on the main camera!");
            }

            if (autoLaunch)
                LaunchBall();
        }

        private void FixedUpdate()
        {
            //Debug.Log("fixed update");
            // Continuously enforce constant speed when not paused/frozen
            if (!_isPaused && !_isFrozen && Body.linearVelocity.sqrMagnitude < 0.01f && Body.position != Vector2.zero)
            {
                Body.linearVelocity = - Body.position.normalized * speed;
                _velocity = Body.linearVelocity;
            }
        }

        public void LaunchBall()
        {
            _isFrozen = false;
            float angle = Random.Range(-launchAngleRange, launchAngleRange) * Mathf.Deg2Rad;
            int direction = Random.value < 0.5f ? 1 : -1;
            Vector2 dir = new Vector2(Mathf.Cos(angle) * direction, Mathf.Sin(angle));
            Body.linearVelocity = dir.normalized * speed;
            _velocity = Body.linearVelocity;
        }

        public void FreezeBall()
        {
            _isFrozen = true;
            _velocity = Body.linearVelocity;
            Body.linearVelocity = Vector2.zero;
        }

        public void ResetBall()
        {
            transform.position = Vector3.zero;
            _lastPlayer = null;

            if (_spriteRenderer != null)
                _spriteRenderer.color = originalColor;

            if (autoLaunch)
                LaunchBall();
            else
                FreezeBall();
        }

        public override void OnPause()
        {
            base.OnPause();
            _isPaused = true;
            _velocity = Body.linearVelocity;
            Body.linearVelocity = Vector2.zero;
        }

        public override void OnResume()
        {
            base.OnResume();
            _isPaused = false;
            Body.linearVelocity = _velocity;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Vector2 normal = collision.contacts[0].normal;
            Vector2 reflected = Vector2.Reflect(_velocity, normal);

            Player player = collision.collider.GetComponent<Player>();
            if (player != null)
            {
                // Apply some vertical influence
                reflected.y += player.VerticalVelocity * 0.2f;

                _lastPlayer = player;

                if (_spriteRenderer != null)
                    _spriteRenderer.color = player.GetComponentInChildren<SpriteRenderer>().color;
            }
            else if (collision.gameObject.name == "WallRectangle")
            {
                // Prevent near-horizontal bounces off walls
                float minVerticalComponent = 0.3f;
                
                if (Mathf.Abs(reflected.y) < minVerticalComponent)
                {
                    float sign = reflected.y >= 0 ? 1f : -1f;
                    reflected.y = minVerticalComponent * sign;
                }
            }

            Body.linearVelocity = reflected.normalized * speed;
            _velocity = Body.linearVelocity;

            PlayBounceSound();
            ShakeCamera(normal);
        }

        private void PlayBounceSound()
        {
            if (_audioSource == null)
                _audioSource = gameObject.AddComponent<AudioSource>();

            if (bounceSounds != null && bounceSounds.Length > 0)
            {
                AudioClip clip = bounceSounds[Random.Range(0, bounceSounds.Length)];
                _audioSource.PlayOneShot(clip);
            }
        }

        private void ShakeCamera(Vector2 collisionNormal)
        {
            if (cameraShake != null)
            {
                float impactFactor = Mathf.Abs(Vector2.Dot(_velocity.normalized, collisionNormal));
                float shakeAmount = maxShakeMagnitude * impactFactor;
                StartCoroutine(cameraShake.Shake(shakeDuration, shakeAmount));
            }
        }
    }
}