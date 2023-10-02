using System.Collections;
using System.Collections.Generic;
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
    public int Health;
    public int Damage;

    public void GenerateStats(int EnemyType)
    {
        System.Random Rand = new();

        Health = Rand.Next(HealthMin[EnemyType], HealthMax[EnemyType]);
        Damage = Rand.Next(DamageMin[EnemyType], DamageMax[EnemyType]);
    }
}
