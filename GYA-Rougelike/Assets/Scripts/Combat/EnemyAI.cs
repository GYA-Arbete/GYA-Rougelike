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

    public void SetupEnemy(int Enemytype, Sprite[] Images)
    {
        EnemyType = Enemytype;
        MoveIndicators = Images;
    }

    public int GenerateMove()
    {
        System.Random Rand = new();

        switch (EnemyType)
        {
            // Boss
            case 0:
                break;
            // Basic Grunt
            case 1:
                int Move = Rand.Next(0, 6);
                // 20% Defence
                if (Move == 0)
                {
                    EnemyMoveIndicatorImage.sprite = MoveIndicators[0];
                    return 0;
                }
                // 80% Attack
                else
                {
                    EnemyMoveIndicatorImage.sprite = MoveIndicators[1];
                    return 1;
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
        return 0;
    }
}
