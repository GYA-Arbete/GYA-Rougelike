using UnityEngine;

public class RoomTypeGen : MonoBehaviour
{
    public int RoomType;
    public bool HiddenType = false;

    // Start is called before the first frame update
    void Start()
    {
        // When object is created, generate the room type for that room
        GenerateRoomType();
    }

    void GenerateRoomType()
    {
        /*
        0 == StartRoom

        1 == EnemyRoom

        2 == LootRoom

        3 == CampRoom

        4 == BossRoom
        */

        // ##########################
        // Set Picture for Room-Type

        // If StartRoom, set room-type to StartRoom
        if (gameObject.name == "SpawnPoint Start")
        {
            RoomType = 0;
        }
        // If EndRoom, set room-type to BossRoom
        else if (gameObject.name == "SpawnPoint End")
        {
            RoomType = 5;
        }
        else
        {
            System.Random rand = new System.Random();

            // Generates a random number 1 -> 3
            // Flat 1/3 chance for each room-type
            RoomType = rand.Next(1, 4);

            // 20% Chans att det är HiddenType
            int temp = rand.Next(1, 6);
            if (temp == 0)
            {
                HiddenType = true;
            }
        }
    }
}
