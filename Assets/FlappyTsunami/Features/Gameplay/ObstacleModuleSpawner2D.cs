using UnityEngine;

namespace hp55games.FlappyTsunami.Features.Gameplay
{
    public class ObstacleModuleSpawner2D : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private GameObject obstacleModulePrefab;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private float spawnInterval = 2f;

        private float _timer;

        private void Update()
        {
            _timer += Time.deltaTime;

            if (_timer >= spawnInterval)
            {
                _timer = 0f;
                SpawnModule();
            }
        }

        private void SpawnModule()
        {
            if (obstacleModulePrefab == null || spawnPoint == null)
            {
                Debug.LogWarning("[ObstacleModuleSpawner2D] Prefab o spawnPoint non assegnati.");
                return;
            }

            Instantiate(obstacleModulePrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}