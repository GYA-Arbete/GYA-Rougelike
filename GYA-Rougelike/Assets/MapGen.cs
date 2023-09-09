using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class MapGen : MonoBehaviour
{
    public Button m_YourFirstButton;
    public Transform[] children;

    public GameObject MapPrefab;
    public Transform MapPrefabParent;

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
        System.Random rand = new System.Random();

        // Slumpa mängden element per kolumn
        // Upper bounds is not inclusive, eg Next(1, 4) == 1, 2 or 3
        int[]RoomCount = { rand.Next(1, 4), rand.Next(1, 4), rand.Next(1, 4), rand.Next(1, 4), rand.Next(1, 4) };

        // Bestäm vilken / vilka spawnpoints som ska bli rum
        byte[] SpawnPoint = new byte[5];

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
                            SpawnPoint[i] = 100;
                            break;
                        case 1:
                            SpawnPoint[i] = 010;
                            break;
                        case 2:
                            SpawnPoint[i] = 001;
                            break;
                    }
                    break;
                case 2:
                    int temp2 = rand.Next(0, 3);
                    switch (temp2)
                    {
                        case 0:
                            SpawnPoint[i] = 110;
                            break;
                        case 1:
                            SpawnPoint[i] = 101;
                            break;
                        case 2:
                            SpawnPoint[i] = 011;
                            break;
                    }
                    break;
                case 3:
                    SpawnPoint[i] = 111;
                    break;
            }
        }

        // This returns the GameObject named "xxxx".
        //Prefab = GameObject.Find("Hand");

        // Skapa punkt vid varje spawnpoint
        // https://docs.unity3d.com/ScriptReference/Object.Instantiate.html
        // Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent);
        Instantiate(MapPrefab, new Vector3(1, 1, 0), new Quaternion(0, 0, 0, 0), MapPrefabParent);

        // Skriv ut värden i Log
        for (int i = 0; i < 5; i++)
        {
            Debug.Log($"Column Number {i}");
            Debug.Log($"RoomCount: {RoomCount[i]}");
            Debug.Log($"SpawnPoint: {SpawnPoint[i]}");
        }
    }
}
