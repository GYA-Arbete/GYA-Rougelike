using UnityEngine;
using UnityEngine.UI;
using System;

public class MapNavigation : MonoBehaviour
{
    public Transform MainCamera;
    public Button ExitRoomButton;
    public int RoomType;
    public PullMapUpDown CameraMoveScript;
    public MapGen MapGenScript;

    public Transform[] Rooms;
    public int CurrentRoom;
    public GameObject PreviousRoom;

    public Texture Cleared;
    public Texture Selected;

    // Start is called before the first frame update
    void Start()
    {
        MainCamera = Camera.main.transform;

        Button button = GetComponent<Button>();
        button.onClick.AddListener(EnterRoom);

        UnityEngine.Object[] temp = Resources.FindObjectsOfTypeAll(typeof(GameObject));
        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i].name == "ExitRoomButton")
            {
                GameObject temp2 = (GameObject)temp[i];
                ExitRoomButton = temp2.GetComponent<Button>();
                break;
            }
            
        }
        ExitRoomButton.onClick.AddListener(ExitRoom);

        RoomType = RoomTypeGen.RoomType;

        CameraMoveScript = FindObjectOfType<PullMapUpDown>();
        MapGenScript = FindObjectOfType<MapGen>();
    }

    void EnterRoom()
    {
        Rooms = GameObject.Find("SpawnPoints").GetComponent<MapGen>().Rooms;
        CurrentRoom = GameObject.Find("SpawnPoints").GetComponent<MapGen>().CurrentRoom;
        PreviousRoom = GameObject.Find("SpawnPoints").GetComponent<MapGen>().PreviousRoom;

        for (int i = 1; i < Rooms.Length; i++)
        {
            if (Rooms[i].name == gameObject.name)
            {
                if (i == CurrentRoom + 1 || i == CurrentRoom + 2 || i == CurrentRoom + 3)
                {
                    // Calc delta x & delta y
                    float dX = Math.Abs(gameObject.transform.position.x - PreviousRoom.transform.position.x);
                    float dY = Math.Abs(gameObject.transform.position.y - PreviousRoom.transform.position.y);

                    // Om de inte ligger övanför varandra och avståndet är mindre än roten ur 8, eg ~ 2.8, och ligger till höger på kartan
                    if (dX > 0 && dX <= Math.Sqrt(8) && dY <= Math.Sqrt(8) && gameObject.transform.position.x > PreviousRoom.transform.position.x)
                    {
                        CurrentRoom = i;
                        PreviousRoom = gameObject;

                        // Change image
                        Transform[] Children;
                        Children = GetComponentsInChildren<Transform>();
                        RawImage RawImg = Children[1].GetComponent<RawImage>();
                        RawImg.texture = Cleared;

                        MapGenScript.UpdateOtherScriptShit(CurrentRoom, PreviousRoom);
                        CameraMoveScript.MapUpDown();
                    }
                }
            }
        }
    }

    void ExitRoom()
    {
        CameraMoveScript.MapUpDown();
    }
}
