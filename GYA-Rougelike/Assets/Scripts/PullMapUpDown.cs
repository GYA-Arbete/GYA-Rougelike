using UnityEngine;
using UnityEngine.UI;

public class PullMapUpDown : MonoBehaviour
{
    [Header("Buttons")]
    public Button PullMapUpDownButton;

    [Header("Cameras")]
    public Camera MapViewCamera;
    public Camera RoomViewCamera;

    public Canvas PauseMenu;

    // Start is called before the first frame update
    void Start()
    {
        PullMapUpDownButton.onClick.AddListener(MapUpDown);

        RoomViewCamera.enabled = false;
    }

    public void MapUpDown()
    {
        if (MapViewCamera.enabled == true)
        {
            SetViewRoom();
        }
        else
        {
            SetViewMap();
        }
    }

    public void SetViewRoom()
    {
        MapViewCamera.enabled = false;
        RoomViewCamera.enabled = true;

        PauseMenu.worldCamera = RoomViewCamera;
    }

    public void SetViewMap()
    {
        MapViewCamera.enabled = true;
        RoomViewCamera.enabled = false;

        PauseMenu.worldCamera = MapViewCamera;
    }
}
