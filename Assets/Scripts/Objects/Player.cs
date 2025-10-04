using UnityEngine;
using UnityEngine.InputSystem;

namespace Objects
{
    public class Player : MonoBehaviour
    {
        [Header("Input")]
        public InputActionReference up;
        public InputActionReference down;

        [Header("Movement")]
        public float arenaHeight;
        public float speed = 5f;

        [Header("Appearance")]
        [SerializeField] private Color playerColor = Color.white;

        [Header("AI Settings")]
        public bool isAI = false;
        public Transform ballTarget;
        [Range(0f, 1f)] public float aiDifficulty = 0.85f;
        [Range(0f, 1f)] public float aiSmoothness = 0.15f; // Lower = smoother
        
        private Vector3 _startPosition;
        private SpriteRenderer _childRenderer;
        private float _previousY;
        
        // AI-specific variables
        private float _aiTargetY;
        private float _aiCurrentVelocity = 0f; // For smooth damping
        private float _aiReactionTimer = 0f;
        private float _aiNextReactionTime = 0f;
        private float _aiErrorOffset = 0f;
        private Vector2 _aiNoiseOffset;

        public float VerticalVelocity { get; private set; } = 0f;

        void Awake()
        {
            _childRenderer = GetComponentInChildren<SpriteRenderer>();
            ApplyColor();
            _startPosition = transform.position;
            _previousY = transform.position.y;
            
            // Initialize AI randomization
            _aiNoiseOffset = new Vector2(Random.value * 100f, Random.value * 100f);
            _aiTargetY = transform.position.y;
            UpdateAITarget();
        }

        void Update()
        {
            float halfHeight = transform.localScale.y / 2f;
            Vector3 pos = transform.position;

            if (isAI && ballTarget != null)
            {
                UpdateAIMovement(ref pos, halfHeight);
            }
            else
            {
                // Human player input
                if (up.action.IsPressed() && pos.y + halfHeight < arenaHeight / 2)
                    pos.y = Mathf.Min(pos.y + Time.deltaTime * speed, arenaHeight / 2 - halfHeight);

                if (down.action.IsPressed() && pos.y - halfHeight > -arenaHeight / 2)
                    pos.y = Mathf.Max(pos.y - Time.deltaTime * speed, -arenaHeight / 2 + halfHeight);
            }

            transform.position = pos;

            // Compute vertical velocity
            VerticalVelocity = (transform.position.y - _previousY) / Time.deltaTime;
            _previousY = transform.position.y;
        }

        private void UpdateAIMovement(ref Vector3 pos, float halfHeight)
        {
            // Update reaction timer
            _aiReactionTimer += Time.deltaTime;
            
            // Periodically update target with reaction delay
            if (_aiReactionTimer >= _aiNextReactionTime)
            {
                UpdateAITarget();
                _aiReactionTimer = 0f;
            }

            // Add subtle Perlin noise for organic micro-movements
            float noiseValue = Mathf.PerlinNoise(
                Time.time * 0.8f + _aiNoiseOffset.x, 
                _aiNoiseOffset.y
            );
            float noiseInfluence = Mathf.Lerp(0.15f, 0.03f, aiDifficulty);
            float smoothTarget = _aiTargetY + (noiseValue - 0.5f) * noiseInfluence;

            // Use SmoothDamp for buttery smooth movement
            float maxSpeed = speed * Mathf.Lerp(0.8f, 1.1f, aiDifficulty);
            float newY = Mathf.SmoothDamp(
                pos.y, 
                smoothTarget, 
                ref _aiCurrentVelocity, 
                aiSmoothness, 
                maxSpeed,
                Time.deltaTime
            );

            // Clamp to arena bounds
            newY = Mathf.Clamp(newY, -arenaHeight / 2 + halfHeight, arenaHeight / 2 - halfHeight);
            
            // If we hit a boundary, reset velocity to prevent buildup
            if (Mathf.Approximately(newY, -arenaHeight / 2 + halfHeight) || 
                Mathf.Approximately(newY, arenaHeight / 2 - halfHeight))
            {
                _aiCurrentVelocity = 0f;
            }
            
            pos.y = newY;
        }

        private void UpdateAITarget()
        {
            // Reaction time based on difficulty
            _aiNextReactionTime = Mathf.Lerp(0.15f, 0.05f, aiDifficulty) + Random.Range(-0.02f, 0.02f);
            
            // Smaller tracking error for better AI
            float maxError = Mathf.Lerp(0.4f, 0.05f, aiDifficulty);
            _aiErrorOffset = Random.Range(-maxError, maxError);
            
            // Set target
            _aiTargetY = ballTarget.position.y + _aiErrorOffset;
        }

        public void Reset()
        {
            transform.position = _startPosition;
            ApplyColor();
            _previousY = _startPosition.y;
            
            // Reset AI state
            if (isAI)
            {
                _aiCurrentVelocity = 0f;
                _aiReactionTimer = 0f;
                _aiTargetY = _startPosition.y;
                UpdateAITarget();
            }
        }

        public void SetStartPosition(Vector3 position)
        {
            _startPosition = position;
            transform.position = position;
            _previousY = position.y;
        }

        private void ApplyColor()
        {
            if (_childRenderer != null)
                _childRenderer.color = playerColor;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_childRenderer == null)
                _childRenderer = GetComponentInChildren<SpriteRenderer>();

            ApplyColor();
        }
#endif
    }
}