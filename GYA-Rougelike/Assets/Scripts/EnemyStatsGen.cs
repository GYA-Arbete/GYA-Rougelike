using UnityEngine;

public class EnemyStatsGen : MonoBehaviour
{
    [Header("Health")]
    public int[] HealthMin = { 1, 1, 1, 1, 1 };
    public int[] HealthMax = { 5, 5, 5, 5, 5 };

    [Header("Damage")]
    public int[] DamageMin = { 1, 1, 1, 1, 1 };
    public int[] DamageMax = { 5, 5, 5, 5, 5 };

    [Header("Enemy Stats")]
    public int Damage;

    public void GenerateStats(int EnemyType, Transform Object)
    {
        HealthSystem HealthSystemScript = Object.GetComponent<HealthSystem>();

        System.Random Rand = new();

        HealthSystemScript.MaxHealth = Rand.Next(HealthMin[EnemyType], HealthMax[EnemyType]);
        Damage = Rand.Next(DamageMin[EnemyType], DamageMax[EnemyType]);
    }
}
