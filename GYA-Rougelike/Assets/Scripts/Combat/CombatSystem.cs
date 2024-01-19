using System.Collections.Generic;
using System;
using System.Linq;
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
    public int[] EnemyType;
    public Transform[] Summons;
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
    public int[] EnemyDamageBuff;
    public int PlayerDamageBuff;
    public int[] StunDuration;

    [Header("Stuff for drawing TargedIndicators (from DragDropCardComponent.cs)")]
    public Transform EnemyTarget;

    [Header("Other Scripts")]
    public BarScript EnergyBarScript;
    public EnemySpawner EnemySpawnerScript;
    public CardSpawner CardSpawnerScript;
    public CameraSwitch CameraSwitchScript;
    public CardInventory CardInventoryScript;

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
        EnemyDamageBuff = new int[EnemyAmount];
        StunDuration = new int[EnemyAmount];

        EnemyType = EnemyTypes;

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

            // Set default value
            EnemyDamageBuff[i] = 0;
            StunDuration[i] = 0;
        }

        // Save which enemy will be targeted by the players card
        EnemyTarget = Enemies[0];
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
                Enemy.gameObject.GetComponent<HealthSystem>().Die();
            }
        }
        // If a Summon exists, remove it
        foreach (Transform Summon in Summons)
        {
            if (Summon != null)
            {
                Summon.gameObject.GetComponent<HealthSystem>().Die();
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

        int TankIndex = Array.IndexOf(EnemyMove, 4);
        // If failed to find Tank
        if (TankIndex == -1)
        {
            EnemyTarget = Enemies[0];
        }
        else
        {
            EnemyTarget = Enemies[TankIndex];
        }
    }

    void GetCardsInMoveQueue()
    {
        CardsInMoveQueue.Clear();

        Transform[] Cards = CardParent.GetComponentsInChildren<Transform>();

        foreach (Transform Card in Cards)
        {
            CardStats Stats = Card.GetComponent<CardStats>();

            // If in MoveQueue and is the card itself, not attached text
            if (Card.position.y == MoveQueueSnapPoint.position.y && Stats != null)
            {
                CardsInMoveQueue.Add(Card);

                // Set cooldown for each used card
                CardInventoryScript.Inventory.cardList[Stats.InventoryIndex].CardCooldown = Stats.Cooldown;
            }
        }
    }

    // Called when player has finished their turn, will play each card in the MoveQueue
    void PlayCards()
    {
        // Check if a Roid-Rage card is in the "queue"
        // Do that "move" first
        PlayerDamageBuff = 0;
        for (int i = 0; i < CardsInMoveQueue.Count; i++)
        {
            CardStats CardStatsScript = CardsInMoveQueue[i].gameObject.GetComponent<CardStats>();

            if (CardStatsScript.DamageBuff > 0)
            {
                PlayerDamageBuff += 2;
            }
        }

        // Do each cards "thing"
        for (int i = 0; i < CardsInMoveQueue.Count; i++)
        {
            CardStats CardStatsScript = CardsInMoveQueue[i].gameObject.GetComponent<CardStats>();

            // Cleave
            if (CardStatsScript.SplashDamage == true)
            {
                // If a tank is spawned
                if (EnemyType.Contains(4))
                {
                    int TankIndex = Array.IndexOf(EnemyType, 4);
                    
                    // Check how many enemies are alive
                    int AliveEnemies = 0;
                    foreach (Transform Enemy in Enemies)
                    {
                        if (Enemy != null)
                        {
                            AliveEnemies++;
                        }
                    }

                    // Make tank absorb the damage meant for each enemy
                    Enemies[TankIndex].GetComponent<HealthSystem>().TakeDamage((CardStatsScript.Damage + PlayerDamageBuff) * AliveEnemies);
                }
                else
                {
                    foreach (Transform Enemy in Enemies)
                    {
                        if (Enemy != null)
                        {
                            Enemy.GetComponent<HealthSystem>().TakeDamage(CardStatsScript.Damage + PlayerDamageBuff);
                        }
                    }
                }

                // Set damage buff to 0 when it has been used
                PlayerDamageBuff = 0;
            }
            // Shielded Charge
            else if (CardStatsScript.Defence > 0 && CardStatsScript.Damage > 0)
            {
                int Damage = CardStatsScript.Damage + PlayerDamageBuff;

                // Damage an enemy
                foreach (Transform Enemy in Enemies)
                {
                    if (Enemy != null)
                    {
                        Enemy.GetComponent<HealthSystem>().TakeDamage(Damage);
                    }
                }

                // Give block to self equal to damage dealt
                foreach (Transform Player in Players)
                {
                    if (Player.gameObject.activeSelf)
                    {
                        Player.GetComponent<HealthSystem>().AddDefence(Damage);
                    }
                }

                // Set damage buff to 0 when it has been used
                PlayerDamageBuff = 0;
            }
            // Block
            else if (CardStatsScript.Defence > 0)
            {
                foreach (Transform Player in Players)
                {
                    if (Player.gameObject.activeSelf)
                    {
                        Player.GetComponent<HealthSystem>().AddDefence(CardStatsScript.Defence);
                    }
                }
            }
            // Thorns
            else if (CardStatsScript.Thorns > 0)
            {
                foreach (Transform Player in Players)
                {
                    if (Player.gameObject.activeSelf)
                    {
                        Player.GetComponent<HealthSystem>().Thorns = CardStatsScript.Thorns;
                    }
                }
            }
            // Bash
            else if (CardStatsScript.Stun > 0)
            {
                // If a tank is spawned
                if (EnemyType.Contains(4))
                {
                    int TankIndex = Array.IndexOf(EnemyType, 4);

                    StunDuration[TankIndex] = CardStatsScript.Stun;
                    Enemies[TankIndex].GetComponent<EnemyAI>().Stun(CardStatsScript.Stun);
                }
                else
                {
                    for (int j = 0; j < Enemies.Length; j++)
                    {
                        if (Enemies[j] != null)
                        {
                            // Stun attacked enemy
                            StunDuration[j] = CardStatsScript.Stun;
                            Enemies[j].GetComponent<EnemyAI>().Stun(CardStatsScript.Stun);
                            break;
                        }
                    }
                }
            }
            // Roid-Rage
            else if (CardStatsScript.DamageBuff > 0)
            {
                // Do nothing since any Roid-Rage cards where done first in queue
            }
            // Slash
            else
            {
                // If a tank is spawned
                if (EnemyType.Contains(4))
                {
                    int TankIndex = Array.IndexOf(EnemyType, 4);

                    Enemies[TankIndex].GetComponent<HealthSystem>().TakeDamage(CardStatsScript.Damage + PlayerDamageBuff);
                }
                else
                {
                    foreach (Transform Enemy in Enemies)
                    {
                        if (Enemy != null)
                        {
                            Enemy.GetComponent<HealthSystem>().TakeDamage(CardStatsScript.Damage + PlayerDamageBuff);
                            break;
                        }
                    }
                }

                // Set damage buff to 0 when it has been used
                PlayerDamageBuff = 0;
            }
        }
    }

    // Called when its the enemies turn, they do stuff then
    void EnemyTurn()
    {
        // Check if a DamageBuff move is in the "queue"
        // Do said move first
        if (EnemyMove.Contains(4))
        {
            for (int i = 0; i < EnemyDamageBuff.Length; i++)
            {
                EnemyDamageBuff[i] = 2;
            }
        }

        // Do each move in EnemyMoves
        for (int i = 0; i < EnemyMove.Length; i++)
        {
            // If enemy exists and isnt stunned
            if (Enemies[i] != null && StunDuration[i] == 0)
            {
                // Do Cleave (Normal Splash)
                if (SplashDamage[i] == true)
                {
                    int Damage = Enemies[i].GetComponent<EnemyAI>().Damage;

                    foreach (Transform Player in Players)
                    {
                        if (Player.gameObject.activeSelf)
                        {
                            HealthSystem HealthScript = Player.GetComponent<HealthSystem>();

                            HealthScript.TakeDamage(Damage + EnemyDamageBuff[i]);

                            // If thorns, reflect damage back to attacker
                            if (HealthScript.Thorns > 0)
                            {
                                Enemies[i].GetComponent<HealthSystem>().TakeDamage(Damage + EnemyDamageBuff[i] * HealthScript.Thorns); // Thorns acts as a damage multiplier

                                // Reset Thorns when it has been used
                                HealthScript.Thorns = 0;
                            }
                        }
                    }

                    // Reset DamageBuff when it has been used
                    EnemyDamageBuff[i] = 0;
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
                                if (Player.gameObject.activeSelf)
                                {
                                    HealthSystem HealthScript = Player.GetComponent<HealthSystem>();

                                    HealthScript.TakeDamage(Damage + EnemyDamageBuff[i]);

                                    // If thorns, reflect damage back to attacker
                                    if (HealthScript.Thorns > 0)
                                    {
                                        Enemies[i].GetComponent<HealthSystem>().TakeDamage(Damage + EnemyDamageBuff[i] * HealthScript.Thorns); // Thorns acts as a damage multiplier

                                        // Reset Thorns when it has been used
                                        HealthScript.Thorns = 0;
                                    }

                                    // Reset DamageBuff when it has been used
                                    EnemyDamageBuff[i] = 0;
                                    break;
                                }
                            }
                            break;
                        // Block
                        case 2:
                            Enemies[i].GetComponent<HealthSystem>().AddDefence(Enemies[i].GetComponent<EnemyAI>().Defence);
                            break;
                        // Summon summons
                        case 3:
                            Summons = EnemySpawnerScript.SpawnSummons();
                            break;
                        // Buff each ally
                        case 4:
                            break;
                    }
                }
            }
            else
            {
                StunDuration[i]--;
            }
        }
    }
}
