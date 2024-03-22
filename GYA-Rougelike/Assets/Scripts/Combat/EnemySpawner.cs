using Mirror;
using UnityEngine;
using System.Threading.Tasks;

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
    public readonly int[] HealthMin = { 100, 20, 10, 10, 30, 1 };
    public readonly int[] HealthMax = { 150, 40, 25, 25, 50, 5 };
    public readonly int[] DamageMin = { 8, 3, 1, 1, 1, 1 };
    public readonly int[] DamageMax = { 10, 6, 3, 2, 3, 2 };

    [Server]
    public Transform[] SpawnEnemies(int NumberOfEnemies, int[] EnemyTypes)
    {
        Transform[] Enemies = new Transform[NumberOfEnemies];

        for (int i = 0; i < NumberOfEnemies; i++)
        {
            // https://docs.unity3d.com/ScriptReference/Object.Instantiate.html
            // Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent);
            GameObject Enemy = Instantiate(EnemyPrefabs[EnemyTypes[i]], EnemySpawnPoints[i].position, new Quaternion(0, 0, 0, 0), EnemyParent);
            Enemy.transform.localScale = new Vector3(108, 108, 1);
            // If Boss
            if (EnemyTypes[i] == 0)
            {
                Enemy.transform.position = EnemySpawnPoints[1].position;
            }
            NetworkServer.Spawn(Enemy);
            SetItemParent(Enemy, EnemyParent);

            Enemies[i] = Enemy.transform;

            // Generate EnemyStats
            System.Random Rand = new();

            int MaxHealth = Rand.Next(HealthMin[EnemyTypes[i]], HealthMax[EnemyTypes[i]] + 1);
            Enemy.GetComponent<HealthSystem>().SetupObject(MaxHealth, false, -1);

            EnemyAI EnemyAIScript = Enemy.GetComponent<EnemyAI>();
            EnemyAIScript.Damage = Rand.Next(DamageMin[EnemyTypes[i]], DamageMax[EnemyTypes[i]] + 1);

            CreateHealthBar(Enemy, "HealthBarParent", MaxHealth);
        }

        // Return an array containing all enemies
        return Enemies;
    }

    [Server]
    public async Task<(Transform[], bool[])> SpawnSummons(Transform[] Summons)
    {
        bool[] SpawnedEnemy = new bool[4];

        for (int i = 0; i < 4; i++)
        {
            // If there isnt already a summon for that position
            if (Summons[i] == null)
            {
                SpawnedEnemy[i] = true;

                // https://docs.unity3d.com/ScriptReference/Object.Instantiate.html
                // Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent);
                GameObject Summon = Instantiate(SummonPrefab, SummonSpawnPoints[i].position, new Quaternion(0, 0, 0, 0), SummonParent);
                Summon.transform.localScale = new Vector3(80, 80, 1);
                NetworkServer.Spawn(Summon);
                SetItemParent(Summon, SummonParent);

                Summons[i] = Summon.transform;

                // Generate EnemyStats (5 == Summon in EnemyIndex)
                System.Random Rand = new();

                int MaxHealth = Rand.Next(HealthMin[5], HealthMax[5] + 1);
                Summon.GetComponent<HealthSystem>().SetupObject(MaxHealth, false, -1);

                EnemyAI EnemyAIScript = Summon.GetComponent<EnemyAI>();
                EnemyAIScript.Damage = Rand.Next(DamageMin[5], DamageMax[5] + 1);

                CreateHealthBar(Summon, "SummonHealthBarParent", MaxHealth);
            }
        }

        // Return an array containing all summons
        return (Summons, SpawnedEnemy);
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
