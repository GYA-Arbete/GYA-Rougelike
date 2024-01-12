using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// Super basic script that holds different properties of ecah room
// This makes it so MapNavigation.cs can function from one script instead of one script per room
public class RoomProperties : MonoBehaviour
{
    [Header("Properties")]
    public int RoomID;
    public int RoomType;
    public bool HiddenType;

    [Header("Enemy stuff")]
    public int EnemyAmount;
    public int[] EnemyTypes;

    [Header("Children Objects")]
    public GameObject RoomImageObject;

    public void SetupRoom(int ID, int roomType, Texture RoomImage, GameObject ImagePrefab)
    {
        RoomID = ID;
        RoomType = roomType;

        RoomImageObject = Instantiate(ImagePrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), new Quaternion(0, 0, 0, 0), transform);
        RoomImageObject.name = transform.name;

        RawImage RawImg = RoomImageObject.GetComponent<RawImage>();
        RawImg.texture = RoomImage;

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
            System.Random Rand = new();

            // Slumpar ett tal mellan 1 till och med 4
            EnemyAmount = Rand.Next(1, 5);

            EnemyTypes = new int[EnemyAmount];

            for (int i = 0; i < EnemyAmount; i++)
            {
                // Check that it doesnt already contain a Summoner
                if (!EnemyTypes.Contains(3))
                {
                    // If a summoner hasnt been included, allow every enemy to be picked
                    EnemyTypes[i] = Rand.Next(1, 5);
                }
                else
                {
                    int[] AllowedEnemyTypes = { 1, 2, 4 };

                    EnemyTypes[i] = AllowedEnemyTypes[Rand.Next(0, AllowedEnemyTypes.Length)];
                }
            }
        }
    }
}
