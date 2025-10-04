using System.Collections;
using UnityEngine;

namespace Objects
{
    public class SpeedUpPowerUp : BasePowerUp
    {
        public float speedMultiplier = 1.5f;

        public override void OnPickup(GameObject player)
        {
            Ball ball = Object.FindFirstObjectByType<Ball>();
            if (ball != null)
                ball.StartCoroutine(ApplySpeed(ball));
        }

        private IEnumerator ApplySpeed(Ball ball)
        {
            // Multiply speed
            ball.speed *= speedMultiplier;
            ball.Body.linearVelocity = ball.Body.linearVelocity.normalized * ball.speed;

            yield return new WaitForSeconds(duration);

            // Restore by dividing instead of resetting to an "original" value
            ball.speed /= speedMultiplier;
            ball.Body.linearVelocity = ball.Body.linearVelocity.normalized * ball.speed;
        }
    }
}