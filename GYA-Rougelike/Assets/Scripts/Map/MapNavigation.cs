using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Mirror;

public class MapNavigation : NetworkBehaviour
{
    public Transform[] Rooms;
    public List<Button> RoomsButtons;

    // Each int is StartRoom / EndRoom id for a line
    public List<KeyValuePair<int, int>> AllowedNav = new();

    // 0 == StartRoom, 1 == EnemyRoom, 2 == LootRoom, 3 == CampRoom, 4 == BossRoom
    // 5 == HiddenType, 6 == ClearedRoom, 7 == CurrentRoom
    public Texture[] RoomImages;
    public GameObject ImagePrefab;

    [Header("SyncVars")]
    [SyncVar]
    public int CurrentRoom = 0;
    [SyncVar]
    public GameObject PreviousRoom;

    [Header("Room Limits")]
    const int AllowedLootRooms = 2;
    const int AllowedCampRooms = 3;
    const int AllowedHiddenRooms = 2;

    [Header("Other Scripts")]
    public MapGen MapGenScript;
    public CampRoom CampRoomScript;
    public LootRoom LootRoomScript;
    public CombatSystem CombatSystemScript;

    // Simple function that is called when map is generated to setup everything needed for MapNav to work
    [Server]
    public void SetupForMapNav(List<KeyValuePair<Vector3, Vector3>> SpawnedLines)
    {
        GetRooms();

        ResetNavigation();

        var ReturnedVals = GenerateRoomProperties();

        // Set room properties to show up for all clients, hence why its a seperate function and ClientRpc
        SetRoomProperties(ReturnedVals.Item1, ReturnedVals.Item2);

        // Setup rooms server-side
        for (int i = 1; i < Rooms.Length; i++)
        {
            if (ReturnedVals.Item2[i - 1])
            {
                Rooms[i].GetComponent<RoomProperties>().SetupRoomServerSide(i - 1, ReturnedVals.Item1[i - 1]);
            }
            else
            {
                Rooms[i].GetComponent<RoomProperties>().SetupRoomServerSide(i - 1, ReturnedVals.Item1[i - 1]);
            }
        }

        ParseAllowedPaths(SpawnedLines);
    }

    // Function called from Clients and Server
    void GetRooms()
    {
        Rooms = MapGenScript.MapRoomPrefabParent.GetComponentsInChildren<Transform>();
    }

    [ClientRpc]
    void SetRoomProperties(int[] RoomType, bool[] HiddenType)
    {
        GetRooms();

        for (int i = 1; i < Rooms.Length; i++)
        {
            Button Button = Rooms[i].GetComponent<Button>();

            Button.onClick.AddListener(delegate { RoomButtonPressed(Button); });

            if (HiddenType[i - 1])
            {
                Rooms[i].GetComponent<RoomProperties>().SetupRoomClientSide(i - 1, RoomType[i - 1], RoomImages[5], ImagePrefab);
            }
            else
            {
                Rooms[i].GetComponent<RoomProperties>().SetupRoomClientSide(i - 1, RoomType[i - 1], RoomImages[RoomType[i - 1]], ImagePrefab);
            }

            RoomsButtons.Add(Button);
        }
    }

    [Server]
    private (int[], bool[]) GenerateRoomProperties()
    {
        int[] RoomType = new int[12];
        bool[] HiddenType = new bool[12];
        List<int> UntakenIndexes = new() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        // Choose type for each room
        RoomType[0] = 0; // StartRoom
        RoomType[11] = 4; // EndRoom

        System.Random Rand = new();

        // Choose 3 indexes for CampRoom
        for (int i = 0; i < AllowedCampRooms; i++)
        {
            int ChoosenIndex = Rand.Next(0, UntakenIndexes.Count);
            RoomType[UntakenIndexes[ChoosenIndex]] = 3;
            UntakenIndexes.RemoveAt(ChoosenIndex);
        }

        // Choose 2 indexes for LootRoom
        for (int i = 0; i < AllowedLootRooms; i++)
        {
            int ChoosenIndex = Rand.Next(0, UntakenIndexes.Count);
            RoomType[UntakenIndexes[ChoosenIndex]] = 2;
            UntakenIndexes.RemoveAt(ChoosenIndex);
        }

        // Set the rest to EnemyRoom
        for (int i = 0; i < UntakenIndexes.Count; i++)
        {
            RoomType[UntakenIndexes[i]] = 1;
        }

        // Reset UntakenIndexes
        UntakenIndexes = new() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        // Choose 2 indexes for HiddenType
        for (int i = 0; i < AllowedHiddenRooms; i++)
        {
            int ChoosenIndex = Rand.Next(0, UntakenIndexes.Count);
            HiddenType[UntakenIndexes[ChoosenIndex]] = true;
            UntakenIndexes.RemoveAt(ChoosenIndex);
        }

        return (RoomType, HiddenType);
    }

