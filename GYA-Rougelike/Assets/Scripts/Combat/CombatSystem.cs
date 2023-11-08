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

    [Header("EnemyAI Stuff")]
    public Sprite[] MoveIndicators;
    public int[] EnemyMove;
    public bool[] SplashDamage;

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
        EnemyMove = new int[EnemyAmount];
        SplashDamage = new bool[EnemyAmount];

        EnergyBarScript.SetupBar(10, new Color32(252, 206, 82, 255));

        CameraSwitchScript.SetViewToRoom();

        // Generate each enemies next turn
        for (int i = 0; i < EnemyAmount; i++) 
        {
            EnemyAI EnemyAIScript = Enemies[i].gameObject.GetComponent<EnemyAI>();

            EnemyAIScript.SetupEnemy(EnemyTypes[i], MoveIndicators);

            // Get each enemies move
            var ReturnedValues = EnemyAIScript.GenerateMove();
            EnemyMove[i] = ReturnedValues.Item1;
            SplashDamage[i] = ReturnedValues.Item2;
        }
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

        EnemyTurn();

        // Set back energy to 10 / 10
        EnergyBarScript.ResetBar();

        // Generate each enemies next turn
        for (int i = 0; i < Enemies.Length; i++)
        {
            if (Enemies[i] != null)
            {
                // Get each enemies move
                var ReturnedValues = Enemies[i].GetComponent<EnemyAI>().GenerateMove();
                EnemyMove[i] = ReturnedValues.Item1;
                SplashDamage[i] = ReturnedValues.Item2;
            }
        }
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
        // Do each cards "thing"
        for (int i = 0; i < CardsInMoveQueue.Count; i++)
        {
            CardStats CardStatsScript = CardsInMoveQueue[i].gameObject.GetComponent<CardStats>();

            if (CardStatsScript.SplashDamage == true)
            {
                foreach (Transform Enemy in Enemies)
                {
                    if (Enemy != null)
                    {
                        Enemy.GetComponent<HealthSystem>().TakeDamage(CardStatsScript.Damage);
                    }
                }
                break;
            }
            else if (CardStatsScript.Defence > 0)
            {
                foreach (Transform Player in Players)
                {
                    Player.GetComponent<HealthSystem>().AddDefence(CardStatsScript.Defence);
                }
                break;
            }
            else
            {
                foreach (Transform Enemy in Enemies)
                {
                    if (Enemy != null)
                    {
                        Enemy.GetComponent<HealthSystem>().TakeDamage(CardStatsScript.Damage);
                        break;
                    }
                }
            }
        }
    }

    // Called when its the enemies turn, they do stuff then
    void EnemyTurn()
    {
        // Do each move in EnemyMoves
        for (int i = 0; i < EnemyMove.Length; i++)
        {
            if (Enemies[i] != null)
            {
                if (SplashDamage[i] == true)
                {
                    int Damage = Enemies[i].GetComponent<EnemyAI>().Damage;

                    foreach (Transform Player in Players)
                    {
                        if (Player != null)
                        {
                            Player.GetComponent<HealthSystem>().TakeDamage(Damage);
                        }
                    }
                }
                else
                {
                    switch (EnemyMove[i])
                    {
                        // Cleave (Normal splash)
                        case 0:
                            break;
                        // Slash (Normal attack)
                        case 1:
                            int Damage = Enemies[i].GetComponent<EnemyAI>().Damage;

                            foreach (Transform Player in Players)
                            {
                                if (Player != null)
                                {
                                    Player.GetComponent<HealthSystem>().TakeDamage(Damage);
                                    break;
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
}
