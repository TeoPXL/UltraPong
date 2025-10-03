using UnityEngine;
using UnityEngine.InputSystem;

namespace Objects
{
    public class Player : MonoBehaviour
    {
        public float speed;
        public float size = 1.5f;
        public InputActionReference up;
        public InputActionReference down;
        public float arenaHeight;

        void Update()
        {
            if (transform.localScale.y != size)
            {
                transform.localScale = new Vector3(0.2f, size, 0);
            }
            if (up.action.IsPressed() && transform.localPosition.y + size / 2 < arenaHeight / 2)
            {
                float newY = Mathf.Min(arenaHeight / 2, transform.localPosition.y + Time.deltaTime * speed);
                transform.localPosition = new Vector3(transform.localPosition.x, newY, 0);
            }
            if (down.action.IsPressed() && transform.localPosition.y - size / 2 > -arenaHeight / 2)
            {
                float newY = Mathf.Max(-arenaHeight / 2, transform.localPosition.y - Time.deltaTime * speed);
                transform.localPosition = new Vector3(transform.localPosition.x, newY, 0);
            }
        }
    }
}
