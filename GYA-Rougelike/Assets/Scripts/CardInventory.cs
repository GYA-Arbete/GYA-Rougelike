using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CardInventory : MonoBehaviour
{
    bool InventoryOpen = false;

    public Button CardInventoryButton;

    public Transform[] CardInventoryArray;

    public GameObject InventoryBox;

    public Sprite[] CardSprites;

    [Header("CardInventory.json Stuff")]
    public CardList Inventory;
    public TextAsset JsonFile;

    [Header("CardSpawning stuff")]
    public Transform CardParent;
    public GameObject CardPrefab;
    public Transform[] Row1SpawnPoints;
    public List<GameObject> SpawnedCards;
    public List<int> CardType;

    [System.Serializable]
    public class CardList
    {
        public List<Cards> cardList;
    }

    [System.Serializable]
    public class Cards
    {
        public int Energy;
        public int Damage;
        public int Defence;
        public int Cooldown;
    }

    // Start is called before the first frame update
    void Start()
    {
        CardInventoryButton.onClick.AddListener(SwitchInventory);
    }

    public void SetInventory(CardList List, int[] TypeArray)
    {
        Inventory = List;

        CardType.Clear();
        foreach (int Type in TypeArray)
        {
            CardType.Add(Type);
        }
    }

    public void UpdateInventory(CardList List, int[] TypeArray)
    {
        foreach (Cards Card in List.cardList)
        {
            Inventory.cardList.Add(Card);
        }

        foreach (int Type in TypeArray)
        {
            CardType.Add(Type);
        }
    }

    void SwitchInventory()
    {
        if (InventoryOpen)
        {
            CloseInventory();
        }
        else
        {
            OpenInvetory();
        }

        // Invert value of InventoryOpen
        InventoryOpen = !InventoryOpen;
    }

    void OpenInvetory()
    {
        InventoryBox.SetActive(true);

        // Clear SpawnedCars list
        SpawnedCards = new List<GameObject>();

        // Spawn cards
        for (int i = 0; i < Inventory.cardList.Count; i++)
        {
            // Calculate the Y-Offset, rounded down to closest int
            // This means it increases by one every 5 "loops"
            Decimal Division = i / 5;
            Decimal YOffset = Decimal.Truncate(Division);

            // https://docs.unity3d.com/ScriptReference/Object.Instantiate.html
            // Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent);
            // Running (i Modulo 5) since every 5th value it should jump down, so 0, 1, 2, 3, 4 then back to 0
            GameObject Card = Instantiate(CardPrefab, new Vector3(Row1SpawnPoints[i % 5].position.x, Row1SpawnPoints[i % 5].position.y - ((float)YOffset * 3.7f), Row1SpawnPoints[i % 5].position.z), new Quaternion(0, 0, 0, 0), CardParent);

            // Set image of said Card
            Card.GetComponent<Image>().sprite = CardSprites[CardType[i]];

            SpawnedCards.Add(Card);
        }
    }

    void CloseInventory()
    {
        InventoryBox.SetActive(false);

        foreach (GameObject Card in SpawnedCards)
        {
            Destroy(Card);
        }
    }
}
