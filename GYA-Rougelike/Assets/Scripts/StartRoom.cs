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

    public CardList CardInventory;

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
        ChoiceButtons[0].onClick.AddListener(Choose1);
        ChoiceButtons[1].onClick.AddListener(Choose2);
        ChoiceButtons[2].onClick.AddListener(Choose3);

        EnterStartRoom();
    }

    public void EnterStartRoom()
    {
        foreach (GameObject Element in ElementsToHide)
        {
            Element.SetActive(false);
        }

        System.Random Rand = new();
        foreach (Button Button in ChoiceButtons)
        {
            Button.gameObject.GetComponent<Image>().sprite = CardSprites[Rand.Next(0, 7)];
        }

        StartChoice.SetActive(true);

        ViewSwitchScript.SetViewRoom();
    }

    void ExitStartRoom()
    {
        foreach (GameObject Element in ElementsToHide)
        {
            Element.SetActive(true);
        }

        StartChoice.SetActive(false);

        ViewSwitchScript.SetViewMap();
    }

    void Choose1()
    {
        Debug.Log("Chooooose 1");
        ExitStartRoom();
    }

    void Choose2()
    {
        Debug.Log("Chooooose 2");
        ExitStartRoom();
    }

    void Choose3()
    {
        Debug.Log("Chooooose 3");
        ExitStartRoom();
    }
}
