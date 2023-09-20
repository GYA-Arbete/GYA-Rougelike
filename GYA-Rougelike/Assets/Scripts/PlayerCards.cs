using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCards : MonoBehaviour
{
    public string JsonString;

    public CardList cardList;

    [System.Serializable]
    public class CardList
    {

        public List<Cards> cardList;
    }

    [System.Serializable]
    public class Cards
    {
        public int energy { get; set; }
        public int damage { get; set; }
        public int defence { get; set; }
        public int cooldown { get; set; }
    }

    // Start is called before the first frame update
    void Start()
    {
        cardList = JsonUtility.FromJson<CardList>(JsonString);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
