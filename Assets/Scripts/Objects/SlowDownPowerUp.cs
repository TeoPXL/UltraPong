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

            Destroy(gameObject);
        }

        private IEnumerator ApplySlow(Ball ball)
        {
            float originalSpeed = ball.speed;
            ball.speed *= speedMultiplier;
            ball.Body.linearVelocity = ball.Body.linearVelocity.normalized * ball.speed;

            yield return new WaitForSeconds(duration);

            ball.speed = originalSpeed;
            ball.Body.linearVelocity = ball.Body.linearVelocity.normalized * ball.speed;
        }
    }
}