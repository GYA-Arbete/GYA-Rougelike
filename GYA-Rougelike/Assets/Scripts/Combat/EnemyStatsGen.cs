using UnityEngine;

public class EnemyStatsGen : MonoBehaviour
{
    [Header("Health")]
    public int[] HealthMin = { 10, 10, 10, 10, 10, 5 };
    public int[] HealthMax = { 50, 50, 50, 50, 50, 25 };

    [Header("Damage")]
    public int[] DamageMin = { 1, 1, 1, 1, 1, 1 };
    public int[] DamageMax = { 5, 5, 5, 5, 5, 3 };

    public void GenerateStats(int EnemyType)
    {
        HealthSystem HealthSystemScript = gameObject.GetComponent<HealthSystem>();
        EnemyAI EnemyAIScript = gameObject.GetComponent<EnemyAI>();

        System.Random Rand = new();

        HealthSystemScript.MaxHealth = Rand.Next(HealthMin[EnemyType], HealthMax[EnemyType]);
        EnemyAIScript.Damage = Rand.Next(DamageMin[EnemyType], DamageMax[EnemyType]);
    }
}
