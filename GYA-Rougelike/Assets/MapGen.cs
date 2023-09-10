using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class MapGen : MonoBehaviour
{
    // Fancy formating stuff for Inspector
    [Header("Buttons")]

    public Button GenerateButton;

    [Space]
    [Header("Prefab")]

    public Transform MapPrefabParent;
    public GameObject MapPrefab;

    [Space]
    [Header("Arrays")]

    public Transform[] children;
    public Transform[] Clones;

    // Start is called before the first frame update
    void Start()
    {
        GenerateButton.onClick.AddListener(GenerateMapBtnPressed);

        // Put parent + children into array
        children = GetComponentsInChildren<Transform>();
    }

    void GenerateMapBtnPressed()
    {
        DeleteMap();

        GenerateRooms();

        GeneratePaths();
    }

    // Function for deleting all elements of previously generated map
    void DeleteMap()
    {
        // Put all clones + parent into Array
        Clones = MapPrefabParent.GetComponentsInChildren<Transform>();
        for (int i = 0; i < Clones.Length; i++)
        {
            // Ignore the first 2, eg Parent and Template
            if (i > 1)
            {
                // Destroy object
                Destroy(Clones[i].gameObject);
            }
        }

        // Clear array
        Clones = null;
    }

    // Function for generating rooms on map
    void GenerateRooms()
    {
        System.Random rand = new System.Random();

        // Slumpa mängden element per kolumn
        // Upper bounds is not inclusive, eg Next(1, 4) == 1, 2 or 3
        int[] RoomCount = { rand.Next(1, 4), rand.Next(1, 4), rand.Next(1, 4), rand.Next(1, 4), rand.Next(1, 4) };

        // Bestäm vilken / vilka spawnpoints som ska bli rum
        int[] SpawnPoint = new int[children.Length];

        // Set SpawnPoints for StartRoom and End Room
        SpawnPoint[1] = 1;
        SpawnPoint[children.Length - 1] = 1;

        // Bestäm spawnpoints
        for (int i = 0; i < 5; i++)
        {
            switch (RoomCount[i])
            {
                case 1:
                    int temp1 = rand.Next(0, 3);
                    switch (temp1)
                    {
                        case 0:
                            SpawnPoint[(i + 1) * 3 - 1] = 1;
                            SpawnPoint[(i + 1) * 3] = 0;
                            SpawnPoint[(i + 1) * 3 + 1] = 0;
                            break;
                        case 1:
                            SpawnPoint[(i + 1) * 3 - 1] = 0;
                            SpawnPoint[(i + 1) * 3] = 1;
                            SpawnPoint[(i + 1) * 3 + 1] = 0;
                            break;
                        case 2:
                            SpawnPoint[(i + 1) * 3 - 1] = 0;
                            SpawnPoint[(i + 1) * 3] = 0;
                            SpawnPoint[(i + 1) * 3 + 1] = 1;
                            break;
                    }
                    break;
                case 2:
                    int temp2 = rand.Next(0, 3);
                    switch (temp2)
                    {
                        case 0:
                            SpawnPoint[(i + 1) * 3 - 1] = 1;
                            SpawnPoint[(i + 1) * 3] = 1;
                            SpawnPoint[(i + 1) * 3 + 1] = 0;
                            break;
                        case 1:
                            SpawnPoint[(i + 1) * 3 - 1] = 1;
                            SpawnPoint[(i + 1) * 3] = 0;
                            SpawnPoint[(i + 1) * 3 + 1] = 1;
                            break;
                        case 2:
                            SpawnPoint[(i + 1) * 3 - 1] = 0;
                            SpawnPoint[(i + 1) * 3] = 1;
                            SpawnPoint[(i + 1) * 3 + 1] = 1;
                            break;
                    }
                    break;
                case 3:
                    SpawnPoint[(i + 1) * 3 - 1] = 1;
                    SpawnPoint[(i + 1) * 3] = 1;
                    SpawnPoint[(i + 1) * 3 + 1] = 1;
                    break;
            }
        }

        for (int i = 1; i < children.Length; i++)
        {
            if (SpawnPoint[i] == 1)
            {
                // https://docs.unity3d.com/ScriptReference/Object.Instantiate.html
                // Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent);
                Instantiate(MapPrefab, new Vector3(children[i].position.x, children[i].position.y, children[i].position.z), new Quaternion(0, 0, 0, 0), MapPrefabParent);
            }
        }
    }

    // Function for generating paths between rooms
    void GeneratePaths()
    {

    }
}
