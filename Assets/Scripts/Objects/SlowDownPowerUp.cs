using System.Collections;
using UnityEngine;

namespace Objects
{
    public class SlowDownPowerUp : BasePowerUp
    {
        public float speedMultiplier = 0.5f;

        public override void OnPickup(GameObject player)
        {
            Ball ball = Object.FindFirstObjectByType<Ball>();
            if (ball != null)
                ball.StartCoroutine(ApplySlow(ball));
        }

        private IEnumerator ApplySlow(Ball ball)
        {
            // Apply slowdown
            ball.speed *= speedMultiplier;
            ball.Body.linearVelocity = ball.Body.linearVelocity.normalized * ball.speed;

            yield return new WaitForSeconds(duration);

            // Restore by dividing instead of resetting
            ball.speed /= speedMultiplier;
            ball.Body.linearVelocity = ball.Body.linearVelocity.normalized * ball.speed;
        }
    }
}