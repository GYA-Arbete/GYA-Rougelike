using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CampRoom : NetworkBehaviour
{
    [Header("Viewable Elements")]
    public GameObject CampRoomCanvas;
    public Button RestButton;
    public GameObject[] Players;

    [Header("Other Scripts")]
    public CameraSwitch CameraSwitchScript;
    public PlayerManager PlayerManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        RestButton.onClick.AddListener(Rest);
    }

    public void EnterCampRoom()
    {
        ToggleCanvasVisibility();
        PlayerManagerScript.SetHealthbarVisibility(false);

        CameraSwitchScript.SetViewToRoom();
    }

    [Command(requiresAuthority = false)]
    public void ExitCampRoom()
    {
        ToggleCanvasVisibility();
        PlayerManagerScript.SetHealthbarVisibility(true);

        CameraSwitchScript.SetViewToMap();
    }

    [ClientRpc]
    void ToggleCanvasVisibility()
    {
        CampRoomCanvas.SetActive(!CampRoomCanvas.activeSelf);
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
