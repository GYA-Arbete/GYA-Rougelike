using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int Damage;
    public int Defence = 0;

    [Header("Stun Stuff")]
    public bool Stunned = false;
    public int StunDuration = 0;
    public GameObject StunIndicator;

    [Header("Other")]
    public int EnemyType;

    public Image EnemyMoveIndicatorImage;

    public Sprite[] MoveIndicators;

    public int Cooldown;

    public void SetupEnemy(int Enemytype, Sprite[] Images)
    {
        EnemyType = Enemytype;
        MoveIndicators = Images;

        StunIndicator.SetActive(false);

        switch (EnemyType)
        {
            case 0:
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

    public void Stun(int Duration)
    {
        Stunned = true;
        StunDuration = Duration;
        StunIndicator.SetActive(true);
    }

    // Int for type of move, bool for if splash damage
    public (int, bool) GenerateMove()
    {
        // If the enemy is stunned, do an early exit and send back an "out of range index" to it doesnt do anything later
        if (Stunned)
        {
            // If StunDuration is 0, dont return a "stunned" val but instead do move generation as usual
            if (StunDuration == 0)
            {
                Stunned = false;
                StunIndicator.SetActive(false);
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
                break;
            // Basic Grunt
            case 1:
                // Splash Attack
                if (Cooldown == 0)
                {
                    Cooldown = 5;

                    EnemyMoveIndicatorImage.sprite = MoveIndicators[0];
                    return (0, true);
                }
                // Normal Attack
                else
                {
                    Cooldown--;

                    EnemyMoveIndicatorImage.sprite = MoveIndicators[1];
                    return (1, false);
                }
            // Buff / Debuff
            case 2:
                // Buff every ally
                if (Cooldown == 0)
                {
                    Cooldown = 4;

                    //EnemyMoveIndicatorImage.sprite = MoveIndicators[] // MISSING CORRECT IMAGE
                    return (4, false);
                }
                // Normal Attack
                else
                {
                    Cooldown--;

                    EnemyMoveIndicatorImage.sprite = MoveIndicators[1];
                    return (1, false);
                }
            // Summoner
            case 3:
                // Summon summons
                if (Cooldown == 0)
                {
                    Cooldown = 5;

                    //EnemyMoveIndicatorImage.sprite = MoveIndicators[] // MISSING CORRECT IMAGE
                    return (3, false);
                }
                // Normal Attack
                else
                {
                    Cooldown--;

                    EnemyMoveIndicatorImage.sprite = MoveIndicators[1];
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

                    EnemyMoveIndicatorImage.sprite = MoveIndicators[2];
                    return (2, false);
                }
                // Normal Attack
                else
                {
                    Cooldown--;

                    EnemyMoveIndicatorImage.sprite = MoveIndicators[1];
                    return (1, false);
                }
            // Summon (Enemy spawned by Summoner)
            case 5:
                // Only has normal attack
                EnemyMoveIndicatorImage.sprite = MoveIndicators[1];
                return (1, false);
        }
        return (0, true);
    }
}
