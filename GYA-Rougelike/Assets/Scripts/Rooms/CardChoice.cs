using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CardChoice : MonoBehaviour
{
    [Header("Viewable Elements")]
    public Button[] ChoiceButtons = new Button[3];
    public GameObject CardChoiceCanvas;

    [Header("Variables for Choice")]
    public int[] CardChoices = new int[3];
    public string Sender;

    [Header("Other Scripts")]
    public CardInventory CardInventoryScript;
    public StartRoom StartRoomScript;
    public LootRoom LootRoomScript;

    // Start is called before the first frame update
    void Start()
    {
        ChoiceButtons[0].onClick.AddListener(Choose1);
        ChoiceButtons[1].onClick.AddListener(Choose2);
        ChoiceButtons[2].onClick.AddListener(Choose3);
    }

    public void StartChoice(string SenderScript)
    {
        // Remember which script called this function to exit said room correctly
        Sender = SenderScript;

        System.Random Rand = new();

        // Generate which cards to have as choices
        int[] CardType = new int[3];
        int[] AllowedCards = { 0, 1, 2, 3, 4, 5, 6 };
        for (int i = 0; i < 3; i++)
        {
            int ChoosenCard = Rand.Next(0, AllowedCards.Length);

            CardType[i] = AllowedCards[ChoosenCard];

            // Remove the choosen number from array
            AllowedCards = AllowedCards.Where(val => val != ChoosenCard).ToArray();
        }

        for (int i = 0; i < 3; i++)
        {
            // Set which card-type is shown for each choice
            CardChoices[i] = CardType[i];

            // Set correct image for each button
            ChoiceButtons[i].gameObject.GetComponent<Image>().sprite = CardInventoryScript.CardSprites[CardType[i]];

            // Set text for the card-sprite
            Transform[] TextBoxes = ChoiceButtons[i].GetComponentsInChildren<Transform>();
            foreach (Transform TextBox in TextBoxes)
            {
                TextMeshProUGUI Text = TextBox.GetComponent<TextMeshProUGUI>();
                if (Text != null)
                {
                    if (TextBox.name == "EnergyCount")
                    {
                        Text.text = CardInventoryScript.CardTypes.cardList[CardType[i]].Energy.ToString();
                    }
                    else
                    {
                        if (CardInventoryScript.CardTypes.cardList[CardType[i]].Damage > 0)
                        {
                            Text.text = CardInventoryScript.CardTypes.cardList[CardType[i]].Damage.ToString();
                            Text.fontSize = 0.3f;
                        }
                        else if (CardInventoryScript.CardTypes.cardList[CardType[i]].Defence > 0)
                        {
                            Text.text = CardInventoryScript.CardTypes.cardList[CardType[i]].Defence.ToString();
                            Text.fontSize = 0.3f;
                        }
                        else if (CardInventoryScript.CardTypes.cardList[CardType[i]].DamageBuff > 0)
                        {
                            Text.text = CardInventoryScript.CardTypes.cardList[CardType[i]].DamageBuff.ToString();
                            Text.fontSize = 0.3f;
                        }
                        else if (CardInventoryScript.CardTypes.cardList[CardType[i]].Stun > 0)
                        {
                            Text.text = CardInventoryScript.CardTypes.cardList[CardType[i]].Stun.ToString();
                            Text.fontSize = 0.3f;
                        }
                        else if (CardInventoryScript.CardTypes.cardList[CardType[i]].Thorns)
                        {
                            Text.text = "1x";
                            Text.fontSize = 0.2f;
                        }
                    }
                }
            }
        }

        // Show the choice buttons
        CardChoiceCanvas.SetActive(true);
    }

    void Choose1()
    {
        EndChoice(CardChoices[0]);
    }

    void Choose2()
    {
        EndChoice(CardChoices[1]);
    }

    void Choose3()
    {
        EndChoice(CardChoices[2]);
    }

    void EndChoice(int ChoosenCard)
    {
        // Hide the choice buttons
        CardChoiceCanvas.SetActive(false);

        // Update the players inventory
        CardInventoryScript.AddCardToInventory(ChoosenCard);

        // Check which room the player is in and exit correctly
        if (Sender == "StartRoom")
        {
            StartRoomScript.ExitStartRoom();
        }
        else if (Sender == "LootRoom")
        {
            LootRoomScript.ExitLootRoom();
        }
    }
}
