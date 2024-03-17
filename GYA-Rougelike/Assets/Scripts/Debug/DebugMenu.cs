using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    public Button ExitRoomButton;
    public GameObject DebugCanvas;
    public TextMeshProUGUI PlayerIDText;

    [Header("Set inventory")]
    public Button DefaultButton;
    public Button OneOfEachButton;

    // Start is called before the first frame update
    void Start()
    {
        ExitRoomButton.onClick.AddListener(ExitRoom);

        DefaultButton.onClick.AddListener(delegate { SetCardInventory(0); });
        OneOfEachButton.onClick.AddListener(delegate { SetCardInventory(1); });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F3))
        {
            DebugCanvas.SetActive(!DebugCanvas.activeSelf);
            UpdatePlayerID();
        }
    }

    void UpdatePlayerID()
    {
        int PlayerID = FindAnyObjectByType<LobbyMenuUI>().PlayerNumber;

        // If PlayerNumber hasnt been set
        if (PlayerID == 0)
        {
            PlayerIDText.text = "PlayerID: ?";
        }
        else
        {
            PlayerIDText.text = $"PlayerID: {PlayerID}";
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

    void SetCardInventory(int SenderIndex)
    {
        CardInventory InventoryScript = FindAnyObjectByType<CardInventory>();
        InventoryScript.ClearInventory();

        int[] CardTypes = new int[7];

        switch (SenderIndex)
        {
            // Default
            case 0:
                CardTypes = new int[] { 0, 0, 0, 1, 1 };
                break;
            // One of Each
            case 1:
                CardTypes = new int[] { 0, 1, 2, 3, 4, 5, 6 };
                break;
        }

        for (int i = 0; i < CardTypes.Length; i++)
        {
            InventoryScript.AddCardToInventory(CardTypes[i]);
        }
    }
}
