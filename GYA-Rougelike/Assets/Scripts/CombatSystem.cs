using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    [Header("Other Scripts")]
    public EnemySpawner EnemySpawn;

    // Start is called before the first frame update
    void Start()
    {
        EnemySpawn = FindObjectOfType<EnemySpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartCombat(int EnemyAmount, int[] EnemyTypes)
    {
        EnemySpawn.SpawnEnemies(EnemyAmount, EnemyTypes);
    }
}
