using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapNavigation : MonoBehaviour
{
    public Transform[] Rooms;
    public List<Button> RoomsButtons;

    public int CurrentRoom;

    // Each int is StartRoom / EndRoom id for a line
    public List<KeyValuePair<int, int>> AllowedNav;

    // 0 == StartRoom, 1 == EnemyRoom, 2 == LootRoom, 3 == CampRoom, 4 == BossRoom
    // 5 == HiddenType, 6 == ClearedRoom, 7 == CurrentRoom
    public Texture[] RoomImages;
    public GameObject ImagePrefab;

    public GameObject PreviousRoom;

    [Header("Other Scripts")]
    public MapGen MapGenScript;
    public CampRoom CampRoomScript;
    public LootRoom LootRoomScript;
    public CombatSystem CombatSystemScript;

    // Simple function that is called when map is generated to setup everything needed for MapNav to work
    public void SetupForMapNav(List<KeyValuePair<Vector3, Vector3>> SpawnedLines)
    {
        Rooms = MapGenScript.Rooms;

        for (int i = 1; i < Rooms.Length; i++)
        {
            Button Button = Rooms[i].GetComponent<Button>();

            Button.onClick.AddListener(delegate { RoomButtonPressed(Button); });

            Rooms[i].GetComponent<RoomProperties>().GenerateProperties(i - 1, RoomImages, ImagePrefab);

            RoomsButtons.Add(Button);
        }

        PreviousRoom = Rooms[1].gameObject;

        CurrentRoom = 0;

        ParseAllowedPaths(SpawnedLines);
    }

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
                Vector3 RoomPosition = new Vector3(Rooms[j].position.x, Rooms[j].position.y, -1);

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

        bool AllowedToNav = true;

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
                AllowedToNav = false;
            }
        }

        // When not allowed to nav, jump out of function
        if (!AllowedToNav)
        {
            return;
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

    void EnterRoom(int ID)
    {
        Transform Room = Rooms[ID + 1];

        RoomProperties Properties = Room.gameObject.GetComponent<RoomProperties>();

        // Set image showing selected room is current room
        Properties.RoomImage.GetComponent<RawImage>().texture = RoomImages[7];

        // Update previous room image to show it has been cleared
        PreviousRoom.GetComponent<RoomProperties>().RoomImage.GetComponent<RawImage>().texture = RoomImages[6];

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
}
