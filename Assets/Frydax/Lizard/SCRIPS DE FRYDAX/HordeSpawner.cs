using UnityEngine;

public class HordeSpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public Transform[] patrolPoints;

    public int amount = 10;

    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < amount; i++)
        {
            Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // ?? offset para que no se amontonen
            Vector3 randomOffset = new Vector3(
                Random.Range(-2f, 2f),
                0,
                Random.Range(-2f, 2f)
            );

            GameObject enemy = Instantiate(enemyPrefab, spawn.position + randomOffset, Quaternion.identity);

            EnemyAI_Pro ai = enemy.GetComponent<EnemyAI_Pro>();

            if (ai != null)
            {
                ai.patrolPoints = patrolPoints;
            }
        }
    }
}