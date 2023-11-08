using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int Damage;

    [Header("Other")]
    public int EnemyType;

    public Image EnemyMoveIndicatorImage;

    public Sprite[] MoveIndicators;

    public int Cooldown;

    public void SetupEnemy(int Enemytype, Sprite[] Images)
    {
        EnemyType = Enemytype;
        MoveIndicators = Images;

        switch (EnemyType)
        {
            case 0:
                break;
            case 1:
                Cooldown = 5;
                break;
        }
    }

    // Int for type of move, bool for if splash damage
    public (int, bool) GenerateMove()
    {
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
                break;
            // Summoner
            case 3:
                break;
            // Tank
            case 4:
                break;
            // Summon (Enemy spawned by Summoner)
            case 5:
                break;
        }
        return (0, true);
    }
}
