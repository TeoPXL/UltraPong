using UnityEngine;

namespace Objects
{
    public interface IPowerUp
    {
        // Called when the ball picks up the powerup
        void OnPickup(GameObject player);

        // Optional: called when spawned
        void OnSpawn();
    }
}
