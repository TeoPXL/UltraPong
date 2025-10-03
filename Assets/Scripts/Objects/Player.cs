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
        public Transform ballTarget; // the ball the AI should track

        private Vector3 _startPosition;
        private SpriteRenderer _childRenderer;
        private float _previousY;

        public float VerticalVelocity { get; private set; } = 0f;

        void Awake()
        {
            _childRenderer = GetComponentInChildren<SpriteRenderer>();
            ApplyColor();
            _startPosition = transform.position;
            _previousY = transform.position.y;
        }

        void Update()
        {
            float halfHeight = transform.localScale.y / 2f;
            Vector3 pos = transform.position;

            if (isAI && ballTarget != null)
            {
                float step = speed * Time.deltaTime;

                // Add slight random offset so AI doesn't perfectly align
                float randomOffset = Mathf.Sin(Time.time * 2f) * 0.3f; 
                float targetY = ballTarget.position.y + randomOffset;

                if (Mathf.Abs(targetY - pos.y) > 0.1f)
                {
                    if (targetY > pos.y && pos.y + halfHeight < arenaHeight / 2)
                        pos.y = Mathf.Min(pos.y + step, arenaHeight / 2 - halfHeight);
                    else if (targetY < pos.y && pos.y - halfHeight > -arenaHeight / 2)
                        pos.y = Mathf.Max(pos.y - step, -arenaHeight / 2 + halfHeight);
                }
            }
            else
            {
                if (up.action.IsPressed() && pos.y + halfHeight < arenaHeight / 2)
                    pos.y = Mathf.Min(pos.y + Time.deltaTime * speed, arenaHeight / 2 - halfHeight);

                if (down.action.IsPressed() && pos.y - halfHeight > -arenaHeight / 2)
                    pos.y = Mathf.Max(pos.y - Time.deltaTime * speed, -arenaHeight / 2 + halfHeight);
            }

            // Update position
            transform.position = pos;

            // Compute vertical velocity for bounce calculations
            VerticalVelocity = (transform.position.y - _previousY) / Time.deltaTime;
            _previousY = transform.position.y;
        }

        public void Reset()
        {
            transform.position = _startPosition;
            ApplyColor();
        }

        public void SetStartPosition(Vector3 position)
        {
            _startPosition = position;
            transform.position = position;
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
