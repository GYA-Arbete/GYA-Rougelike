using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public int Energy;

    [Header("Other Scripts")]
    public EnemySpawner EnemySpawn;

    // Start is called before the first frame update
    void Start()
    {
        EnemySpawn = FindObjectOfType<EnemySpawner>();
    }

    public void StartCombat(int EnemyAmount, int[] EnemyTypes)
    {
        EnemySpawn.SpawnEnemies(EnemyAmount, EnemyTypes);
    }

    // Called when player has finished their turn, will play each card in the MoveQueue
    void PlayCards()
    {

    }
}
