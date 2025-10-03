using UnityEngine;
using UnityEngine.InputSystem;

namespace Objects
{
    public class Player : MonoBehaviour
    {
        public InputActionReference up;
        public InputActionReference down;
        public float arenaHeight;
        public float speed = 5f;

        [Header("Appearance")]
        [SerializeField] private Color playerColor = Color.white; // 👈 Serialized so Inspector shows it

        private Vector3 _startPosition;
        private SpriteRenderer _childRenderer;

        void Awake()
        {
            // Grab the SpriteRenderer from the child
            _childRenderer = GetComponentInChildren<SpriteRenderer>();
            ApplyColor();
        }

        void Start()
        {
            _startPosition = transform.localPosition;
        }

        void Update()
        {
            float halfHeight = transform.localScale.y / 2f;

            // Move up
            if (up.action.IsPressed() && transform.localPosition.y + halfHeight < arenaHeight / 2)
            {
                float newY = Mathf.Min(arenaHeight / 2 - halfHeight, transform.localPosition.y + Time.deltaTime * speed);
                transform.localPosition = new Vector3(transform.localPosition.x, newY, 0);
            }

            // Move down
            if (down.action.IsPressed() && transform.localPosition.y - halfHeight > -arenaHeight / 2)
            {
                float newY = Mathf.Max(-arenaHeight / 2 + halfHeight, transform.localPosition.y - Time.deltaTime * speed);
                transform.localPosition = new Vector3(transform.localPosition.x, newY, 0);
            }
        }
        
        public void Reset()
        {
            transform.localPosition = _startPosition;
            ApplyColor();
        }

        private void ApplyColor()
        {
            if (_childRenderer != null)
            {
                _childRenderer.color = playerColor;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Update color in editor immediately
            if (_childRenderer == null)
                _childRenderer = GetComponentInChildren<SpriteRenderer>();

            ApplyColor();
        }
#endif
    }
}
