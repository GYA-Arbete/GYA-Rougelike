using JetBrains.Annotations;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// Super basic script that holds different properties of ecah room
// This makes it so MapNavigation.cs can function from one script instead of one script per room
public class RoomProperties : MonoBehaviour
{
    [Header("Properties")]
    public int RoomID;
    // 0 == StartRoom, 1 == EnemyRoom, 2 == LootRoom, 3 == CampRoom, 4 == BossRoom
    public int RoomType;
    public bool HiddenType;

    [Header("Enemy stuff")]
    public int EnemyAmount;
    public int[] EnemyTypes;

    [Header("Children Objects")]
    public GameObject RoomImage;

    public void GenerateProperties(int ID, Texture[] RoomImages, GameObject ImagePrefab)
    {
        RoomID = ID;

        // If StartRoom
        if (ID == 0)
        {
            RoomType = 0;
        }
        // If EndRoom
        else if (ID == 11)
        {
            RoomType = 4;
        }
        else
        {
            System.Random rand = new();

            // Generates a random number 1 -> 3
            // Flat 1/3 chance for each room-type
            RoomType = rand.Next(1, 4);

            // 20% Chans att det är HiddenType
            int temp = rand.Next(0, 6);
            if (temp == 0)
            {
                HiddenType = true;
            }
        }

        RoomImage = Instantiate(ImagePrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), new Quaternion(0, 0, 0, 0), transform);

        // Same name as parent object
        RoomImage.name = transform.name;

        if (HiddenType)
        {
            RawImage RawImg = RoomImage.gameObject.GetComponent<RawImage>();
            RawImg.texture = RoomImages[5]; // 5 == HiddenTypeIcon
        }
        else
        {
            RawImage RawImg = RoomImage.gameObject.GetComponent<RawImage>();
            RawImg.texture = RoomImages[RoomType];
        }

        GenerateEnemies();
    }

    void GenerateEnemies()
    {
        if (RoomType == 4)
        {
            EnemyAmount = 1;
            EnemyTypes = new int[1] { 0 };
        }
        else if (RoomType == 1)
        {
            System.Random Rand = new System.Random();
            // Slumpar ett tal mellan 1 till och med 4
            EnemyAmount = Rand.Next(1, 5);

            EnemyTypes = new int[EnemyAmount];
            for (int i = 0; i < EnemyAmount; i++)
            {
                //EnemyTypes[i] = Rand.Next(1, 5);

                // Check that it doesnt already contain a Summoner
                if (!EnemyTypes.Contains(3))
                {
                    // Temporary code to only spawn implemented enemies
                    int[] AllowedEnemyTypes = { 1, 2, 3, 4 };

                    EnemyTypes[i] = AllowedEnemyTypes[Rand.Next(0, AllowedEnemyTypes.Length)];
                }
                else
                {
                    // Temporary code to only spawn implemented enemies
                    int[] AllowedEnemyTypes = { 1, 2, 4 };

                    EnemyTypes[i] = AllowedEnemyTypes[Rand.Next(0, AllowedEnemyTypes.Length)];
                }
            }
        }
    }
}
