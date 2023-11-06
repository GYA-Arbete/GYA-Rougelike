using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatSystem : MonoBehaviour
{
    public bool InCombat = false;

    [Header("Buttons")]
    public Button EndTurnButton;
    public Button ExitRoomButton;

    [Header("Combat Participants")]
    public Transform[] Players;
    public Transform[] Enemies;
    public bool[] DeadEnemies;

    [Header("CombatRoom-Exclusive Elements")]
    public GameObject[] CombatRoomElements;

    [Header("Stuff for checking card position")]
    public Transform CardParent;
    public List<Transform> CardsInMoveQueue;
    public Transform MoveQueueSnapPoint;

    [Header("Other Scripts")]
    public BarScript EnergyBarScript;
    public EnemySpawner EnemySpawnerScript;
    public CardSpawner CardSpawnerScript;
    public CameraSwitch CameraSwitchScript;

    // Start is called before the first frame update
    void Start()
    {
        EndTurnButton.onClick.AddListener(EndTurn);

        ExitRoomButton.onClick.AddListener(EndCombat);
    }

    public void StartCombat(int EnemyAmount, int[] EnemyTypes)
    {
        InCombat = true;

        foreach (GameObject Element in CombatRoomElements)
        {
            Element.SetActive(true);
        }

        Enemies = EnemySpawnerScript.SpawnEnemies(EnemyAmount, EnemyTypes);

        CardSpawnerScript.ResetCards();

        DeadEnemies = new bool[EnemyAmount];

        EnergyBarScript.SetupBar(10, new Color32(252, 206, 82, 255));

        CameraSwitchScript.SetViewToRoom();
    }

    public void EndCombat()
    {
        InCombat = false;

        foreach (GameObject Element in CombatRoomElements)
        {
            Element.SetActive(false);
        }

        // If enemy still exists, remove it
        foreach (Transform Enemy in Enemies)
        {
            if (Enemy != null)
            {
                Enemy.GetComponent<HealthSystem>().Die();
            }
        }

        CardSpawnerScript.DespawnCards();

        // Exit the room
        CameraSwitchScript.SetViewToMap();
    }

    void EndTurn()
    {
        GetCardsInMoveQueue();

        PlayCards();

        CardSpawnerScript.ResetCards();

        // Check if all Enemies are dead, eg combat has ended
        int DeadEnemiesAmount = 0;

        for (int i = 0; i < Enemies.Length; i++)
        {
            if (DeadEnemies[i] == true)
            {
                DeadEnemiesAmount++;
            }
        }
        if (DeadEnemiesAmount == Enemies.Length)
        {
            EndCombat();
            return;
        }

        // Enemies turn

        EnemyAttack();

        // Set back energy to 10 / 10
        EnergyBarScript.ResetBar();
    }

    void GetCardsInMoveQueue()
    {
        CardsInMoveQueue.Clear();

        Transform[] Cards = CardParent.GetComponentsInChildren<Transform>();

        foreach (Transform Card in Cards)
        {
            // If in MoveQueue and is the card itself, not attached text
            if (Card.position.y == MoveQueueSnapPoint.position.y && Card.GetComponent<CardStats>() != null)
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
            for (int i = 0; i < Enemies.Length; i++)
            {
                if (Enemies[i] != null)
                {
                    HealthSystem HealthSystemScript = Enemies[i].GetComponent<HealthSystem>();

                    if (HealthSystemScript.Health > 0)
                    {
                        DeadEnemies[i] = HealthSystemScript.TakeDamage(TotalDamage);
                    }
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
        int TotalDamage = 0;
        int TotalDefence = 0;

        // Get what to do
        foreach (Transform Enemy in Enemies)
        {
            if (Enemy != null)
            {
                EnemyStatsGen DamageSystemScript = Enemy.GetComponent<EnemyStatsGen>();
                TotalDamage += DamageSystemScript.Damage;
            }
        }

        // Do said things to self
        if (TotalDefence > 0)
        {
            foreach (Transform Enemy in Enemies)
            {
                HealthSystem HealthSystemScript = Enemy.GetComponent<HealthSystem>();

                HealthSystemScript.AddDefence(TotalDefence);
            }
        }

        // Do said things to players
        if (TotalDamage > 0)
        {
            foreach (Transform Player in Players)
            {
                HealthSystem HealthSystemScript = Player.GetComponent<HealthSystem>();

                if (HealthSystemScript.Health > 0)
                {
                    HealthSystemScript.TakeDamage(TotalDamage);
                }
            }
        }
    }
}
