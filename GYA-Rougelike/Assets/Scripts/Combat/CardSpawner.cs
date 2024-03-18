using Mirror;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardSpawner : NetworkBehaviour
{
    [Header("Card Spawning")]
    public Transform CardsParent;
    public GameObject CardPrefab;
    public Transform CardSpawnPointsParent;
    public Transform[] CardSpawnPoints;
    public GameObject[] SpawnedCards;

    [Header("Other Scripts")]
    public CardInventory CardInventoryScript;
    public CardManager CardManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        // Put parent + children into array
        CardSpawnPoints = CardSpawnPointsParent.GetComponentsInChildren<Transform>();
    }

    public void DespawnCards()
    {
        // Remove cards
        foreach (GameObject Card in SpawnedCards)
        {
            Destroy(Card);
        }

        // Reset CardInPoint array, no cards are in any point since they are all despawned
        for (int i = 0; i < CardManagerScript.CardInPoint.Length; i++)
        {
            CardManagerScript.CardInPoint[i] = false;
        }
    }

    [ClientRpc]
    public void RpcDespawnCards()
    {
        DespawnCards();
    }

    [ClientRpc]
    public void ResetCards()
    {
        DespawnCards();

        SpawnCards();
    }

    [Client]
    void SpawnCards()
    {
        CardInventory.CardList CardsInInventory = CardInventoryScript.Inventory;
        List<int> CardType = CardInventoryScript.CardType;
        Sprite[] CardSprites = CardInventoryScript.CardSprites;

        SpawnedCards = new GameObject[CardsInInventory.cardList.Count];

        List<int> SnappedToPoints = new();

        // i = 1 eftersom den ska ingorera parent
        for (int i = 1; i < CardsInInventory.cardList.Count + 1; i++)
        {
            // Check that the card isnt on cooldown
            if (CardsInInventory.cardList[i - 1].CardCooldown == 0)
            {
                // https://docs.unity3d.com/ScriptReference/Object.Instantiate.html
                // Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent);
                GameObject Card = Instantiate(CardPrefab, new Vector3(CardSpawnPoints[i].position.x, CardSpawnPoints[i].position.y, CardSpawnPoints[i].position.z), new Quaternion(0, 0, 0, 0), CardsParent);

                // Assign values to each created card
                CardStats CardStatsScript = Card.GetComponent<CardStats>();
                CardStatsScript.AssignValues(CardsInInventory.cardList[i - 1].Energy, CardsInInventory.cardList[i - 1].Damage, CardsInInventory.cardList[i - 1].SplashDamage, CardsInInventory.cardList[i - 1].Defence, CardsInInventory.cardList[i - 1].Thorns, CardsInInventory.cardList[i - 1].Stun, CardsInInventory.cardList[i - 1].DamageBuff, CardsInInventory.cardList[i - 1].Cooldown, i - 1);

                // Setup stuff for snapping
                Card.GetComponent<CardComponent>().Setup(i - 1, CardManagerScript.CardTypesText.cardList[CardType[i - 1]].CardInfo);
                SnappedToPoints.Add(i - 1);

                // Change the cards image
                Card.GetComponent<SpriteRenderer>().sprite = CardSprites[CardType[i - 1]];

                // Set text for the card-sprite
                Transform[] TextBoxes = Card.GetComponentsInChildren<Transform>();
                foreach (Transform TextBox in TextBoxes)
                {
                    TextMeshProUGUI Text = TextBox.GetComponent<TextMeshProUGUI>();
                    if (Text != null)
                    {
                        if (TextBox.name == "EnergyCount")
                        {
                            Text.text = CardsInInventory.cardList[i - 1].Energy.ToString();
                        }
                        else
                        {
                            if (CardStatsScript.Damage > 0)
                            {
                                Text.text = CardStatsScript.Damage.ToString();
                            }
                            else if (CardStatsScript.Defence > 0)
                            {
                                Text.text = CardStatsScript.Defence.ToString();
                            }
                            else if (CardStatsScript.DamageBuff > 0)
                            {
                                Text.text = CardStatsScript.DamageBuff.ToString();
                            }
                            else if (CardStatsScript.Stun > 0)
                            {
                                Text.text = CardStatsScript.Stun.ToString();
                            }
                            else if (CardStatsScript.Thorns > 0)
                            {
                                Text.text = $"{CardStatsScript.Thorns}x";
                                Text.fontSize = 0.2f;
                            }
                        }
                    }
                }

                SpawnedCards[i - 1] = Card;

                CardManagerScript.SetCardInPoint(SnappedToPoints);
            }
            // If the card is on cooldown, reduce it by 1
            else
            {
                CardInventoryScript.Inventory.cardList[i - 1].CardCooldown--;
            }
        }
    }
}
