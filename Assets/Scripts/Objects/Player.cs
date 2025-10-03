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

        void Awake()
        {
            // Grab the SpriteRenderer from the child
            _childRenderer = GetComponentInChildren<SpriteRenderer>();
            ApplyColor();

            // Capture the starting position in world space
            _startPosition = transform.position;
        }

        void Update()
        {
            float halfHeight = transform.localScale.y / 2f;
            Vector3 pos = transform.position;

            if (isAI && ballTarget != null)
            {
                // Move towards the ball's y position
                float step = speed * Time.deltaTime;

                if (ballTarget.position.y > pos.y + 0.1f && pos.y + halfHeight < arenaHeight / 2)
                    pos.y = Mathf.Min(pos.y + step, arenaHeight / 2 - halfHeight);
                else if (ballTarget.position.y < pos.y - 0.1f && pos.y - halfHeight > -arenaHeight / 2)
                    pos.y = Mathf.Max(pos.y - step, -arenaHeight / 2 + halfHeight);
            }
            else
            {
                // Human input movement
                if (up.action.IsPressed() && pos.y + halfHeight < arenaHeight / 2)
                    pos.y = Mathf.Min(pos.y + Time.deltaTime * speed, arenaHeight / 2 - halfHeight);

                if (down.action.IsPressed() && pos.y - halfHeight > -arenaHeight / 2)
                    pos.y = Mathf.Max(pos.y - Time.deltaTime * speed, -arenaHeight / 2 + halfHeight);
            }

            transform.position = pos;
        }

        /// <summary>
        /// Resets player to its starting position and reapplies color.
        /// </summary>
        public void Reset()
        {
            transform.position = _startPosition;
            ApplyColor();
        }

        /// <summary>
        /// Allows Arena or other scripts to explicitly set the start position.
        /// </summary>
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
