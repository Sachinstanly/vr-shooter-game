using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Prefab for the enemy to spawn
    public Transform spawnPoint; // The point where the enemy will spawn
    public float spawnDelay = 2f; // Delay between spawns

    public void StartSpawningEnemies()
    {
        StartCoroutine(SpawnEnemies()); // Start the spawning coroutine
    }

    private IEnumerator SpawnEnemies()
    {
        while (true) // Keep spawning enemies indefinitely
        {
            // Spawn an enemy
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

            // Wait until the enemy is destroyed
            yield return new WaitUntil(() => enemy == null);

            // Optional: Wait a delay before spawning the next enemy
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    public void StopSpawningEnemies()
    {
        StopAllCoroutines();
        EnemyController[] enemies = GameObject.FindObjectsOfType<EnemyController>();
        foreach (EnemyController enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
    }
}
