using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : NetworkBehaviour
{
    public Transform[] EnemySpawnPoints;
    public GameObject[] EnemyPrefabs;
    public Transform EnemyParent;

    [Header("Summon Stuff")]
    public Transform[] SummonSpawnPoints;
    public GameObject SummonPrefab;
    public Transform SummonParent;

    [Header("HealthBars")]
    public GameObject HealthBarPrefab;
    public Transform HealthBarParent;
    public Transform SummonHealthBarParent;
    public Sprite HealthBarImage;

    [Server]
    public Transform[] SpawnEnemies(int NumberOfEnemies, int[] EnemyTypes)
    {
        Transform[] Enemies = new Transform[NumberOfEnemies];

        for (int i = 0; i < NumberOfEnemies; i++)
        {
            // https://docs.unity3d.com/ScriptReference/Object.Instantiate.html
            // Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent);
            GameObject Enemy = Instantiate(EnemyPrefabs[EnemyTypes[i]], new Vector3(EnemySpawnPoints[i].position.x, EnemySpawnPoints[i].position.y, EnemySpawnPoints[i].position.z), new Quaternion(0, 0, 0, 0), EnemyParent);
            Enemy.transform.localScale = new Vector3(108, 108, 1);
            NetworkServer.Spawn(Enemy);
            SetItemParent(Enemy, "EnemyParent");

            Enemies[i] = Enemy.transform;

            // Generate EnemyStats
            Enemy.GetComponent<EnemyStatsGen>().GenerateStats(EnemyTypes[i]);

            CreateHealthBar(Enemy, "HealthBarParent");
        }

        // Return an array containing all enemies
        return Enemies;
    }

    [Server]
    public Transform[] SpawnSummons()
    {
        Transform[] Summons = new Transform[4];

        for (int i = 0; i < 4; i++)
        {
            // https://docs.unity3d.com/ScriptReference/Object.Instantiate.html
            // Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent);
            GameObject Summon = Instantiate(SummonPrefab, SummonSpawnPoints[i].position, new Quaternion(0, 0, 0, 0), SummonParent);
            Summon.transform.localScale = new Vector3(80, 80, 1);
            NetworkServer.Spawn(Summon);
            SetItemParent(Summon, "SummonParent");

            Summons[i] = Summon.transform;

            // Generate EnemyStats
            Summon.GetComponent<EnemyStatsGen>().GenerateStats(5); // 5 == Summon in EnemyIndex

            CreateHealthBar(Summon, "SummonHealthBarParent");
        }

        // Return an array containing all summons
        return Summons;
    }

    [ClientRpc]
    void CreateHealthBar(GameObject Enemy, string ParentName)
    {
        // Workaround so game correctly sets parent on each client, suspect its because function isnt called from command
        Transform Parent;
        if (ParentName == "HealthBarParent")
        {
            Parent = HealthBarParent;
        }
        else
        {
            Parent = SummonHealthBarParent;
        }

        // Create a HealthBar
        GameObject HealthBar = Instantiate(HealthBarPrefab, new Vector3(Enemy.transform.position.x, Enemy.transform.position.y - 1, Enemy.transform.position.z), new Quaternion(0, 0, 0, 0), Parent);
        HealthBar.transform.localScale = new Vector3(0.5f, 0.5f, 1);

        // Set image on HealthBar
        Transform BarImageItem = HealthBar.transform.Find("ResourceImage");
        Image ImageItem = BarImageItem.GetComponent<Image>();
        ImageItem.sprite = HealthBarImage;

        // Get HealthSystemScript of the spawned enemy
        HealthSystem HealthSystemScript = Enemy.GetComponent<HealthSystem>();

        // Set values of the HealthBar
        BarScript HealthBarScript = HealthBar.GetComponent<BarScript>();
        HealthBarScript.SetupBar(HealthSystemScript.MaxHealth, Color.red);

        // Setup HealthSystem
        HealthSystemScript.SetupEnemy(HealthBarScript);
    }

    [ClientRpc]
    void SetItemParent(GameObject Element, string ParentName)
    {
        // Workaround so game correctly sets parent on each client, suspect its because function isnt called from command
        Transform Parent;
        if (ParentName == "EnemyParent")
        {
            Parent = EnemyParent;
        }
        else
        {
            Parent = SummonParent;
        }

        Element.transform.SetParent(Parent, false);
    }
}
