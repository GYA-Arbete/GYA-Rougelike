using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using Mirror;

public class CardChoice : NetworkBehaviour
{
    [Header("Viewable Elements")]
    public Button[] ChoiceButtons = new Button[3];
    public GameObject CardChoiceCanvas;
    public Button ExitRoomButton;

    [Header("Variables for Choice")]
    public int[] CardChoices = new int[3];
    public string Sender;

    [Header("Other Scripts")]
    public CardInventory CardInventoryScript;
    public StartRoom StartRoomScript;
    public LootRoom LootRoomScript;

    private bool Upgrade = false;
    private int[] ChoosenCardIndexes = new int[3];

    [Header("SyncVars")]
    [SyncVar(hook = nameof(HandleReadyPlayersChanged))]
    public int ReadyPlayers = 0;

    // Start is called before the first frame update
    void Start()
    {
        ChoiceButtons[0].onClick.AddListener(delegate { ChooseCard(CardChoices[0]); });
        ChoiceButtons[1].onClick.AddListener(delegate { ChooseCard(CardChoices[1]); });
        ChoiceButtons[2].onClick.AddListener(delegate { ChooseCard(CardChoices[2]); });
        ExitRoomButton.onClick.AddListener(ExitRoom);
    }

    // DO NOT REMOVE ANY VARIABLES, IT WILL CAUSE ERRORS
    public void HandleReadyPlayersChanged(int oldValue, int newValue) => UpdateExitRoomButton();

    void UpdateExitRoomButton()
    {
        switch (ReadyPlayers)
        {
            case 0:
                ExitRoomButton.interactable = false;
                ExitRoomButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "0/2 Finished";
                break;
            case 1:
                ExitRoomButton.interactable = false;
                ExitRoomButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "1/2 Finished";
                break;
            case 2:
                ExitRoomButton.interactable = true;
                ExitRoomButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Continue";
                break;
        }
    }

    [ClientRpc]
    public void StartChoice(string SenderScript, bool upgrade)
    {
        Upgrade = upgrade;

        // Remember which script called this function to exit said room correctly
        Sender = SenderScript;

        System.Random Rand = new();

        int[] CardType = new int[3];

        // If choice is which card to upgrade
        if (Upgrade)
        {
            ChoosenCardIndexes = new int[3];

            CardInventory.CardList PossibleCards = CardInventoryScript.Inventory;
            List<int> CardTypes = CardInventoryScript.CardType;
            int IndexOffset = 0;

            for (int i = 0; i < 3; i++)
            {
                // Randomly choose 3 cards from the CardInventory
                int ChoosenCard = Rand.Next(0, PossibleCards.cardList.Count);

                CardType[i] = ChoosenCard;

                // Compensate for indexes getting shifted when removing items from lists
                if (i != 0)
                {
                    if (IndexOffset > ChoosenCardIndexes[i - 1])
                    {
                        IndexOffset++;
                    }
                }
                
                ChoosenCardIndexes[i] = ChoosenCard + IndexOffset;

                // Remove the choosen card from each list
                PossibleCards.cardList.RemoveAt(ChoosenCard);
                CardTypes.RemoveAt(ChoosenCard);
            }
        }
        // If choice is for a new card
        else
        {
            // Generate which cards to offer as choice
            int[] AllowedCards = { 0, 1, 2, 3, 4, 5, 6 };
            for (int i = 0; i < 3; i++)
            {
                int ChoosenCard = Rand.Next(0, AllowedCards.Length);

                CardType[i] = AllowedCards[ChoosenCard];

                // Remove the choosen number from array
                AllowedCards = AllowedCards.Where(val => val != AllowedCards[ChoosenCard]).ToArray();
            }
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
                        else if (CardInventoryScript.CardTypes.cardList[CardType[i]].Thorns > 0)
                        {
                            Text.text = $"{CardInventoryScript.CardTypes.cardList[CardType[i]].Thorns}x";
                            Text.fontSize = 0.2f;
                        }
                    }
                }
            }
        }

        // Set starting state of ExitRoomButton
        UpdateExitRoomButton();

        // Show the choice buttons
        CardChoiceCanvas.SetActive(true);
    }

    void ChooseCard(int Choice)
    {
        EndChoice(Choice);
    }

    void EndChoice(int ChoosenCard)
    {
        // Remove the buttons as player has already choosen
        foreach (Button ChoiceButton in ChoiceButtons)
        {
            Destroy(ChoiceButton.gameObject);
        }

        if (Upgrade)
        {
            CardInventoryScript.UpgradeCard(ChoosenCardIndexes[ChoosenCard]);
        }
        else
        {
            // Update the players inventory
            CardInventoryScript.AddCardToInventory(ChoosenCard);
        }

        // Calls a command to update the SyncVar cause otherwise it wont be synced between clients
        UpdateReadyPlayers();
    }

    [Command(requiresAuthority=false)]
    void UpdateReadyPlayers()
    {
        ReadyPlayers++;
    }

    [Command(requiresAuthority=false)]
    void ResetReadyPlayers()
    {
        ReadyPlayers = 0;
    }

    [Command(requiresAuthority=false)]
    void ExitRoom()
    {
        // Yes this is dumb but it has to be done because:
        // 1. We want to hide the Canvas for all players
        // 2. We only want to call the exit room function once
        HideCardChoiceCanvas();

        ResetReadyPlayers();

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

    [ClientRpc]
    void HideCardChoiceCanvas()
    {
        CardChoiceCanvas.SetActive(false);
    }
}
