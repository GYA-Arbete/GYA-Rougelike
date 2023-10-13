using UnityEngine;
using UnityEngine.UI;

public class PullMapUpDown : MonoBehaviour
{
    [Header("Buttons")]
    public Button PullMapUpDownButton;

    [Header("Cameras")]
    public Camera MapViewCamera;
    public Camera RoomViewCamera;

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
        }
        else
        {
            MapViewCamera.enabled = true;
            RoomViewCamera.enabled = false;
        }
    }
}
