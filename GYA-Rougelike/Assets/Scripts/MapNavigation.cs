using UnityEngine;
using UnityEngine.UI;

public class MapNavigation : MonoBehaviour
{
    public Transform MainCamera;
    public Button ExitRoomButton;
    public Transform ExitRoomButtonParent;
    public GameObject MapUpDownScriptHolder;
    public int RoomType;

    // Start is called before the first frame update
    void Start()
    {
        MainCamera = Camera.main.transform;

        Button button = GetComponent<Button>();
        button.onClick.AddListener(EnterRoom);

        Object[] temp = Resources.FindObjectsOfTypeAll(typeof(GameObject));
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

        MapUpDownScriptHolder = gameObject.transform.parent.gameObject.transform.parent.gameObject;
    }

    void EnterRoom()
    {
        // Flyttar kameran till rätt position för att "se" rummet
        MainCamera.transform.position = new Vector3(0, -10, MainCamera.transform.position.z);


    }

    void ExitRoom()
    {
        // Flyttar kameran till rätt position för att "se" kartan
        MainCamera.transform.position = new Vector3(0, 0, MainCamera.transform.position.z);


    }
}
