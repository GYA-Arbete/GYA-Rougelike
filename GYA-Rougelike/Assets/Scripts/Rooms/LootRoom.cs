using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class LootRoom : NetworkBehaviour
{
    [Header("Viewable Elements")]
    public GameObject LootRoomCanvas;
    public Button UpgradeButton;
    public Button NewCardButton;
    public GameObject[] ElementsToHide;

    [Header("Other Scripts")]
    public CameraSwitch CameraSwitchScript;
    public CardInventory CardInventoryScript;
    public CardChoice CardChoiceScript;

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
        foreach (GameObject Element in ElementsToHide)
        {
            Element.SetActive(false);
        }
        SetCanvasVisibility(false);

        CardChoiceScript.StartChoice("LootRoom", false);
    }

    public void EnterLootRoom()
    {
        SetCanvasVisibility(true);

        CameraSwitchScript.SetViewToRoom();
    }

    [ClientRpc]
    void SetCanvasVisibility(bool Override)
    {
        LootRoomCanvas.SetActive(Override);
    }

    public void ExitLootRoom()
    {
        foreach (GameObject Element in ElementsToHide)
        {
            Element.SetActive(true);
        }

        SetCanvasVisibility(false);

        CameraSwitchScript.SetViewToMap();
    }
}
