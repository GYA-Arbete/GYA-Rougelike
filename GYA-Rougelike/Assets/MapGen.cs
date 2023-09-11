using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MapGen : MonoBehaviour
{
    // Fancy formating stuff for Inspector
    [Header("Buttons")]

    public Button GenerateButton;
    public Button ClearMapButton;

    [Space]
    [Header("Map Room Bullshit")]

    public Transform MapPrefabParent;
    public GameObject MapPrefab;

    [Space]
    [Header("Map Line Bullshit")]

    public Transform LinePrefabParent;
    public GameObject LinePrefab;

    [Space]
    [Header("Arrays")]

    public Transform[] children;
    public Transform[] Clones;
    public Transform[] Lines;

    // Start is called before the first frame update
    void Start()
    {
        GenerateButton.onClick.AddListener(GenerateMapBtnPressed);

        ClearMapButton.onClick.AddListener(ClearMapBtnPressed);

        // Put parent + children into array
        children = GetComponentsInChildren<Transform>();
    }

    void GenerateMapBtnPressed()
    {
        StartCoroutine(CreateMap());
    }

    // FatPerson115 saving my ass
    IEnumerator CreateMap()
    {
        DeleteMap();

        // yield on a new YieldInstruction that waits for 0.1 seconds.
        // 0.1f to tell stupid compiler its float, not double
        yield return new WaitForSeconds(0.05f);

        GenerateRooms();

        GeneratePaths();
    }

    void ClearMapBtnPressed()
    {
        DeleteMap();
    }

    // Function for deleting all elements of previously generated map
    void DeleteMap()
    {
        if (Clones != null )
        {
            for (int i = 2; i < Clones.Length; i++)
            {
                if (Clones[i] != null)
                {
                    // Destroy object
                    Destroy(Clones[i].gameObject);
                }
            }
        }

        if (Lines != null)
        {
            for (int i = 2; i < Lines.Length; i++)
            {
                if (Lines[i] != null)
                {
                    // Destroy object
                    Destroy(Lines[i].gameObject);
                }
            }
        }

        // Clear Arrays
        Clones = null;
        Lines = null;
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
        // Amount of Lines, ignore index 0 & 1 as they are Parent and Template
        int LineCount = 2;

        for (int i = 2; i < Clones.Length; i++)
        {
            for (int j = 2; j < Clones.Length; j++)
            {
                // Så att den inte kollar avstånd mellan objekt n och objekt n
                if (i != j)
                {
                    // Calc delta x & delta y
                    float dX = Math.Abs(Clones[i].position.x - Clones[j].position.x);
                    float dY = Math.Abs(Clones[i].position.y - Clones[j].position.y);

                    // Om de inte ligger övanför varandra och avståndet är mindre än roten ur 2, eg ~ 1.4
                    if (dX > 0 && dX <= Math.Sqrt(2) && dY <= Math.Sqrt(2))
                    {
                        // Clear array
                        Lines = null;

                        // Skapar en klon av ett tomt objekt som håller en LineRenderer, detta då varje LineRenderer bara kan ha en linje
                        Instantiate(LinePrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), LinePrefabParent);

                        // Put all clones + parent into Array
                        Lines = LinePrefabParent.GetComponentsInChildren<Transform>();

                        LineRenderer LineRend = Lines[LineCount].GetComponent<LineRenderer>();

                        // Start-position för linjen
                        LineRend.SetPosition(0, new Vector3(Clones[i].position.x, Clones[i].position.y, 0));

                        // Om 1:a eller 4:e kvadranten
                        if (Clones[j].position.x > Clones[i].position.x)
                        {
                            // Om 1:a kvadranten
                            if (Clones[j].position.y > Clones[i].position.y)
                            {
                                LineRend.SetPosition(1, new Vector3(Clones[i].position.x + dX, Clones[i].position.y + dY, 0));
                            }
                            // Om 4:e kvadranten
                            else
                            {
                                LineRend.SetPosition(1, new Vector3(Clones[i].position.x + dX, Clones[i].position.y - dY, 0));
                            }
                        }
                        // Om 2:a eller 3:e kvadranten
                        else
                        {
                            // Om 2:a kvadranten
                            if (Clones[j].position.y > Clones[i].position.y)
                            {
                                LineRend.SetPosition(1, new Vector3(Clones[i].position.x - dX, Clones[i].position.y + dY, 0));
                            }
                            // Om 3:e kvadranten
                            else
                            {
                                LineRend.SetPosition(1, new Vector3(Clones[i].position.x - dX, Clones[i].position.y - dY, 0));
                            }
                        }

                        LineCount++;
                    }
                }
            }
        }
    }
}
