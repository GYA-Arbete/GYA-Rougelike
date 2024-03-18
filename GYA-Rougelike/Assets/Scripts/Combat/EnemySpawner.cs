using Mirror;
using UnityEngine;

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

    [Header("Stat Generation")]
    public readonly int[] HealthMin = { 10, 10, 10, 10, 10, 5 };
    public readonly int[] HealthMax = { 50, 50, 50, 50, 50, 25 };
    public readonly int[] DamageMin = { 1, 1, 1, 1, 1, 1 };
    public readonly int[] DamageMax = { 5, 5, 5, 5, 5, 3 };

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
            SetItemParent(Enemy, EnemyParent);

            Enemies[i] = Enemy.transform;

            // Generate EnemyStats
            System.Random Rand = new();

            int MaxHealth = Rand.Next(HealthMin[EnemyTypes[i]], HealthMax[EnemyTypes[i]]);
            Enemy.GetComponent<HealthSystem>().SetupEnemy(MaxHealth);

            EnemyAI EnemyAIScript = Enemy.GetComponent<EnemyAI>();
            EnemyAIScript.Damage = Rand.Next(DamageMin[EnemyTypes[i]], DamageMax[EnemyTypes[i]]);

            CreateHealthBar(Enemy, "HealthBarParent", MaxHealth);
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
            SetItemParent(Summon, SummonParent);

            Summons[i] = Summon.transform;

            // Generate EnemyStats (5 == Summon in EnemyIndex)
            System.Random Rand = new();

            int MaxHealth = Rand.Next(HealthMin[5], HealthMax[5]);
            Summon.GetComponent<HealthSystem>().SetupEnemy(MaxHealth);

            EnemyAI EnemyAIScript = Summon.GetComponent<EnemyAI>();
            EnemyAIScript.Damage = Rand.Next(DamageMin[5], DamageMax[5]);

            CreateHealthBar(Summon, "SummonHealthBarParent", MaxHealth);
        }

        // Return an array containing all summons
        return Summons;
    }

    [Command(requiresAuthority = false)]
    void CreateHealthBar(GameObject Enemy, string ParentName, int MaxHealth)
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
        NetworkServer.Spawn(HealthBar);
        SetItemParent(HealthBar, Parent);

        SetupHealthbar(Enemy, HealthBar, MaxHealth);
    }

    [ClientRpc]
    void SetupHealthbar(GameObject Enemy, GameObject HealthBar, int MaxHealth)
    {
        // Get HealthSystemScript of the spawned enemy
        HealthSystem HealthSystemScript = Enemy.GetComponent<HealthSystem>();

        // Set values of the HealthBar
        BarScript HealthBarScript = HealthBar.GetComponent<BarScript>();
        HealthBarScript.SetupBar(MaxHealth, Color.red);

        // Setup HealthSystem
        HealthSystemScript.SetBarScript(HealthBarScript);
    }

    [ClientRpc]
    void SetItemParent(GameObject Element, Transform Parent)
    {
        Element.transform.SetParent(Parent, false);
    }
}
