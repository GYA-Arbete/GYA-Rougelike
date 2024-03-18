using Mirror;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardInventory : NetworkBehaviour
{
    bool InventoryOpen = false;

    public Button CardInventoryButton;

    public GameObject InventoryBox;

    public Sprite[] CardSprites;

    [Header("CardTypesJson")]
    public TextAsset CardTypesJson;
    public CardList CardTypes;

    [Header("CardInventory")]
    public CardList Inventory;

    [Header("CardSpawning stuff")]
    public Transform CardParent;
    public GameObject CardPrefab;
    public Transform[] Row1SpawnPoints;
    public List<GameObject> SpawnedCards;
    public List<int> CardType;

    [Serializable]
    public class CardList
    {
        public List<Cards> cardList;
    }

    [Serializable]
    public class Cards
    {
        public int Energy;
        public int Damage;
        public bool SplashDamage;
        public int Defence;
        public int Thorns;
        public int Stun;
        public int DamageBuff;
        public int Cooldown;
        public int CardCooldown;
    }

    // Start is called before the first frame update
    void Start()
    {
        CardTypes = JsonUtility.FromJson<CardList>(CardTypesJson.text);

        CardInventoryButton.onClick.AddListener(SwitchInventory);
    }

    public void ClearInventory()
    {
        Inventory.cardList.Clear();
        CardType.Clear();
    }

    [ClientRpc]
    public void ResetInventory()
    {
        // Clear each list to not keep values when restarting
        Inventory.cardList.Clear();
        CardType.Clear();

        int[] StartingCardTypes = { 0, 0, 0, 1, 1 };

        for (int i = 0; i < 5; i++)
        {
            Cards Card = CardTypes.cardList[StartingCardTypes[i]];

            Inventory.cardList.Add(new Cards() { Energy = Card.Energy, Damage = Card.Damage, SplashDamage = Card.SplashDamage, Defence = Card.Defence, Thorns = Card.Thorns, Stun = Card.Stun, DamageBuff = Card.DamageBuff, Cooldown = Card.Cooldown, CardCooldown = 0 });
            CardType.Add(StartingCardTypes[i]);
        }
    }

    // Function that is called when adding cards to the inventory
    public void AddCardToInventory(int ChoosenCardType)
    {
        // Add the selected card to the inventory
        Inventory.cardList.Add(CardTypes.cardList[ChoosenCardType]);

        CardType.Add(ChoosenCardType);
    }

    public void UpgradeCard(int Index)
    {
        switch (CardType[Index])
        {
            case 0:
                Inventory.cardList[Index].Damage += 2;
                break;
            case 1:
                Inventory.cardList[Index].Defence += 2;
                break;
            case 2:
                Inventory.cardList[Index].Thorns++; 
                break;
            case 3:
                Inventory.cardList[Index].Damage += 2;
                break;
            case 4:
                Inventory.cardList[Index].Stun++;
                break;
            case 5:
                Inventory.cardList[Index].Damage++;
                Inventory.cardList[Index].Defence++;
                break;
            case 6:
                Inventory.cardList[Index].DamageBuff += 2;
                break;
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

        // Clear SpawnedCards list
        SpawnedCards = new List<GameObject>();

        // Spawn cards
        for (int i = 0; i < Inventory.cardList.Count; i++)
        {
            // Calculate the Y-Offset, rounded down to closest int
            // This means it increases by one every 5 "loops"
            int YOffset = i / 5;

            // https://docs.unity3d.com/ScriptReference/Object.Instantiate.html
            // Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent);
            // Running (i Modulo 5) since every 5th value it should jump down, so 0, 1, 2, 3, 4 then back to 0
            // YOffset * 3.7f since it should make an even space between each row
            GameObject Card = Instantiate(CardPrefab, new Vector3(Row1SpawnPoints[i % 5].position.x, Row1SpawnPoints[i % 5].position.y - (YOffset * 3.7f), Row1SpawnPoints[i % 5].position.z), new Quaternion(0, 0, 0, 0), CardParent);
            Card.transform.localScale = new Vector3(2, 2, 0);

            // Set image of said Card
            Card.GetComponent<Image>().sprite = CardSprites[CardType[i]];

            // Set text for the card-sprite
            Transform[] TextBoxes = Card.GetComponentsInChildren<Transform>();
            foreach (Transform TextBox in TextBoxes)
            {
                TextMeshProUGUI Text = TextBox.GetComponent<TextMeshProUGUI>();
                if (Text != null)
                {
                    // If energy count
                    if (TextBox.name == "EnergyCount")
                    {
                        Text.text = Inventory.cardList[i].Energy.ToString();
                    }
                    // If card-stat count
                    else
                    {
                        if (Inventory.cardList[i].Damage > 0)
                        {
                            Text.text = Inventory.cardList[i].Damage.ToString();
                        }
                        else if (Inventory.cardList[i].Defence > 0)
                        {
                            Text.text = Inventory.cardList[i].Defence.ToString();
                        }
                        else if (Inventory.cardList[i].DamageBuff > 0)
                        {
                            Text.text = Inventory.cardList[i].DamageBuff.ToString();
                        }
                        else if (Inventory.cardList[i].Stun > 0)
                        {
                            Text.text = Inventory.cardList[i].Stun.ToString();
                        }
                        else if (Inventory.cardList[i].Thorns > 0)
                        {
                            Text.text = $"{Inventory.cardList[i].Thorns}x";
                            Text.fontSize = 0.2f;
                        }
                    }
                }
            }

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
