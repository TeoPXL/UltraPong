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
            // Pick a random prefab
            GameObject prefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Count)];

            // Calculate random position within radius
            float xPos = transform.position.x + Random.Range(-xRadius, xRadius);
            float yPos = transform.position.y + Random.Range(-yRadius, yRadius);
            Vector3 spawnPos = new Vector3(xPos, yPos, transform.position.z);

            // Instantiate the powerup
            GameObject powerUp = Instantiate(prefab, spawnPos, Quaternion.identity);

            // Add to active list
            _activePowerUps.Add(powerUp);

            // Remove from list when destroyed
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
