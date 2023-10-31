using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapNavigation : MonoBehaviour
{
    public Transform[] Rooms;
    public List<Button> RoomsButtons;

    public int CurrentRoom;

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
    public void SetupForMapNav()
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
    }

    void RoomButtonPressed(Button Button)
    {
        // Check if navigation is allowed between current room and selected room
        switch (CurrentRoom % 2)
        {
            case 0:
                if (Button.gameObject.GetComponent<RoomProperties>().RoomID == CurrentRoom + 1 || Button.gameObject.GetComponent<RoomProperties>().RoomID == CurrentRoom + 2)
                {
                    EnterRoom(Button.gameObject.GetComponent<RoomProperties>().RoomID);
                }
                break;
            case 1:
                if (Button.gameObject.GetComponent<RoomProperties>().RoomID == CurrentRoom + 2 || Button.gameObject.GetComponent<RoomProperties>().RoomID == CurrentRoom + 3)
                {
                    EnterRoom(Button.gameObject.GetComponent<RoomProperties>().RoomID);
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
