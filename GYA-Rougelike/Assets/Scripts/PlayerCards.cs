using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCards : MonoBehaviour
{
    public string JsonString;

    public Transform CardSpawner;
    public Transform[] CardSpawnPoints;

    public Transform CardsParent;

    public GameObject CardPrefab;

    public CardList cardList;

    [System.Serializable]
    public class CardList
    {
        public List<Cards> cardList;
    }

    [System.Serializable]
    public class Cards
    {
        public int Energy { get; set; }
        public int Damage { get; set; }
        public int Defence { get; set; }
        public int Cooldown { get; set; }
    }

    // Start is called before the first frame update
    void Start()
    {
        cardList = JsonUtility.FromJson<CardList>(JsonString);

        // Put parent + children into array
        CardSpawnPoints = GetComponentsInChildren<Transform>();

        SpawnCards();
    }

    void SpawnCards()
    {
        // i = 1 eftersom den ska ingorera parent
        for (int i = 1; i < CardSpawnPoints.Length; i++)
        {
            // https://docs.unity3d.com/ScriptReference/Object.Instantiate.html
            // Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent);
            Instantiate(CardPrefab, new Vector3(CardSpawnPoints[i].position.x, CardSpawnPoints[i].position.y, CardSpawnPoints[i].position.z), new Quaternion(0, 0, 0, 0), CardsParent);
        }
    }
}
