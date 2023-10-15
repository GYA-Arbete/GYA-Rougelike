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
            MapViewCamera.enabled = false;
            RoomViewCamera.enabled = true;

            PauseMenu.worldCamera = RoomViewCamera;
        }
        else
        {
            MapViewCamera.enabled = true;
            RoomViewCamera.enabled = false;

            PauseMenu.worldCamera = MapViewCamera;
        }
    }
}
