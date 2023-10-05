using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatSystem : MonoBehaviour
{
    public int Energy;

    public bool PlayerTurn = true;

    public Button EndTurnButton;

    [Header("Other Scripts")]
    public EnemySpawner EnemySpawn;

    [Space]
    public int EnemyCount;
    public int[] EnemyTypesList;

    // Start is called before the first frame update
    void Start()
    {
        EndTurnButton.onClick.AddListener(EndTurn);

        EnemySpawn = FindObjectOfType<EnemySpawner>();
    }

    public void StartCombat(int EnemyAmount, int[] EnemyTypes)
    {
        Debug.Log("Started Combat");

        EnemyCount = EnemyAmount;
        EnemyTypesList = EnemyTypes;

        EnemySpawn.SpawnEnemies(EnemyAmount, EnemyTypes);
    }

    void EndTurn()
    {
        if (PlayerTurn)
        {
            PlayCards();

            PlayerTurn = false;
        }
        else
        {
            EnemyAttack();

            PlayerTurn = true;
        }
    }

    // Called when player has finished their turn, will play each card in the MoveQueue
    void PlayCards()
    {
        // Get from cards what to do

        // Do said things to enemies and self
    }

    // Called when its the enemies turn, they do stuff then
    void EnemyAttack()
    {

    }
}
