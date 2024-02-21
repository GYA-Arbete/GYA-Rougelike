using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CameraSwitch : NetworkBehaviour
{
    [Header("Viewable Elements")]
    public Canvas PauseMenuCanvas;
    public Button PullMapUpDownButton;
    public GameObject Map;

    [Header("Cameras")]
    public Camera MapViewCamera;
    public Camera RoomViewCamera;

    // Start is called before the first frame update
    void Start()
    {
        PullMapUpDownButton.onClick.AddListener(SwitchView);

        MapViewCamera.enabled = false;
    }

    public void SwitchView()
    {
        if (MapViewCamera.enabled == true)
        {
            SetViewToRoom();
        }
        else
        {
            SetViewToMap();
        }
    }

    [ClientRpc]
    public void SetViewToRoom()
    {
        MapViewCamera.enabled = false;
        RoomViewCamera.enabled = true;

        // Disable the map to avoid stupid bug (See issue #56)
        Map.SetActive(false);

        // Set which camera is used to render PauseMenu
        PauseMenuCanvas.worldCamera = RoomViewCamera;
    }

    [ClientRpc]
    public void SetViewToMap()
    {
        MapViewCamera.enabled = true;
        RoomViewCamera.enabled = false;

        // Disable the map to avoid stupid bug (See issue #56)
        Map.SetActive(true);

        // Set which camera is used to render PauseMenu
        PauseMenuCanvas.worldCamera = MapViewCamera;
    }
}
