using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatsGen : MonoBehaviour
{
    public Dictionary<int, int> HealthMinMax = new() { { 1, 5 }, { 1, 5 }, { 1, 5 }, { 1, 5 }, { 1, 5 } };
    public Dictionary<int, int> DamageMinMax = new() { { 1, 5 }, { 1, 5 }, { 1, 5 }, { 1, 5 }, { 1, 5 } };

    [Header("Enemy Stats")]
    public int Health;
    public int Damage;

    void GenerateStats(int EnemyType)
    {
        System.Random Rand = new();

        Health = Rand.Next(HealthMinMax[EnemyType]);
        Damage = Rand.Next(DamageMinMax[EnemyType]);
    }
}
