using UnityEngine;
using UnityEngine.UI;

public class CampRoom : MonoBehaviour
{
    public bool InCampRoom = false;

    [Header("Viewable Elements")]
    public GameObject CampRoomCanvas;
    public Button RestButton;
    public GameObject[] Players;

    [Header("Other Scripts")]
    public CameraSwitch CameraSwitchScript;

    // Start is called before the first frame update
    void Start()
    {
        RestButton.onClick.AddListener(Rest);
    }

    public void EnterCampRoom()
    {
        InCampRoom = true;

        CampRoomCanvas.SetActive(true);

        CameraSwitchScript.SetViewToRoom();
    }

    public void ExitCampRoom()
    {
        InCampRoom = false;

        CampRoomCanvas.SetActive(false);

        CameraSwitchScript.SetViewToMap();
    }

    void Rest()
    {
        foreach (GameObject Player in Players)
        {
            if (Player != null)
            {
                HealthSystem HealthSystemScript = Player.GetComponent<HealthSystem>();

                // Add back 10 health
                HealthSystemScript.Heal(10);
            }
        }

        ExitCampRoom();
    }
}
