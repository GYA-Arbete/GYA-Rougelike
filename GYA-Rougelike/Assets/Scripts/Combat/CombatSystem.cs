using Mirror;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CombatSystem : NetworkBehaviour
{
    [SyncVar]
    public bool InCombat = false;
    [SyncVar]
    public int FinishedPlayers = 0;

    [Header("Buttons")]
    public Button EndTurnButton;

    [Header("Combat Participants")]
    public Transform[] Players;
    public Transform[] Enemies;
    public int[] EnemyType;
    public Transform[] Summons;
    public bool[] DeadEnemies;

    [Header("CombatRoom-Exclusive Elements")]
    public GameObject[] CombatRoomElements;
    public GameObject WaitingText;

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
    }

    [Command(requiresAuthority = false)]
    public void StartCombat(int EnemyAmount, int[] EnemyTypes)
    {
        InCombat = true;

        ToggleCombatElementsVisibility();

        HideWaitingText();

        EnableEndTurnButtons();

        ResetFinishedPlayers();

        Enemies = EnemySpawnerScript.SpawnEnemies(EnemyAmount, EnemyTypes);

        CardSpawnerScript.ResetCards();

        EnemyMove = new int[EnemyAmount];
        SplashDamage = new bool[EnemyAmount];
        EnemyDamageBuff = new int[EnemyAmount];
        StunDuration = new int[EnemyAmount];
        DeadEnemies = new bool[EnemyAmount];

        EnemyType = EnemyTypes;

        SetupEnergyBar();

        CameraSwitchScript.SetViewToRoom();

        // Generate each enemies next turn
        for (int i = 0; i < EnemyAmount; i++) 
        {
            EnemyAI EnemyAIScript = Enemies[i].gameObject.GetComponent<EnemyAI>();

            EnemyAIScript.SetupEnemy(EnemyTypes[i]);

            // Get each enemies move
            var ReturnedValues = EnemyAIScript.GenerateMove();
            EnemyMove[i] = ReturnedValues.Item1;
            SplashDamage[i] = ReturnedValues.Item2;

            // Set default value
            EnemyDamageBuff[i] = 0;
            StunDuration[i] = 0;
        }

        // Save which enemy will be targeted by the players card
        UpdateEnemyTarget();
    }

    [ClientRpc]
    void SetEnemyTarget(Transform Enemy)
    {
        EnemyTarget = Enemy;
    }

    [ClientRpc]
    void SetupEnergyBar()
    {
        EnergyBarScript.SetupBar(3, new Color32(252, 206, 82, 255));
    }

    [Command(requiresAuthority = false)]
    public void EndCombat(bool FromPauseMenu)
    {
        InCombat = false;

        ToggleCombatElementsVisibility();

        HideWaitingText();

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

        CardSpawnerScript.RpcDespawnCards();

        if (!FromPauseMenu)
        {
            // Exit the room
            CameraSwitchScript.SetViewToMap();
        }   
    }

    [ClientRpc]
    void ToggleCombatElementsVisibility()
    {
        foreach (GameObject Element in CombatRoomElements)
        {
            Element.SetActive(!Element.activeSelf);
        }
    }

    void EndTurn()
    {
        GetCardsInMoveQueue();

        PlayCards();

        CardSpawnerScript.DespawnCards();

        // Set info for TargetIndicator
        UpdateEnemyTarget();
    }

    [Command(requiresAuthority = false)]
    void CheckCombatEnded()
    {
        // Check if all Enemies are dead, eg combat has ended
        int DeadEnemiesAmount = 0;

        for (int i = 0; i < Enemies.Length; i++)
        {
            // If enemy is null, it is dead
            if (DeadEnemies[i])
            {
                DeadEnemiesAmount++;
            }
        }
        if (DeadEnemiesAmount == Enemies.Length)
        {
            EndCombat(false);
            return;
        }
    }

    [Command(requiresAuthority = false)]
    void ContinueEndTurn()
    {
        // If has exited combat dont do enemy turn logic
        if (!InCombat)
        {
            return;
        }

        EnemyTurn();

        // When both players have finished their turn, spawn the "new" cards
        CardSpawnerScript.ResetCards();

        // Set back energy to max
        ResetEnergyBar();

        EnableEndTurnButtons();

        HideWaitingText();

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

        // Set info for TargetIndicator
        UpdateEnemyTarget();
    }

    [Command(requiresAuthority = false)]
    void UpdateEnemyTarget()
    {
        int TankIndex = Array.IndexOf(EnemyMove, 4);
        // If failed to find Tank
        if (TankIndex == -1)
        {
            // Set target to first alive enemy
            for (int i = 0; i < Enemies.Length; i++)
            {
                if (!DeadEnemies[i])
                {
                    SetEnemyTarget(Enemies[i]);
                    break;
                }
            }
        }
        else
        {
            SetEnemyTarget(Enemies[TankIndex]);
        }
    }

    [ClientRpc]
    void ResetEnergyBar()
    {
        EnergyBarScript.ResetBar();
    }

    [Client]
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
    [Client]
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
                Attack(CardStatsScript.SplashDamage, CardStatsScript.Damage);
            }
            // Shielded Charge
            else if (CardStatsScript.Defence > 0 && CardStatsScript.Damage > 0)
            {
                // Give block to self equal to damage dealt
                foreach (Transform Player in Players)
                {
                    if (Player.gameObject.activeSelf)
                    {
                        Player.GetComponent<HealthSystem>().AddDefence(CardStatsScript.Damage + PlayerDamageBuff);
                    }
                }

                Attack(CardStatsScript.SplashDamage, CardStatsScript.Damage);
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
                        Player.GetComponent<HealthSystem>().AddThorns(CardStatsScript.Thorns);
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
                Attack(CardStatsScript.SplashDamage, CardStatsScript.Damage);
            }
        }

        // When players moves has been done, check if combat has ended
        CheckCombatEnded();

        // Check if the other player is finished
        if (FinishedPlayers == 1)
        {
            ResetFinishedPlayers();
            ContinueEndTurn();
        }
        else
        {
            EndTurnButton.interactable = false;
            WaitingText.SetActive(true);
            // Suboptimal since theoreticly we can get stuck here if both finish at exactly the same time with no way to end combat
            UpdateFinishedPlayers();
        }
    }

    [Command(requiresAuthority = false)]
    void Attack(bool SplashDamage, int Damage)
    {
        // If a tank is spawned & not dead, attack it first
        if (EnemyType.Contains(4) && Enemies[Array.IndexOf(EnemyType, 4)] != null)
        {
            int TankIndex = Array.IndexOf(EnemyType, 4);
            int AliveEnemies = 0;

            if (SplashDamage)
            {
                // Check how many enemies are alive
                for (int i = 0; i < Enemies.Length; i++)
                {
                    if (!DeadEnemies[i])
                    {
                        AliveEnemies++;
                    }
                }
            }
            else
            {
                // If not SplashDamage, set AliveEnemies to 1 as its only meant to attack one enemy (basicly nullifying the multiplier)
                AliveEnemies = 1;
            }

            // Make tank absorb the damage meant for enemy
            DeadEnemies[TankIndex] = Enemies[TankIndex].GetComponent<HealthSystem>().TakeDamage((Damage + PlayerDamageBuff) * AliveEnemies);
        }
        else
        {
            for (int i = 0; i < Enemies.Length; i++)
            {
                if (!DeadEnemies[i])
                {
                    DeadEnemies[i] = Enemies[i].GetComponent<HealthSystem>().TakeDamage(Damage + PlayerDamageBuff);

                    if (!SplashDamage)
                    {
                        // If not SplashDamage, break loop since it should only attack first enemy
                        break;
                    }
                }
            }
        }

        PlayerDamageBuff = 0;
    }

    [Command(requiresAuthority = false)]
    void UpdateFinishedPlayers()
    {
        FinishedPlayers++;
    }

    [Command(requiresAuthority = false)]
    void ResetFinishedPlayers()
    {
        FinishedPlayers = 0;
    }

    [ClientRpc]
    void EnableEndTurnButtons()
    {
        EndTurnButton.interactable = true;
    }

    [ClientRpc]
    void HideWaitingText()
    {
        WaitingText.SetActive(false);
    }

    // Called when its the enemies turn, they do stuff then
    [Command(requiresAuthority = false)]
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
