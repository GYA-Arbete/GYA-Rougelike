using System;
using UnityEngine;
using UnityEngine.UI;

public class MapGen : MonoBehaviour
{
    public Button m_YourFirstButton;
    public Transform[] children;

    public GameObject MapPrefab;
    public Transform MapPrefabParent;

    public Transform[] Clones;

    // Start is called before the first frame update
    void Start()
    {
        m_YourFirstButton.onClick.AddListener(TaskOnClick);

        // Put parent + children into array
        children = GetComponentsInChildren<Transform>();
    }

    /*
    // Update is called once per frame
    void Update()
    {
        
    }
    */

    void TaskOnClick()
    {
        /*
        ############################
        Delete Previous Map Elements
        ############################
        */

        // Put all clones + parent into Array
        Clones = MapPrefabParent.GetComponentsInChildren<Transform>();
        for (int  i = 0; i < Clones.Length; i++)
        {
            // Ignore the first 2, eg Parent and Template
            if (i > 1)
            {
                // Destroy object
                Destroy(Clones[i].gameObject);
            }
        }

        // Clear array
        Clones = null;

        /*
        ##################
        Generate Map Rooms
        ##################
        */

        System.Random rand = new System.Random();

        // Slumpa mängden element per kolumn
        // Upper bounds is not inclusive, eg Next(1, 4) == 1, 2 or 3
        int[]RoomCount = { rand.Next(1, 4), rand.Next(1, 4), rand.Next(1, 4), rand.Next(1, 4), rand.Next(1, 4) };

        // Bestäm vilken / vilka spawnpoints som ska bli rum
        string[] SpawnPoint = new string[5];

        // Bestäm spawnpoints
        for (int i = 0; i < 5; i++)
        {
            switch (RoomCount[i])
            {
                case 1:
                    int temp1 = rand.Next(0, 3);
                    switch (temp1)
                    {
                        case 0:
                            SpawnPoint[i] = "100";
                            break;
                        case 1:
                            SpawnPoint[i] = "010";
                            break;
                        case 2:
                            SpawnPoint[i] = "001";
                            break;
                    }
                    break;
                case 2:
                    int temp2 = rand.Next(0, 3);
                    switch (temp2)
                    {
                        case 0:
                            SpawnPoint[i] = "110";
                            break;
                        case 1:
                            SpawnPoint[i] = "101";
                            break;
                        case 2:
                            SpawnPoint[i] = "011";
                            break;
                    }
                    break;
                case 3:
                    SpawnPoint[i] = "111";
                    break;
            }
        }

        // Int for which index of children to use
        int j = 2;

        // Skapa punkt vid varje spawnpoint
        for (int i = 0; i < 7; i++)
        {
            // Om StartPoint
            if (i == 0)
            {
                // https://docs.unity3d.com/ScriptReference/Object.Instantiate.html
                // Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent);
                Instantiate(MapPrefab, new Vector3(children[i + 1].position.x, children[i + 1].position.y, children[i + 1].position.z), new Quaternion(0, 0, 0, 0), MapPrefabParent);
            }
            // Om EndPoint
            else if (i == 6)
            {
                // https://docs.unity3d.com/ScriptReference/Object.Instantiate.html
                // Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent);
                Instantiate(MapPrefab, new Vector3(children[i + 11].position.x, children[i + 11].position.y, children[i + 11].position.z), new Quaternion(0, 0, 0, 0), MapPrefabParent);
            }
            // Om ngn vanlig kolumn
            else
            {
                foreach (char Number in SpawnPoint[i - 1])
                {
                    if (Number == '1')
                    {
                        Instantiate(MapPrefab, new Vector3(children[j].position.x, children[j].position.y, children[j].position.z), new Quaternion(0, 0, 0, 0), MapPrefabParent);
                    }

                    j++;
                }
            }
        }

        // Skriv ut värden i Log
        for (int i = 0; i < 5; i++)
        {
            Debug.Log($"Column Number {i}");
            Debug.Log($"RoomCount: {RoomCount[i]}");
            Debug.Log($"SpawnPoint: {SpawnPoint[i]}");
        }
    }
}
