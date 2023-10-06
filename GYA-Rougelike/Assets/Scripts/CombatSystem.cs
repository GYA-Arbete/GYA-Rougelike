using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatSystem : MonoBehaviour
{
    public int Energy;

    public bool PlayerTurn = true;

    public Button EndTurnButton;

    public Transform[] Players;
    public Transform[] Enemies;

    [Header("Stuff for checking card position")]
    public Transform CardParent;
    public List<Transform> CardsInMoveQueue;
    public Transform MoveQueueSnapPoint;

    [Header("Other Scripts")]
    public EnemySpawner EnemySpawn;

    // Start is called before the first frame update
    void Start()
    {
        EndTurnButton.onClick.AddListener(EndTurn);

        EnemySpawn = FindObjectOfType<EnemySpawner>();
    }

    public void StartCombat(int EnemyAmount, int[] EnemyTypes)
    {
        Enemies = EnemySpawn.SpawnEnemies(EnemyAmount, EnemyTypes);
    }

    void EndTurn()
    {
        if (PlayerTurn)
        {
            GetCardsInMoveQueue();

            PlayCards();

            PlayerTurn = false;
        }
        else
        {
            EnemyAttack();

            PlayerTurn = true;
        }
    }

    void GetCardsInMoveQueue()
    {
        CardsInMoveQueue.Clear();

        Transform[] Cards = CardParent.GetComponentsInChildren<Transform>();

        foreach (Transform Card in Cards)
        {
            if (Card.position.y == MoveQueueSnapPoint.position.y)
            {
                CardsInMoveQueue.Add(Card);
            }
        }
    }

    // Called when player has finished their turn, will play each card in the MoveQueue
    void PlayCards()
    {
        int TotalDamage = 0;
        int TotalDefence = 0;

        // Get from cards what to do
        foreach (Transform Card in CardsInMoveQueue)
        {
            CardStats Stats = Card.GetComponent<CardStats>();

            TotalDamage += Stats.Damage;
            TotalDefence += Stats.Defence;
        }

        // Do said things to enemies
        if (TotalDamage > 0)
        {
            
            foreach (Transform Enemy in Enemies)
            {
                HealthSystem HealthSystemScript = Enemy.GetComponent<HealthSystem>();

                if (HealthSystemScript.Health > 0)
                {
                    HealthSystemScript.TakeDamage(TotalDamage);
                }
            }
        }
        
        // Do said things to self
        if (TotalDefence > 0)
        {
            foreach (Transform Player in Players)
            {
                HealthSystem HealthSystemScript = Player.GetComponent<HealthSystem>();

                HealthSystemScript.AddDefence(TotalDefence);
            }
        }
        
    }

    // Called when its the enemies turn, they do stuff then
    void EnemyAttack()
    {

    }
}
