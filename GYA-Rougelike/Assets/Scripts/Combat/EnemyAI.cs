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
    public Dictionary<int, bool> GenerateMove()
    {
        Dictionary<int, bool> ReturnVal = new();

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
                    ReturnVal.Add(0, true);
                    break;
                }
                // Normal Attack
                else
                {
                    Cooldown--;

                    EnemyMoveIndicatorImage.sprite = MoveIndicators[1];
                    ReturnVal.Add(1, false);
                    break;
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
        return ReturnVal;
    }
}
