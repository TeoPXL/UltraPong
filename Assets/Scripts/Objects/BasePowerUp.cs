using UnityEngine;

namespace Objects
{
    public abstract class BasePowerUp : MonoBehaviour, IPowerUp
    {
        [Header("PowerUp Settings")]
        public float duration = 2f; // How long the effect lasts

        public virtual void OnSpawn()
        {
            // Optional spawn behavior
        }

        public abstract void OnPickup(GameObject player);
    }
}
