using UnityEngine;

namespace Objects
{
    public abstract class BasePowerUp : MonoBehaviour, IPowerUp
    {
        [Header("PowerUp Settings")]
        public float duration = 2f; // How long the effect lasts

        // Optional spawn behavior
        public virtual void OnSpawn() { }

        // Each powerup defines its effect
        public abstract void OnPickup(GameObject player);

        // Trigger-based pickup detection
        private void OnTriggerEnter2D(Collider2D other)
        {
            Ball ball = other.GetComponent<Ball>();
            if (ball != null)
            {
                OnPickup(ball.gameObject);
                Destroy(gameObject);
            }
        }
    }
}