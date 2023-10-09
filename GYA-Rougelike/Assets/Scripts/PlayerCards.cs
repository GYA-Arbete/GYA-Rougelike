using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCards : MonoBehaviour
{
    public TextAsset JsonString;

    public Transform CardSpawner;
    public Transform[] CardSpawnPoints;

    public Transform CardsParent;

    public GameObject CardPrefab;

    public CardList cardList;

    public int AvailableCards = 0;

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
        cardList = JsonUtility.FromJson<CardList>(JsonString.text);

        foreach (Cards card in cardList.cardList)
        {
            AvailableCards++;
        }

        // Put parent + children into array
        CardSpawnPoints = GetComponentsInChildren<Transform>();

        SpawnCards();
    }

    void SpawnCards()
    {
        // i = 1 eftersom den ska ingorera parent
        for (int i = 1; i < AvailableCards + 1; i++)
        {
            // https://docs.unity3d.com/ScriptReference/Object.Instantiate.html
            // Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent);
            GameObject Card = Instantiate(CardPrefab, new Vector3(CardSpawnPoints[i].position.x, CardSpawnPoints[i].position.y, CardSpawnPoints[i].position.z), new Quaternion(0, 0, 0, 0), CardsParent);

            // Assign values to each created card
            CardStats CardStatsScript = Card.GetComponent<CardStats>();
            CardStatsScript.AssignValues(cardList.cardList[i - 1].Energy, cardList.cardList[i - 1].Damage, cardList.cardList[i - 1].Defence, cardList.cardList[i - 1].Cooldown);
        }
    }
}
