using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform[] EnemySpawnPoints;
    public GameObject[] EnemyPrefabs;
    public Transform EnemyParent;

    public Transform[] SpawnEnemies(int NumberOfEnemies, int[] EnemyTypes)
    {
        Transform[] Enemies = new Transform[NumberOfEnemies];

        for (int i = 0; i < NumberOfEnemies; i++)
        {
            // https://docs.unity3d.com/ScriptReference/Object.Instantiate.html
            // Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent);
            // Skapa en ny EnemyPrefab för varje fiende som ska skapas
            GameObject Enemy = Instantiate(EnemyPrefabs[EnemyTypes[i]], new Vector3(EnemySpawnPoints[i].position.x, EnemySpawnPoints[i].position.y, EnemySpawnPoints[i].position.z), new Quaternion(0, 0, 0, 0), EnemyParent);

            Enemies[i] = Enemy.transform;

            EnemyStatsGen EnemyStatsGenScript = Enemy.GetComponent<EnemyStatsGen>();
            EnemyStatsGenScript.GenerateStats(EnemyTypes[i], Enemy.transform);
        }

        return Enemies;
    }
}
