using UnityEngine;
using UnityEngine.UI;

public class LootRoom : MonoBehaviour
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
        ExitLootRoom();
    }

    void ChooseCard()
    {
        foreach (GameObject Element in ElementsToHide)
        {
            Element.SetActive(false);
        }
        LootRoomCanvas.SetActive(false);

        CardChoiceScript.StartChoice("LootRoom");
    }

    public void EnterLootRoom()
    {
        LootRoomCanvas.SetActive(true);

        CameraSwitchScript.SetViewToRoom();
    }

    public void ExitLootRoom()
    {
        LootRoomCanvas.SetActive(false);

        CameraSwitchScript.SetViewToMap();
    }
}
