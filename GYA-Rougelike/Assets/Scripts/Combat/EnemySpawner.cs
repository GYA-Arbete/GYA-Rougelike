using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    public Transform[] EnemySpawnPoints;
    public GameObject[] EnemyPrefabs;
    public Transform EnemyParent;

    [Header("HealthBar")]
    public GameObject HealthBarPrefab;
    public Transform HealthBarParent;
    public Sprite HealthBarImage;

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

            // Generate EnemyStats
            Enemy.GetComponent<EnemyStatsGen>().GenerateStats(EnemyTypes[i]);

            // Create a HealthBar, putting it under the enemy
            GameObject HealthBar = Instantiate(HealthBarPrefab, new Vector3(Enemy.transform.position.x, Enemy.transform.position.y - 1, Enemy.transform.position.z), new Quaternion(0, 0, 0, 0), HealthBarParent);
            HealthBar.transform.localScale = new Vector3(0.5f, 0.5f, 1);

            // Set image on HealthBar
            Transform BarImageItem = HealthBar.transform.Find("ResourceImage");
            Image ImageItem = BarImageItem.GetComponent<Image>();
            ImageItem.sprite = HealthBarImage;

            // Set color of HealthBar
            Transform BarFillItem = HealthBar.transform.Find("Fill");
            Image FillItem = BarFillItem.GetComponent<Image>();
            FillItem.color = Color.red;

            // Get HealthSystemScript of the spawned Enemy
            HealthSystem HealthSystemScript = Enemy.GetComponent<HealthSystem>();

            // Set values of said HealthBar
            BarScript HealthBarScript = HealthBar.GetComponent<BarScript>();
            HealthBarScript.SetupBar(HealthSystemScript.MaxHealth, Color.red);

            // Setup HealthSystem
            HealthSystemScript.SetHealth(HealthBarScript);
        }

        // Return an array containing all enemies
        return Enemies;
    }
}
