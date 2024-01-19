using UnityEngine;
using UnityEngine.UI;

public class LootRoom : MonoBehaviour
{
    public bool InLootRoom = false;

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
        LootRoomCanvas.SetActive(false);

        CardChoiceScript.StartChoice("LootRoom", false);
    }

    public void EnterLootRoom()
    {
        InLootRoom = true;

        LootRoomCanvas.SetActive(true);

        CameraSwitchScript.SetViewToRoom();
    }

    public void ExitLootRoom()
    {
        InLootRoom = false;

        foreach (GameObject Element in ElementsToHide)
        {
            Element.SetActive(true);
        }

        LootRoomCanvas.SetActive(false);

        CameraSwitchScript.SetViewToMap();
    }
}
