using System.Collections;
using UnityEngine;

namespace Objects
{
    public class AngleChangePowerUp : BasePowerUp
    {
        public float angleOffset = 30f; // Degrees to rotate the ball

        public override void OnPickup(GameObject player)
        {
            Ball ball = Object.FindFirstObjectByType<Ball>();
            if (ball != null)
                ball.StartCoroutine(ApplyAngle(ball));

            Destroy(gameObject);
        }

        private IEnumerator ApplyAngle(Ball ball)
        {
            Vector2 direction = ball.Body.linearVelocity.normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Apply offset
            angle += angleOffset;

            Vector2 newDir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            ball.Body.linearVelocity = newDir.normalized * ball.speed;

            yield return new WaitForSeconds(duration);

            // Optionally: reset angle or leave as is
        }
    }
}