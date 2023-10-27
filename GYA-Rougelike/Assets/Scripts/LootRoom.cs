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




    public GameObject[] ElementsToHide;

    public GameObject CardChoice;
    public Button[] ChoiceButtons;

    public Sprite[] CardSprites;

    // Array for remembering which card is generated for each button
    private int[] CardIndex = new int[3];

    [Header("CardTypesJson")]
    public TextAsset CardTypesJson;
    public CardInventory.CardList CardTypes;

    [Header("Cards Inventory")]
    public CardInventory.CardList CardsInventory;

    [Header("Other Scripts")]
    public CardInventory Inventory;

    // Start is called before the first frame update
    void Start()
    {
        UpgradeButton.onClick.AddListener(UpgradeCard);
        NewCardButton.onClick.AddListener(StartChoice);

        ViewSwitchScript = FindAnyObjectByType<PullMapUpDown>();

        // Get all card-types
        CardTypes = JsonUtility.FromJson<CardInventory.CardList>(CardTypesJson.text);

        

        CardSprites = Inventory.CardSprites;
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

    public void StartChoice()
    {
        CardsInventory.cardList.Clear();

        ChoiceButtons[0].onClick.AddListener(Choose1);
        ChoiceButtons[1].onClick.AddListener(Choose2);
        ChoiceButtons[2].onClick.AddListener(Choose3);

        foreach (GameObject Element in ElementsToHide)
        {
            Element.SetActive(false);
        }
        LootRoomCanvas.SetActive(false);

        System.Random Rand = new();
        for (int i = 0; i < ChoiceButtons.Length; i++)
        {
            int ChosenCard = Rand.Next(0, 7);

            ChoiceButtons[i].gameObject.GetComponent<Image>().sprite = CardSprites[ChosenCard];
            CardIndex[i] = ChosenCard;
        }

        CardChoice.SetActive(true);
    }

    void FinishedChoice(int AddedIndex)
    {
        int[] CardTypes = { AddedIndex };

        // Give the new card inventory to CardInventory.cs for easier acces from other scripts 
        // Also send the type of each card
        Inventory.UpdateInventory(CardsInventory, CardTypes);

        foreach (GameObject Element in ElementsToHide)
        {
            Element.SetActive(true);
        }

        CardChoice.SetActive(false);

        ChoiceButtons[0].onClick.RemoveListener(Choose1);
        ChoiceButtons[1].onClick.RemoveListener(Choose2);
        ChoiceButtons[2].onClick.RemoveListener(Choose3);

        ExitRoom();
    }

    void Choose1()
    {
        int AddedIndex = CardIndex[0];

        // Add the selected card to CardInventory
        CardsInventory.cardList.Add(CardTypes.cardList[AddedIndex]);

        FinishedChoice(AddedIndex);
    }

    void Choose2()
    {
        int AddedIndex = CardIndex[1];

        // Add the selected card to CardInventory
        CardsInventory.cardList.Add(CardTypes.cardList[AddedIndex]);

        FinishedChoice(AddedIndex);
    }

    void Choose3()
    {
        int AddedIndex = CardIndex[2];

        // Add the selected card to CardInventory
        CardsInventory.cardList.Add(CardTypes.cardList[AddedIndex]);

        FinishedChoice(AddedIndex);
    }
}
