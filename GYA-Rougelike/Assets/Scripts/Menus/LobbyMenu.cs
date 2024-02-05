using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class LobbyMenu : NetworkBehaviour
{
    [Header("Ready Toggles")]
    public Toggle[] Toggles;
    [SyncVar(hook = nameof(HandleReadyPlayersChanged))]
    public int ReadyPlayers = 0;
    // SyncLists suck mega ass, so heres 2 SyncVars instead
    [SyncVar(hook = nameof(HandleToggleStateChanged))]
    public bool ToggleState1 = false;
    public bool OldState1 = false;
    [SyncVar(hook = nameof(HandleToggleStateChanged))]
    public bool ToggleState2 = false;
    public bool OldState2 = false;

    [Header("Elements")]
    public GameObject LobbyMenuHolder;
    public Button StartButton;
    public TextMeshProUGUI ButtonText;
    public Canvas MainCanvas;

    [Space]
    public NetworkManagerOverride NetworkManager;
    public NetworkIdentity Identity;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = FindAnyObjectByType<CameraSwitch>().MapViewCamera.transform.position;
        MainCanvas.worldCamera = FindAnyObjectByType<CameraSwitch>().MapViewCamera;

        Toggles[0].onValueChanged.AddListener(delegate { OnReadyToggled(Toggles[0]); });
        Toggles[1].onValueChanged.AddListener(delegate { OnReadyToggled(Toggles[1]); });

        NetworkManager = FindAnyObjectByType<NetworkManagerOverride>();

        UpdateLobbyUi();
    }

    void OnReadyToggled(Toggle toggle)
    {
        int ID = toggle.GetComponent<ToggleHandler>().ID;

        // Yes this sucks but idk how to make it not suck
        if (ID == 0)
        {
            if (toggle.isOn)
            {
                ReadyPlayers++;
                ToggleState1 = true;
            }
            else
            {
                ReadyPlayers--;
                ToggleState1 = false;
            }
        }
        else // ID == 1
        {
            if (toggle.isOn)
            {
                ReadyPlayers++;
                ToggleState2 = true;
            }
            else
            {
                ReadyPlayers--;
                ToggleState2 = false;
            }
        }
    }

    // DO NOT REMOVE ANY VARIABLES, IT WILL CAUSE ERRORS
    public void HandleReadyPlayersChanged(int oldValue, int newValue) => UpdateLobbyUi();
    public void HandleToggleStateChanged(bool oldValue, bool newValue) => UpdateLobbyUi();

    public void UpdateLobbyUi()
    {
        /*
        if (!Identity.isOwned)
        {
            foreach (LobbyMenu Script in NetworkManager.ClientsLobbyMenuScripts)
            {
                if (Script.Identity.isOwned)
                {
                    Script.UpdateLobbyUi();
                    break;
                }
            }

            return;
        }
        */

        switch (ReadyPlayers)
        {
            case 0:
                ButtonText.text = "0/2 Ready";
                StartButton.interactable = false;
                break;
            case 1:
                ButtonText.text = "1/2 Ready";
                StartButton.interactable = false;
                break;
            case 2:
                ButtonText.text = "Start";
                StartButton.interactable = true;
                break;
        }

        Toggles[0].GetComponent<ToggleHandler>().ValueChanged(null, ToggleState1);
        Toggles[1].GetComponent<ToggleHandler>().ValueChanged(null, ToggleState2);
    }
}
