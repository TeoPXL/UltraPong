using UnityEngine;

namespace Objects
{
    [RequireComponent(typeof(Collider2D))]
    public class Lava : MonoBehaviour
    {
        public float slowDownFactor = 0.5f;  
        public float slowDownDuration = 2f;   
        private void OnTriggerEnter2D(Collider2D other)
        {
            Ball ball = other.GetComponent<Ball>();
            if (ball != null)
            {
                ball.ApplySlow(slowDownFactor, slowDownDuration);
            }
        }
    }
}
