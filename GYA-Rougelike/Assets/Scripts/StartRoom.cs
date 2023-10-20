using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartRoom : MonoBehaviour
{
    public GameObject[] ElementsToHide;

    public GameObject StartChoice;
    public Button[] ChoiceButtons;

    public Sprite[] CardSprites;

    public PullMapUpDown ViewSwitchScript;

    // Array for remembering which card is generated for each button
    private int[] CardIndex = new int[3];

    [Header("CardTypesJson")]
    public TextAsset CardTypesJson;
    public CardInventory.CardList CardTypes;

    [Header("Cards Inventory")]
    public TextAsset CardInventoryJson;
    public CardInventory.CardList CardsInventory;

    [Header("Other Scripts")]
    public CardInventory Inventory;
    public MapGen MapGenScript;

    // Start is called before the first frame update
    void Start()
    {
        // Get all card-types
        CardTypes = JsonUtility.FromJson<CardInventory.CardList>(CardTypesJson.text);

        ChoiceButtons[0].onClick.AddListener(Choose1);
        ChoiceButtons[1].onClick.AddListener(Choose2);
        ChoiceButtons[2].onClick.AddListener(Choose3);

        EnterStartRoom();
    }

    public void EnterStartRoom()
    {
        // Reset Card Inventory when re-starting the map / game
        CardsInventory = new CardInventory.CardList();
        CardsInventory = JsonUtility.FromJson<CardInventory.CardList>(CardInventoryJson.text);

        // Add the 5 starting cards to CardInventory
        CardsInventory.cardList.Add(CardTypes.cardList[0]);
        CardsInventory.cardList.Add(CardTypes.cardList[0]);
        CardsInventory.cardList.Add(CardTypes.cardList[0]);
        CardsInventory.cardList.Add(CardTypes.cardList[1]);
        CardsInventory.cardList.Add(CardTypes.cardList[1]);

        foreach (GameObject Element in ElementsToHide)
        {
            Element.SetActive(false);
        }

        System.Random Rand = new();
        for (int i = 0; i < ChoiceButtons.Length; i++)
        {
            int ChosenCard = Rand.Next(0, 7);

            ChoiceButtons[i].gameObject.GetComponent<Image>().sprite = CardSprites[ChosenCard];
            CardIndex[i] = ChosenCard;
        }

        StartChoice.SetActive(true);

        ViewSwitchScript.SetViewRoom();
    }

    void ExitStartRoom(int AddedIndex)
    {
        int[] CardTypes = { 0, 0, 0, 1, 1, AddedIndex };

        // Give the new card inventory to CardInventory.cs for easier acces from other scripts 
        // Also send the type of each card
        Inventory.UpdateInventory(CardsInventory, CardTypes);

        foreach (GameObject Element in ElementsToHide)
        {
            Element.SetActive(true);
        }

        StartChoice.SetActive(false);

        StartCoroutine(MapGenScript.CreateMap());

        ViewSwitchScript.SetViewMap();
    }

    void Choose1()
    {
        int AddedIndex = CardIndex[0];

        // Add the selected card to CardInventory
        CardsInventory.cardList.Add(CardTypes.cardList[AddedIndex]);

        ExitStartRoom(AddedIndex);
    }

    void Choose2()
    {
        int AddedIndex = CardIndex[1];

        // Add the selected card to CardInventory
        CardsInventory.cardList.Add(CardTypes.cardList[AddedIndex]);

        ExitStartRoom(AddedIndex);
    }

    void Choose3()
    {
        int AddedIndex = CardIndex[2];

        // Add the selected card to CardInventory
        CardsInventory.cardList.Add(CardTypes.cardList[AddedIndex]);

        ExitStartRoom(AddedIndex);
    }
}
