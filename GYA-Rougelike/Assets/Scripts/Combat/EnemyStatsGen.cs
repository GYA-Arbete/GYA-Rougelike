using UnityEngine;

public class EnemyStatsGen : MonoBehaviour
{
    [Header("Health")]
    public int[] HealthMin = { 10, 10, 10, 10, 10 };
    public int[] HealthMax = { 50, 50, 50, 50, 50 };

    [Header("Damage")]
    public int[] DamageMin = { 1, 1, 1, 1, 1 };
    public int[] DamageMax = { 5, 5, 5, 5, 5 };

    [Header("Enemy Stats")]
    public int Damage;

    public void GenerateStats(int EnemyType)
    {
        HealthSystem HealthSystemScript = gameObject.GetComponent<HealthSystem>();

        System.Random Rand = new();

        HealthSystemScript.MaxHealth = Rand.Next(HealthMin[EnemyType], HealthMax[EnemyType]);
        Damage = Rand.Next(DamageMin[EnemyType], DamageMax[EnemyType]);
    }
}
