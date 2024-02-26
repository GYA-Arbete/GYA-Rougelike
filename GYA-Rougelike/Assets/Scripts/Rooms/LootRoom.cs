using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class LootRoom : NetworkBehaviour
{
    [Header("Viewable Elements")]
    public GameObject LootRoomCanvas;
    public Button UpgradeButton;
    public Button NewCardButton;

    [Header("Other Scripts")]
    public CameraSwitch CameraSwitchScript;
    public CardInventory CardInventoryScript;
    public CardChoice CardChoiceScript;
    public PlayerManager PlayerManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        UpgradeButton.onClick.AddListener(UpgradeCard);
        NewCardButton.onClick.AddListener(ChooseCard);
    }

    void UpgradeCard()
    {
        CardChoiceScript.StartChoice("LootRoom", true);

        ExitLootRoom();
    }

    void ChooseCard()
    {
        PlayerManagerScript.SetPlayerSpriteVisibility(false);

        SetCanvasVisibility(false);

        CardChoiceScript.StartChoice("LootRoom", false);
    }

    public void EnterLootRoom()
    {
        SetCanvasVisibility(true);

        CameraSwitchScript.SetViewToRoom();
    }

    [ClientRpc]
    void SetCanvasVisibility(bool State)
    {
        LootRoomCanvas.SetActive(State);
    }

    public void ExitLootRoom()
    {
        PlayerManagerScript.SetPlayerSpriteVisibility(true);

        SetCanvasVisibility(false);

        CameraSwitchScript.SetViewToMap();
    }
}
