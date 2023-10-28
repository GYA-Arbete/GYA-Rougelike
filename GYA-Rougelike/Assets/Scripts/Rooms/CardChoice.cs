using UnityEngine;
using UnityEngine.UI;

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
        for (int i = 0; i < 3; i++)
        {
            // Set which card-type is shown for each choice
            int CardType = Rand.Next(0, 7);
            CardChoices[i] = CardType;

            // Set correct image for each button
            ChoiceButtons[i].gameObject.GetComponent<Image>().sprite = CardInventoryScript.CardSprites[CardType];
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
