using System;
using System.Collections;
using UnityEngine;

public class MapGen : MonoBehaviour
{
    // Fancy formating stuff for Inspector
    [Header("Map Room Stuff")]

    public Transform MapRoomPrefabParent;
    public GameObject MapRoomPrefab;

    [Space]
    [Header("Map Line Stuff")]

    public Transform MapLinePrefabParent;
    public GameObject MapLinePrefab;

    [Space]
    [Header("Arrays")]

    public Transform[] SpawnPoints;
    public Transform[] Rooms;
    public Transform[] Lines;

    [Header("Shit for other scripts")]
    public int CurrentRoom;
    public GameObject PreviousRoom;

    // Start is called before the first frame update
    void Start()
    {
        // Put parent + children into array
        SpawnPoints = GetComponentsInChildren<Transform>();

        StartCoroutine(CreateMap());
    }

    public void GenerateMapBtnPressed()
    {
        StartCoroutine(CreateMap());
    }

    // FatPerson115 saving my ass
    IEnumerator CreateMap()
    {
        DeleteMap();

        // yield on a new YieldInstruction that waits for 0.05 seconds.
        // 0.1f to tell stupid compiler its float, not double
        yield return new WaitForSeconds(0.05f);

        GenerateRooms();

        GeneratePaths();
    }

    // Function for deleting all elements of previously generated map
    void DeleteMap()
    {
        if (Rooms != null )
        {
            for (int i = 1; i < Rooms.Length; i++)
            {
                if (Rooms[i] != null)
                {
                    // Destroy object
                    Destroy(Rooms[i].gameObject);
                }
            }
        }

        if (Lines != null)
        {
            for (int i = 1; i < Lines.Length; i++)
            {
                if (Lines[i] != null)
                {
                    // Destroy object
                    Destroy(Lines[i].gameObject);
                }
            }
        }

        // Clear Arrays
        Rooms = null;
        Lines = null;

        CurrentRoom = 1;

        PreviousRoom = GameObject.Find("SpawnPoint Start");
    }

    // Function for generating rooms on map
    void GenerateRooms()
    {
        System.Random rand = new System.Random();

        // Slumpa mängden element per kolumn
        // Upper bounds is not inclusive, eg Next(1, 4) == 1, 2 or 3
        //int[] RoomCount = { rand.Next(1, 4), rand.Next(1, 4), rand.Next(1, 4), rand.Next(1, 4), rand.Next(1, 4) };
        int[] RoomCount = { 2, 2, 2, 2, 2 };

        // Bestäm vilken / vilka spawnpoints som ska bli rum
        int[] SpawnPoint = new int[SpawnPoints.Length];

        // Set SpawnPoints for StartRoom and End Room
        SpawnPoint[1] = 1;
        SpawnPoint[SpawnPoints.Length - 1] = 1;

        // Bestäm spawnpoints
        // 1 - 5 eftersom Column 1, 2, 3, 4 & 5
        for (int i = 1; i < 6; i++)
        {
            int temp2 = rand.Next(0, 3);
            switch (temp2)
            {
                case 0:
                    SpawnPoint[i * 3 - 1] = 1;
                    SpawnPoint[i * 3] = 1;
                    SpawnPoint[i * 3 + 1] = 0;
                    break;
                case 1:
                    SpawnPoint[i * 3 - 1] = 1;
                    SpawnPoint[i * 3] = 0;
                    SpawnPoint[i * 3 + 1] = 1;
                    break;
                case 2:
                    SpawnPoint[i * 3 - 1] = 0;
                    SpawnPoint[i * 3] = 1;
                    SpawnPoint[i * 3 + 1] = 1;
                    break;
            }
        }

        for (int i = 1; i < SpawnPoints.Length; i++)
        {
            if (SpawnPoint[i] == 1)
            {
                // https://docs.unity3d.com/ScriptReference/Object.Instantiate.html
                // Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent);
                GameObject Room = Instantiate(MapRoomPrefab, new Vector3(SpawnPoints[i].position.x, SpawnPoints[i].position.y, SpawnPoints[i].position.z), new Quaternion(0, 0, 0, 0), MapRoomPrefabParent);
                Room.name = SpawnPoints[i].name;
            }
        }

        // Put all clones + parent into Array
        Rooms = MapRoomPrefabParent.GetComponentsInChildren<Transform>();
    }

    // Function for generating paths between rooms
    void GeneratePaths()
    {
        // Amount of Lines, ignore index 0 as it is Parent
        int LineCount = 1;

        for (int i = 1; i < Rooms.Length; i++)
        {
            for (int j = 1; j < Rooms.Length; j++)
            {
                // Så att den inte kollar avstånd mellan objekt n och objekt n
                if (i != j)
                {
                    // Calc delta x & delta y
                    float dX = Math.Abs(Rooms[i].position.x - Rooms[j].position.x);
                    float dY = Math.Abs(Rooms[i].position.y - Rooms[j].position.y);

                    // Om de inte ligger övanför varandra och avståndet är mindre än roten ur 2, eg ~ 1.4
                    if (dX > 0 && dX <= Math.Sqrt(8) && dY <= Math.Sqrt(8))
                    {
                        // Clear array
                        Lines = null;

                        // Skapar en klon av ett tomt objekt som håller en LineRenderer, detta då varje LineRenderer bara kan ha en linje
                        Instantiate(MapLinePrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), MapLinePrefabParent);

                        // Put all clones + parent into Array
                        Lines = MapLinePrefabParent.GetComponentsInChildren<Transform>();

                        LineRenderer LineRend = Lines[LineCount].GetComponent<LineRenderer>();

                        // Start-position för linjen
                        LineRend.SetPosition(0, new Vector3(Rooms[i].position.x, Rooms[i].position.y, -1));

                        // Om 1:a eller 4:e kvadranten
                        if (Rooms[j].position.x > Rooms[i].position.x)
                        {
                            // Om 1:a kvadranten
                            if (Rooms[j].position.y > Rooms[i].position.y)
                            {
                                LineRend.SetPosition(1, new Vector3(Rooms[i].position.x + dX, Rooms[i].position.y + dY, -1));
                            }
                            // Om 4:e kvadranten
                            else
                            {
                                LineRend.SetPosition(1, new Vector3(Rooms[i].position.x + dX, Rooms[i].position.y - dY, -1));
                            }
                        }
                        // Om 2:a eller 3:e kvadranten
                        else
                        {
                            // Om 2:a kvadranten
                            if (Rooms[j].position.y > Rooms[i].position.y)
                            {
                                LineRend.SetPosition(1, new Vector3(Rooms[i].position.x - dX, Rooms[i].position.y + dY, -1));
                            }
                            // Om 3:e kvadranten
                            else
                            {
                                LineRend.SetPosition(1, new Vector3(Rooms[i].position.x - dX, Rooms[i].position.y - dY, -1));
                            }
                        }

                        LineCount++;
                    }
                }
            }
        }
    }

    public void UpdateOtherScriptShit(int CurrentRoomNumber, GameObject CurrentRoomObject)
    {
        CurrentRoom = CurrentRoomNumber;

        PreviousRoom = CurrentRoomObject;
    }
}
