using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardInventory : MonoBehaviour
{
    bool InventoryOpen = false;

    public Button CardInventoryButton;

    public Transform[] CardInventoryArray;

    public GameObject InventoryBox;

    [Header("CardInventory.json Stuff")]
    public CardList cardList;
    public TextAsset JsonFile;

    [Header("CardSpawning stuff")]
    public Transform CardParent;
    public GameObject CardPrefab;
    public Transform[] Row1SpawnPoints;
    public List<GameObject> SpawnedCards;

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

        // Update the CardList
        cardList = new CardList();
        cardList = JsonUtility.FromJson<CardList>(JsonFile.text);

        // Clear SpawnedCars list
        SpawnedCards = new List<GameObject>();

        // Spawn cards
        for (int i = 0; i < cardList.cardList.Count; i++)
        {
            // https://docs.unity3d.com/ScriptReference/Object.Instantiate.html
            // Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent);
            // Running (i Modulo 5) since every 5th value it should jump down, so 0, 1, 2, 3, 4 then back to 0
            GameObject Card = Instantiate(CardPrefab, new Vector3(Row1SpawnPoints[i % 5].position.x, Row1SpawnPoints[i % 5].position.y, Row1SpawnPoints[i % 5].position.z), new Quaternion(0, 0, 0, 0), CardParent);
            Card.transform.localScale = new Vector3(2, 2, 0);

            // Remove attached scripts
            Destroy(Card.GetComponent<DragDropCard>());
            Destroy(Card.GetComponent<CardStats>());

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
