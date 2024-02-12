using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen : NetworkBehaviour
{
    public GameObject SpawnPointsParent;

    [Header("Room Stuff")]
    public Transform MapRoomPrefabParent;
    public GameObject MapRoomPrefab;

    [Header("Line Stuff")]
    public Transform MapLinePrefabParent;
    public GameObject MapLinePrefab;
    public List<KeyValuePair<Vector3, Vector3>> SpawnedLines = new();

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
    [Client]
    public IEnumerator CreateMap()
    {
        DeleteMap();

        // Wait for 0.05 seconds cause it works then, unity dumb ig
        yield return new WaitForSeconds(0.05f);

        GenerateRooms();

        CalculatePaths();

        // Wait for 0.05 seconds cause otherwise Rooms will be null when required, unity dumb ig
        yield return new WaitForSeconds(0.05f);

        MapNavigationScript.SetupForMapNav(SpawnedLines, Rooms);
    }

    [Command(requiresAuthority=false)]
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
                    NetworkServer.Destroy(Rooms[i].gameObject);
                }
            }
        }

        // If there are any lines
        if (Lines != null)
        {
            // Delete each line
            for (int i = 0; i < Lines.Length; i++)
            {
                if (Lines[i] != null)
                {
                    // Destroy object
                    Destroy(Lines[i].gameObject);
                    NetworkServer.Destroy(Lines[i].gameObject);
                }
            }
        }

        // Clear Lists
        Rooms = null;
        Lines = null;
        SpawnedLines = new();
    }

    [Command(requiresAuthority=false)]
    void GenerateRooms()
    {
        System.Random rand = new();

        // Array for if each SpawnPoint has a room or not
        // 1 == Yes room, 0 == No room
        int[] SpawnPoint = new int[SpawnPoints.Length];

        // Set values so StartRoom and EndRoom always spawns
        SpawnPoint[1] = 1;
        SpawnPoint[SpawnPoints.Length - 1] = 1;

        List<int> AllowedCases = new() { 0, 1, 2 };
        int LastCase = -1; // Set to an invalid case before start
        // Bestäm spawnpoints
        // 1 - 5 eftersom Column 1, 2, 3, 4 & 5
        // Column 0 & 6 are start and end, not randomly generater layouts
        for (int i = 1; i < 6; i++)
        {
            int ChoosenCase = rand.Next(0, AllowedCases.Count);
            switch (AllowedCases[ChoosenCase])
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
            // If the same case has been picked twice in a row, dont allow it to be picked next time
            // This is done to not have the same case picked 3 or more times in a row
            if (LastCase == ChoosenCase)
            {
                // Reset AllowedCases
                AllowedCases = new List<int> { 0, 1, 2 };
                AllowedCases.RemoveAt(ChoosenCase);
            }
            else
            {
                LastCase = ChoosenCase;
            }
        }

        // Spawn each room that is meant to be spawned
        for (int i = 1; i < SpawnPoints.Length; i++)
        {
            if (SpawnPoint[i] == 1)
            {
                GameObject Room = Instantiate(MapRoomPrefab, new Vector3(SpawnPoints[i].position.x, SpawnPoints[i].position.y, SpawnPoints[i].position.z), new Quaternion(0, 0, 0, 0), MapRoomPrefabParent);
                NetworkServer.Spawn(Room);
                SetItemParent(Room, MapRoomPrefabParent);
            }
        }

        // Get all Rooms into Rooms list
        Rooms = MapRoomPrefabParent.GetComponentsInChildren<Transform>();
    }

    // Function for calculating every path that is within spec
    [Command(requiresAuthority=false)]
    void CalculatePaths()
    {
        List<KeyValuePair<Vector3, Vector3>> LineEndPoints = new();

        Vector3 StartPoint;
        Vector3 EndPoint;

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

                    // Calc distance
                    double Distance = Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));

                    // Om de inte ligger övanför varandra och avståndet är mindre än roten ur 8
                    if (dX > 0 && Distance <= Math.Sqrt(8))
                    {
                        // Om 1:a eller 4:e kvadranten
                        if (Rooms[j].position.x > Rooms[i].position.x)
                        {
                            // Start-position for the Line
                            StartPoint = new(Rooms[i].position.x, Rooms[i].position.y, -1);

                            // Om 1:a kvadranten
                            if (Rooms[j].position.y > Rooms[i].position.y)
                            {
                                EndPoint = new(Rooms[i].position.x + dX, Rooms[i].position.y + dY, -1);
                            }
                            // Om 4:e kvadranten
                            else
                            {
                                EndPoint = new(Rooms[i].position.x + dX, Rooms[i].position.y - dY, -1);
                            }
                        }
                        // Om 2:a eller 3:e kvadranten
                        else
                        {
                            // Start-position for the Line
                            StartPoint = new(Rooms[i].position.x, Rooms[i].position.y, -1);

                            // Om 2:a kvadranten
                            if (Rooms[j].position.y > Rooms[i].position.y)
                            {
                                EndPoint = new(Rooms[i].position.x - dX, Rooms[i].position.y + dY, -1);
                            }
                            // Om 3:e kvadranten
                            else
                            {
                                EndPoint = new(Rooms[i].position.x - dX, Rooms[i].position.y - dY, -1);
                            }
                        }

                        LineEndPoints.Add(new KeyValuePair<Vector3, Vector3>(StartPoint, EndPoint));
                    }
                }
            }
        }

        List<Transform> TempLineHolder = new();

        // Randomize order of List
        LineEndPoints = ShuffleList(LineEndPoints);

        // Foreach EndPoints pair
        for (int i = 0; i < LineEndPoints.Count; i++)
        {
            Vector3 StartPos = LineEndPoints[i].Key;
            Vector3 EndPos = LineEndPoints[i].Value;

            // #####################################################
            // Logic to check if lines are duplicate or are crossing
            // #####################################################

            // Dumb shit to compare the current LineRend positions to each spawned LineRend
            //KeyValuePair<Vector3, Vector3> ToSpawnEndPoints = new(EndPos, StartPos);

            bool DuplicateLine = false;

            foreach (KeyValuePair<Vector3, Vector3> EndPoints in SpawnedLines)
            {
                // Check if the line has already been spawned by inverting positions on the new LineRend and referencing it to each spawned line
                // Also check with the "non-inverted" positions to cover both cases
                if ((EndPoints.Key == EndPos && EndPoints.Value == StartPos) || (EndPoints.Key == StartPos && EndPoints.Value == EndPos))
                {
                    DuplicateLine = true;

                    break;
                }

                // Calc all vals
                int k1 = ((int)EndPoints.Key.y - (int)EndPoints.Value.y) / ((int)EndPoints.Key.x - (int)EndPoints.Value.x);
                int k2 = ((int)EndPos.y - (int)StartPos.y) / ((int)EndPos.x - (int)StartPos.x);

                int x1old = (int)EndPoints.Key.x;
                int x2old = (int)EndPoints.Value.x;
                int x1new = (int)StartPos.x;
                int x2new = (int)EndPos.x;

                int dy1 = Math.Abs((int)EndPoints.Key.y - (int)StartPos.y);
                int dy2 = Math.Abs((int)EndPoints.Value.y - (int)EndPos.y);

                // Check if the line is intersecting an existing line (See "Line Cross Calc.png" for more info on how / why)
                if (k1 != 0 && k1 == -k2 && x1old == x1new && x2old == x2new && dy1 == 2 && dy2 == 2)
                {
                    DuplicateLine = true;

                    break;
                }

                // Re-calculate some values for the "flipped" positions
                x1new = (int)EndPos.x;
                x2new = (int)StartPos.x;

                dy1 = Math.Abs((int)EndPoints.Key.y - (int)EndPos.y);
                dy2 = Math.Abs((int)EndPoints.Value.y - (int)StartPos.y);

                // Check again if the line is intersecting an existing line (See "Line Cross Calc.png" for more info on how / why)
                if (k1 != 0 && k1 == -k2 && x1old == x1new && x2old == x2new && dy1 == 2 && dy2 == 2)
                {
                    DuplicateLine = true;

                    break;
                }
            }

            if (!DuplicateLine)
            {
                GameObject LineHolder = Instantiate(MapLinePrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), MapLinePrefabParent);

                TempLineHolder.Add(LineHolder.transform);

                SpawnedLines.Add(new(StartPos, EndPos));

                NetworkServer.Spawn(LineHolder);

                // Get the LineRenderer component of the newly created object
                LineRenderer LineRend = LineHolder.GetComponent<LineRenderer>();

                // Set endpoints for the created LineRend
                LineRend.SetPosition(0, StartPos);
                LineRend.SetPosition(1, EndPos);

                SetItemParent(LineHolder, MapLinePrefabParent);
            }
        }

        // Convert temp list into an array
        // All lines are put into a temporary list cause if we just try to fill the array normally
        // then the LineRends that are being deleted will be put into said array
        Lines = new Transform[TempLineHolder.Count];
        for (int i = 0; i < TempLineHolder.Count; i++)
        {
            Lines[i] = TempLineHolder[i];
        }
    }

    // From https://code-maze.com/csharp-randomize-list/
    public List<KeyValuePair<Vector3, Vector3>> ShuffleList(List<KeyValuePair<Vector3, Vector3>> listToShuffle)
    {
        System.Random Rand = new();

        for (int i = listToShuffle.Count - 1; i > 0; i--)
        {
            var k = Rand.Next(i + 1);
            var value = listToShuffle[k];
            listToShuffle[k] = listToShuffle[i];
            listToShuffle[i] = value;
        }

        return listToShuffle;
    }

    [ClientRpc]
    void SetItemParent(GameObject Room, Transform Parent)
    {
        Room.transform.SetParent(Parent, false);
    }
}
