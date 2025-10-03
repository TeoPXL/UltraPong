using UnityEngine;

namespace Objects
{
    public class PlayerAi : Player
    {
        public Ball targetBall;
        public float followSmoothness = 5f; 

        private void Update()
        {
            if (targetBall == null) return;

            float targetY = targetBall.transform.localPosition.y;

            targetY = Mathf.Clamp(targetY, -arenaHeight / 2 + size / 2, arenaHeight / 2 - size / 2);

            float newY = Mathf.Lerp(
                transform.localPosition.y,
                targetY,
                Time.deltaTime * followSmoothness
            );

            transform.localPosition = new Vector3(transform.localPosition.x, newY, 0);
        }
    }
}
