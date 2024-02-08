using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class LobbyMenu : NetworkBehaviour
{
    public GameObject UIPrefab;
    public LobbyMenuUI LobbyUI;

    [Header("Ready Toggles")]
    [SyncVar(hook = nameof(HandleReadyPlayersChanged))]
    public int ReadyPlayers = 0;
    // SyncLists suck mega ass, so heres 2 SyncVars instead
    [SyncVar(hook = nameof(HandleToggleStateChanged))]
    public bool ToggleState1 = false;
    [SyncVar(hook = nameof(HandleToggleStateChanged))]
    public bool ToggleState2 = false;

    [Space]
    public NetworkManagerOverride NetworkManager;
    public NetworkIdentity Identity;

    // Start is called before the first frame update
    void Start()
    {
        NetworkManager = FindAnyObjectByType<NetworkManagerOverride>();
    }

    public void OnReadyToggled(Toggle toggle, bool OverwritingState)
    {
        if (!OverwritingState)
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
    }

    // DO NOT REMOVE ANY VARIABLES, IT WILL CAUSE ERRORS
    public void HandleReadyPlayersChanged(int oldValue, int newValue) => LobbyUI.UpdateButtonText(ReadyPlayers);
    public void HandleToggleStateChanged(bool oldValue, bool newValue) => LobbyUI.UpdateToggles(ToggleState1, ToggleState2);

    // Called on every NetworkBehaviour when it is activated on a client.
    // Objects on the host have this function called, as there is a local client on the host.
    // The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.
    public override void OnStartClient()
    {
        // Spawn a seperate instance of the UI on each Client
        GameObject ClientSideUI = Instantiate(UIPrefab);

        LobbyUI = ClientSideUI.GetComponent<LobbyMenuUI>();
    }

    // Called when the local player object has been set up.
    // This happens after OnStartClient(), as it is triggered by an ownership message from the server. 
    // This is an appropriate place to activate components or functionality that should only be active for the local player, such as cameras and input.
    public override void OnStartLocalPlayer()
    {
        // Set starting stuff for UI
        LobbyUI.SetupUI();
    }
}
