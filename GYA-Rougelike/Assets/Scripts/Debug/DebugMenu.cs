using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    public Button ExitRoomButton;
    public GameObject DebugCanvas;

    // Start is called before the first frame update
    void Start()
    {
        ExitRoomButton.onClick.AddListener(ExitRoom);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F3))
        {
            DebugCanvas.SetActive(!DebugCanvas.activeSelf);
        }
    }

    void ExitRoom()
    {
        MapNavigation MapNavScript = FindAnyObjectByType<MapNavigation>();
        Transform CurrentRoom = MapNavScript.Rooms[MapNavScript.CurrentRoom + 1];
        int RoomType = CurrentRoom.GetComponent<RoomProperties>().RoomType;

        switch (RoomType)
        {
            case 0:
                FindAnyObjectByType<StartRoom>().ExitStartRoom();
                break;
            case 1:
                FindAnyObjectByType<CombatSystem>().EndCombat();
                break;
            case 2:
                FindAnyObjectByType<LootRoom>().ExitLootRoom();
                break;
            case 3:
                FindAnyObjectByType<CampRoom>().ExitCampRoom();
                break;
            case 4:
                FindAnyObjectByType<CombatSystem>().EndCombat();
                break;
        }
    }
}