    [Server]
    void ParseAllowedPaths(List<KeyValuePair<Vector3, Vector3>> SpawnedLines)
    {
        // Clear AllowedNav to avoid old entries being kept
        AllowedNav = new();

        for (int i = 0; i < SpawnedLines.Count; i++)
        {
            Transform StartRoom = null;
            Transform EndRoom = null;

            for (int j = 1; j < Rooms.Length; j++)
            {
                Vector3 RoomPosition = new(Rooms[j].position.x, Rooms[j].position.y, -1);

                if (SpawnedLines[i].Key == RoomPosition)
                {
                    StartRoom = Rooms[j];
                }
                else if (SpawnedLines[i].Value == RoomPosition)
                {
                    EndRoom = Rooms[j];
                }
            }

            int StartID = StartRoom.gameObject.GetComponent<RoomProperties>().RoomID;
            int EndID = EndRoom.gameObject.GetComponent<RoomProperties>().RoomID;

            // Check that the room with lowest id is set as StartRoom
            if (StartID < EndID)
            {
                AllowedNav.Add(new KeyValuePair<int, int>(StartID, EndID));
            }
            else
            {
                AllowedNav.Add(new KeyValuePair<int, int>(EndID, StartID));
            }
        }
    }

    void RoomButtonPressed(Button Button)
    {
        int ID = Button.gameObject.GetComponent<RoomProperties>().RoomID;

        CheckNavIsAllowed(ID);
    }

    [Command(requiresAuthority=false)]
    void CheckNavIsAllowed(int ID)
    {
        // Check that a line is going between CurrentRoom and the room we want to navigate too
        for (int i = 0; i < AllowedNav.Count; i++)
        {
            if (CurrentRoom == AllowedNav[i].Key && ID == AllowedNav[i].Value)
            {
                // Nav is allowed, jump out of loop and keep going
                break;
            }
            else if (i == AllowedNav.Count - 1)
            {
                // If have checked with every AllowedNav and every one failed, do not allow to keep going
                return;
            }
        }

        // Check if navigation is allowed between current room and selected room
        switch (CurrentRoom % 2)
        {
            case 0:
                if (ID == CurrentRoom + 1 || ID == CurrentRoom + 2)
                {
                    EnterRoom(ID);
                }
                break;
            case 1:
                if (ID == CurrentRoom + 2 || ID == CurrentRoom + 3)
                {
                    EnterRoom(ID);
                }
                break;
        }
    }

    [Command(requiresAuthority=false)]
    void EnterRoom(int ID)
    {
        Transform Room = Rooms[ID + 1];

        RoomProperties Properties = Room.gameObject.GetComponent<RoomProperties>();

        UpdateRoomImages(Room, PreviousRoom);

        switch (Properties.RoomType)
        {
            case 1:
                CombatSystemScript.StartCombat(Properties.EnemyAmount, Properties.EnemyTypes);
                break;
            case 2:
                LootRoomScript.EnterLootRoom();
                break;
            case 3:
                CampRoomScript.EnterCampRoom();
                break;
            case 4:
                CombatSystemScript.StartCombat(Properties.EnemyAmount, Properties.EnemyTypes);
                break;
        }

        CurrentRoom = ID;

        PreviousRoom = Room.gameObject;
    }

    [Command(requiresAuthority=false)]
    void ResetNavigation()
    {
        CurrentRoom = 0;
        PreviousRoom = Rooms[1].gameObject;
    }

    [ClientRpc]
    void UpdateRoomImages(Transform Room, GameObject PreviousRoom)
    {
        // Set image showing selected room is current room
        Room.gameObject.GetComponent<RoomProperties>().RoomImageObject.GetComponent<RawImage>().texture = RoomImages[7];

        // Update previous room image to show it has been cleared
        PreviousRoom.GetComponent<RoomProperties>().RoomImageObject.GetComponent<RawImage>().texture = RoomImages[6];
    }
}
