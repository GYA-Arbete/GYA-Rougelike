using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootRoom : MonoBehaviour
{
    public GameObject LootRoomCanvas;

    public Button UpgradeButton;
    public Button NewCardButton;

    public PullMapUpDown ViewSwitchScript;

    // Start is called before the first frame update
    void Start()
    {
        UpgradeButton.onClick.AddListener(UpgradeCard);
        NewCardButton.onClick.AddListener(ChooseNewCard);

        ViewSwitchScript = FindAnyObjectByType<PullMapUpDown>();
    }

    public void EnterRoom()
    {
        LootRoomCanvas.SetActive(true);
    }

    void ExitRoom()
    {
        LootRoomCanvas.SetActive(false);

        ViewSwitchScript.SetViewMap();
    }

    void UpgradeCard()
    {
        ExitRoom();
    }

    void ChooseNewCard()
    {
        ExitRoom();
    }
}
