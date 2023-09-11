using System;
using UnityEngine;
using UnityEngine.UI;

public class MapGen : MonoBehaviour
{
    // Fancy formating stuff for Inspector
    [Header("Buttons")]

    public Button GenerateButton;

    [Space]
    [Header("Prefab")]

    public Transform MapPrefabParent;
    public GameObject MapPrefab;

    [Space]
    [Header("Map Line Bullshit")]

    public Transform LinePrefabParent;
    public Transform LinePrefab;

    [Space]
    [Header("Arrays")]

    public Transform[] children;
    public Transform[] Clones;
    public Transform[] Lines;

    // Start is called before the first frame update
    void Start()
    {
        GenerateButton.onClick.AddListener(GenerateMapBtnPressed);

        // Put parent + children into array
        children = GetComponentsInChildren<Transform>();
    }

    void GenerateMapBtnPressed()
    {
        DeleteMap();

        GenerateRooms();

        GeneratePaths();
    }

    // Function for deleting all elements of previously generated map
    void DeleteMap()
    {
        // Put all clones + parent into Array
        Clones = MapPrefabParent.GetComponentsInChildren<Transform>();

        
        for (int i = 2; i < Clones.Length; i++)
        {
            if (Clones[i] != null)
            {
                // Destroy object
                Destroy(Clones[i].gameObject);
            }
        }

        /*
        for (int i = 2; i < Lines.Length; i++)
        {
            if (Lines[i] != null)
            {
                // Destroy object
                Destroy(Lines[i].gameObject);
            }
        }

        /*
        // Clear array
        if (Clones.Length > 2)
        {
            Array.Clear(Clones, 2, Clones.Length - 2);
        }

        if (Lines.Length > 2)
        {
            Array.Clear(Lines, 2, Lines.Length - 2);
        }
        */
        //Clones = null;
        //Lines = null;
    }

    // Function for generating rooms on map
    void GenerateRooms()
    {
        System.Random rand = new System.Random();

        // Slumpa mängden element per kolumn
        // Upper bounds is not inclusive, eg Next(1, 4) == 1, 2 or 3
        //int[] RoomCount = { rand.Next(1, 4), rand.Next(1, 4), rand.Next(1, 4), rand.Next(1, 4), rand.Next(1, 4) };
        int[] RoomCount = { 2, 2, 2, 2, 2 };

        // Bestäm vilken / vilka spawnpoints som ska bli rum
        int[] SpawnPoint = new int[children.Length];

        // Set SpawnPoints for StartRoom and End Room
        SpawnPoint[1] = 1;
        SpawnPoint[children.Length - 1] = 1;

        // Bestäm spawnpoints
        // 1 - 5 eftersom Column 1, 2, 3, 4 & 5
        for (int i = 1; i < 6; i++)
        {
            switch (RoomCount[i - 1])
            {
                /*
                case 1:
                    int temp1 = rand.Next(0, 3);
                    switch (temp1)
                    {
                        case 0:
                            SpawnPoint[i * 3 - 1] = 1;
                            SpawnPoint[i * 3] = 0;
                            SpawnPoint[i * 3 + 1] = 0;
                            break;
                        case 1:
                            SpawnPoint[i * 3 - 1] = 0;
                            SpawnPoint[i * 3] = 1;
                            SpawnPoint[i * 3 + 1] = 0;
                            break;
                        case 2:
                            SpawnPoint[i * 3 - 1] = 0;
                            SpawnPoint[i * 3] = 0;
                            SpawnPoint[i * 3 + 1] = 1;
                            break;
                    }
                    break;
                */
                case 2:
                    int temp2 = rand.Next(0, 3);
                    switch (temp2)
                    {
                        case 0:
                            SpawnPoint[i * 3 - 1] = 1;
                            SpawnPoint[i * 3] = 1;
                            SpawnPoint[i * 3 + 1] = 0;
                            break;
                        case 1:
                            SpawnPoint[i * 3 - 1] = 1;
                            SpawnPoint[i * 3] = 0;
                            SpawnPoint[i * 3 + 1] = 1;
                            break;
                        case 2:
                            SpawnPoint[i * 3 - 1] = 0;
                            SpawnPoint[i * 3] = 1;
                            SpawnPoint[i * 3 + 1] = 1;
                            break;
                    }
                    break;
                /*
                case 3:
                    SpawnPoint[i * 3 - 1] = 1;
                    SpawnPoint[i * 3] = 1;
                    SpawnPoint[i * 3 + 1] = 1;
                    break;
                */
            }
        }

        for (int i = 1; i < children.Length; i++)
        {
            if (SpawnPoint[i] == 1)
            {
                // https://docs.unity3d.com/ScriptReference/Object.Instantiate.html
                // Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent);
                Instantiate(MapPrefab, new Vector3(children[i].position.x, children[i].position.y, children[i].position.z), new Quaternion(0, 0, 0, 0), MapPrefabParent);
            }
        }

        // Put all clones + parent into Array
        Clones = MapPrefabParent.GetComponentsInChildren<Transform>();
    }

    // Function for generating paths between rooms
    void GeneratePaths()
    {
        //
        int LineCount = 2;

        for (int i = 2; i < Clones.Length; i++)
        {
            for (int j = 2; j < Clones.Length; j++)
            {
                // Så att den inte kollar avstånd mellan objekt n och objekt n
                if (i != j)
                {
                    // Calc delta x & delta y
                    float dX = System.Math.Abs(Clones[i].position.x - Clones[j].position.x);
                    float dY = System.Math.Abs(Clones[i].position.y - Clones[j].position.y);

                    if (dX < System.Math.Sqrt(2) && dY < System.Math.Sqrt(2))
                    {
                        // Clear array
                        Lines = null;

                        Instantiate(LinePrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), LinePrefabParent);

                        // Put all clones + parent into Array
                        Lines = LinePrefabParent.GetComponentsInChildren<Transform>();

                        LineRenderer LineRend = Lines[LineCount].GetComponent<LineRenderer>();

                        LineRend.SetPosition(0, new Vector3(Clones[i].position.x, Clones[i].position.y, 0));

                        if (j > i && dX > 0)
                        {
                            LineRend.SetPosition(1, new Vector3(Clones[i].position.x + dX, Clones[i].position.y + dY, 0));
                        }
                        else if (j < i && dX > 0)
                        {
                            LineRend.SetPosition(1, new Vector3(Clones[i].position.x - dX, Clones[i].position.y - dY, 0));
                        }

                        LineCount++;
                    }
                }
            }
        }
    }
}
