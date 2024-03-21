using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : NetworkBehaviour
{
    [Header("Enemy Stats")]
    public int Damage;
    public int Defence = 0;

    [Header("Stun Stuff")]
    public bool Stunned = false;
    public int StunDuration = 0;
    public GameObject StunIndicator;

    [Header("Other")]
    public int EnemyType = -1;

    public Image EnemyMoveIndicatorImage;
    public TextMeshProUGUI EnemyMoveIndicatorText;

    public int Cooldown;

    [Server]
    public void SetupEnemy(int Enemytype)
    {
        EnemyType = Enemytype;

        StunIndicatorVisibility(false);

        switch (EnemyType)
        {
            case 0:
                Cooldown = 4;
                break;
            case 1:
                Cooldown = 5;
                break;
            case 2:
                Cooldown = 4;
                break;
            case 3:
                Cooldown = 5;
                break;
            case 4:
                Cooldown = 5;
                break;
        }
    }

    [Command(requiresAuthority = false)]
    public void Stun(int Duration)
    {
        Stunned = true;
        StunDuration = Duration;
        StunIndicatorVisibility(true);
    }

    // Int for type of move, bool for if splash damage
    [Server]
    public (int, bool) GenerateMove()
    {
        // If the enemy is stunned, do an early exit and send back an "out of range index" to it doesnt do anything later
        if (Stunned)
        {
            // If StunDuration is 0, dont return a "stunned" val but instead do move generation as usual
            if (StunDuration == 0)
            {
                Stunned = false;
                StunIndicatorVisibility(false);
            }
            else
            {
                StunDuration--;
                return (-1, false);
            }
        }

        Defence = 0;

        switch (EnemyType)
        {
            // Boss
            case 0:
                if (Cooldown == 0)
                {
                    Cooldown = 4;

                    System.Random Rand = new();
                    Defence = Rand.Next(3, 6); // Picks a random number between 3 and 5

                    SetMoveIndicator(2, $"{Defence}");
                    return (2, false);
                }
                // Normal Attack
                else
                {
                    Cooldown--;

                    SetMoveIndicator(1, $"{Damage}");
                    return (1, false);
                }
            // Basic Grunt
            case 1:
                // Splash Attack
                if (Cooldown == 0)
                {
                    Cooldown = 5;

                    SetMoveIndicator(0, $"{Damage}");
                    return (0, true);
                }
                // Normal Attack
                else
                {
                    Cooldown--;

                    SetMoveIndicator(1, $"{Damage}");
                    return (1, false);
                }
            // Buff / Debuff
            case 2:
                // Buff every ally
                if (Cooldown == 0)
                {
                    Cooldown = 4;

                    SetMoveIndicator(3, $"+2");
                    return (4, false);
                }
                // Normal Attack
                else
                {
                    Cooldown--;

                    SetMoveIndicator(1, $"{Damage}");
                    return (1, false);
                }
            // Summoner
            case 3:
                // Summon summons
                if (Cooldown == 0)
                {
                    Cooldown = 5;

                    SetMoveIndicator(4, $"{FindAnyObjectByType<CombatSystem>().Summons.Length}x");
                    return (3, false);
                }
                // Normal Attack
                else
                {
                    Cooldown--;

                    SetMoveIndicator(1, $"{Damage}");
                    return (1, false);
                }
            // Tank
            case 4:
                // Block
                if (Cooldown == 0)
                {
                    Cooldown = 5;

                    System.Random Rand = new();
                    Defence = Rand.Next(2, 5); // Picks a random number between 2 and 4

                    SetMoveIndicator(2, $"{Defence}");
                    return (2, false);
                }
                // Normal Attack
                else
                {
                    Cooldown--;

                    SetMoveIndicator(1, $"{Damage}");
                    return (1, false);
                }
            // Summon (Enemy spawned by Summoner)
            case 5:
                // Only has normal attack
                SetMoveIndicator(1, $"{Damage}");
                return (1, false);
        }
        return (0, true);
    }

    [ClientRpc]
    void SetMoveIndicator(int ImageIndex, string EffectText)
    {
        EnemyMoveIndicatorImage.sprite = FindAnyObjectByType<CombatSystem>().MoveIndicators[ImageIndex];
        EnemyMoveIndicatorText.text = EffectText;
    }

    [ClientRpc]
    void StunIndicatorVisibility(bool Visibility)
    {
        StunIndicator.SetActive(Visibility);
    }
}
