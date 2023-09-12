using UnityEngine;

public class RoomTypeGen : MonoBehaviour
{
    public int RoomType;

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

        4 == RandomRoom

        5 == BossRoom
         
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

            // Generates a random number 1 -> 4
            // Flat 1/4 chance for each room-type
            RoomType = rand.Next(1, 5);
        }
    }
}
