using UnityEngine;
using UnityEngine.UI;

public class RoomTypeGen : MonoBehaviour
{
    [Header("Type Generation Stuff")]

    public static int RoomType;
    public int RoomTypeValue; // För att visa värdet av RoomType i inspector
    public bool HiddenType = false;

    [Header("Image Stuff")]

    public GameObject ImagePrefab;
    public Texture[] MapRoomIcons;

    [Header("Enemy stiff")]
    public int EnemyAmount;
    public int[] EnemyTypes;

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

        // If StartRoom, set room-type to StartRoom
        if (gameObject.name == "SpawnPoint Start")
        {
            RoomType = 0;
        }
        // If EndRoom, set room-type to BossRoom
        else if (gameObject.name == "SpawnPoint End")
        {
            RoomType = 4;
        }
        else
        {
            System.Random rand = new System.Random();

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

        // Create an overlayed image indicating room-type
        // https://docs.unity3d.com/ScriptReference/Object.Instantiate.html
        // Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent);
        // transform.position.n == transform component for object script is attached to
        // Sets parent as this object
        GameObject Image = Instantiate(ImagePrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), new Quaternion(0, 0, 0, 0), transform);
        // Same name as parent object
        Image.name = transform.name;

        Transform[] Children;
        Children = GetComponentsInChildren<Transform>();

        for (int i = 0; i < Children.Length; i++)
        {
            if (Children[i].GetComponent<RawImage>() != null && RoomType > 0)
            {
                if (HiddenType)
                {
                    RawImage RawImg = Children[i].GetComponent<RawImage>();
                    RawImg.texture = MapRoomIcons[5]; // 5 == HiddenTypeIcon
                }
                else
                {
                    RawImage RawImg = Children[i].GetComponent<RawImage>();
                    RawImg.texture = MapRoomIcons[RoomType];
                }
            }
            else if (Children[i].GetComponent<RawImage>() != null && RoomType == 0)
            {
                RawImage RawImg = Children[i].GetComponent<RawImage>();
                RawImg.texture = MapRoomIcons[0];
            }
        }

        // För att visa värdet av RoomType i inspector
        RoomTypeValue = RoomType;

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
                EnemyTypes[i] = Rand.Next(1, 5);
            }
        }
    }
}
