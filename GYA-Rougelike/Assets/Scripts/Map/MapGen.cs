using System;
using System.Collections;
using UnityEngine;

public class MapGen : MonoBehaviour
{
    public GameObject SpawnPointsParent;

    [Header("Room Stuff")]
    public Transform MapRoomPrefabParent;
    public GameObject MapRoomPrefab;

    [Header("Line Stuff")]
    public Transform MapLinePrefabParent;
    public GameObject MapLinePrefab;

    [Header("Arrays")]
    public Transform[] SpawnPoints;
    public Transform[] Rooms;
    public Transform[] Lines;

    [Header("Other Scripts")]
    public MapNavigation MapNavigationScript;

    // Start is called before the first frame update
    void Start()
    {
        // Get all SpawnPoints into an array
        SpawnPoints = SpawnPointsParent.GetComponentsInChildren<Transform>();
    }

    // FatPerson115 saving my ass
    public IEnumerator CreateMap()
    {
        DeleteMap();

        // yield on a new YieldInstruction that waits for 0.05 seconds
        yield return new WaitForSeconds(0.05f);

        GenerateRooms();

        GeneratePaths();

        MapNavigationScript.SetupForMapNav();
    }

    public void DeleteMap()
    {
        // If there are any rooms
        if (Rooms != null )
        {
            // Delete each room
            for (int i = 1; i < Rooms.Length; i++)
            {
                if (Rooms[i] != null)
                {
                    Destroy(Rooms[i].gameObject);
                }
            }
        }

        // If there are any lines
        if (Lines != null)
        {
            // Delete each line
            for (int i = 1; i < Lines.Length; i++)
            {
                if (Lines[i] != null)
                {
                    // Destroy object
                    Destroy(Lines[i].gameObject);
                }
            }
        }

        // Clear Lists
        Rooms = null;
        Lines = null;
    }

    void GenerateRooms()
    {
        System.Random rand = new System.Random();

        // Array for if each SpawnPoint has a room or not
        // 1 == Yes room, 0 == No room
        int[] SpawnPoint = new int[SpawnPoints.Length];

        // Set values so StartRoom and EndRoom always spawns
        SpawnPoint[1] = 1;
        SpawnPoint[SpawnPoints.Length - 1] = 1;

        // Best�m spawnpoints
        // 1 - 5 eftersom Column 1, 2, 3, 4 & 5
        for (int i = 1; i < 6; i++)
        {
            int temp = rand.Next(0, 3);
            switch (temp)
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

        // Spawn each room that is meant to be spawned
        for (int i = 1; i < SpawnPoints.Length; i++)
        {
            if (SpawnPoint[i] == 1)
            {
                // https://docs.unity3d.com/ScriptReference/Object.Instantiate.html
                // Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent);
                Instantiate(MapRoomPrefab, new Vector3(SpawnPoints[i].position.x, SpawnPoints[i].position.y, SpawnPoints[i].position.z), new Quaternion(0, 0, 0, 0), MapRoomPrefabParent);
            }
        }

        // Get all Rooms into Rooms list
        Rooms = MapRoomPrefabParent.GetComponentsInChildren<Transform>();
    }

    void GeneratePaths()
    {
        for (int i = 1; i < Rooms.Length; i++)
        {
            for (int j = 1; j < Rooms.Length; j++)
            {
                // S� att den inte kollar avst�nd mellan objekt n och objekt n
                if (i != j)
                {
                    // Calc delta x & delta y
                    float dX = Math.Abs(Rooms[i].position.x - Rooms[j].position.x);
                    float dY = Math.Abs(Rooms[i].position.y - Rooms[j].position.y);

                    // Calc distance
                    double Distance = Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));

                    // Om de inte ligger �vanf�r varandra och avst�ndet �r mindre �n roten ur 8
                    if (dX > 0 && Distance <= Math.Sqrt(8))
                    {
                        // Skapar en klon av ett tomt objekt som h�ller en LineRenderer, detta d� varje LineRenderer bara kan ha en linje
                        GameObject LineHolder = Instantiate(MapLinePrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), MapLinePrefabParent);

                        // Get the LineRenderer component of the newly created object
                        LineRenderer LineRend = LineHolder.GetComponent<LineRenderer>();

                        // Start-position for the Line
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
                    }
                }
            }
        }

        // Put all lines into an array
        Lines = MapLinePrefabParent.GetComponentsInChildren<Transform>();
    }
}
