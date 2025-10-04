using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    public class PowerUpSpawner : MonoBehaviour
    {
        [Header("PowerUp Settings")]
        public List<GameObject> powerUpPrefabs;   // List of powerups to spawn
        public int maxPowerUps = 3;               // Maximum active powerups at a time
        public float spawnInterval = 5f;          // Time between spawns in seconds

        [Header("Spawn Area")]
        public float xRadius = 5f;
        public float yRadius = 3.5f;

        private List<GameObject> _activePowerUps = new List<GameObject>();

        void Start()
        {
            // Start spawning powerups
            StartCoroutine(SpawnPowerUpsRoutine());
        }

        IEnumerator SpawnPowerUpsRoutine()
        {
            while (true)
            {
                // Only spawn if we haven't reached the max
                if (_activePowerUps.Count < maxPowerUps && powerUpPrefabs.Count > 0)
                {
                    SpawnPowerUp();
                }

                // Wait for next spawn
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        void SpawnPowerUp()
        {
            GameObject prefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Count)];

            float xPos = transform.position.x + Random.Range(-xRadius, xRadius);
            float yPos = transform.position.y + Random.Range(-yRadius, yRadius);
            Vector3 spawnPos = new Vector3(xPos, yPos, transform.position.z);

            // Set parent to this spawner (which is part of arena)
            GameObject powerUp = Instantiate(prefab, spawnPos, Quaternion.identity, transform);

            _activePowerUps.Add(powerUp);
            powerUp.AddComponent<PowerUpTracker>().Initialize(this);
        }

        // Called by PowerUpTracker when a powerup is destroyed
        public void NotifyPowerUpDestroyed(GameObject powerUp)
        {
            _activePowerUps.Remove(powerUp);
        }

        void Reset()
        {
            _activePowerUps.Clear();
        }
        
        public void ClearAllPowerUps()
        {
            // Destroy all active powerups
            foreach (var pu in _activePowerUps)
            {
                if (pu != null)
                    Destroy(pu);
            }
            _activePowerUps.Clear();
        }
    }

    // Helper component to track when powerups are destroyed
    public class PowerUpTracker : MonoBehaviour
    {
        private PowerUpSpawner _spawner;

        public void Initialize(PowerUpSpawner spawner)
        {
            this._spawner = spawner;
        }

        void OnDestroy()
        {
            if (_spawner != null)
            {
                _spawner.NotifyPowerUpDestroyed(gameObject);
            }
        }
    }
}
