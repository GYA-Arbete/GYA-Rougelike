using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class LobbyMenu : NetworkBehaviour
{
    public GameObject UIPrefab;
    public LobbyMenuUI LobbyUI = null;

    [Header("SyncVars")]
    [SyncVar(hook = nameof(HandleReadyPlayersChanged))]
    public int ReadyPlayers = 0;
    // SyncLists suck mega ass, so heres 2 SyncVars instead
    [SyncVar(hook = nameof(HandleToggleStateChanged))]
    public bool ToggleState1 = false;
    [SyncVar(hook = nameof(HandleToggleStateChanged))]
    public bool ToggleState2 = false;

    [Header("Other Scripts")]
    public StartRoom StartRoomScript;

    // Events that the LobbyUI will subscribe to
    public event System.Action<int> OnReadyPlayersChanged;
    public event System.Action<bool, bool> OnToggleStateChanged;

    [Command(requiresAuthority=false)]
    public void StartGame()
    {
        // Yes this is dumb but it has to be done because:
        // 1. We want to hide the LobbyUI for all players
        // 2. We only want to call EnterStartRoom once
        HideLobbyUI();
        StartRoomScript.EnterStartRoom();
    }

    [ClientRpc]
    void HideLobbyUI()
    {
        LobbyUI.MainCanvas.gameObject.SetActive(false);
    }

    public void OnReadyToggled(Toggle toggle, bool OverwritingState, int PlayerNumber)
    {
        if (!OverwritingState)
        {
            ToggleHandler Handler = toggle.GetComponent<ToggleHandler>();
            int ID = Handler.ID;

            if (ID == PlayerNumber - 1)
            {
                UpdateSyncvars(ID, toggle.isOn);
            }
            else
            {
                // When player doesnt have authority, reset toggle to previous state
                LobbyUI.OverwritingToggleState = true;
                Handler.ValueChanged(toggle, !toggle.isOn);
                LobbyUI.OverwritingToggleState = false;
            }
        }
    }

    // Changes syncvar values in command so it updates for every user no matter which user caused the change
    [Command(requiresAuthority=false)]
    public void UpdateSyncvars(int ID, bool State)
    {
        // Yes this sucks but idk how to make it not suck
        if (ID == 0)
        {
            if (State)
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
            if (State)
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
    public void HandleReadyPlayersChanged(int oldValue, int newValue) { OnReadyPlayersChanged?.Invoke(ReadyPlayers); }
    public void HandleToggleStateChanged(bool oldValue, bool newValue) { OnToggleStateChanged?.Invoke(ToggleState1, ToggleState2); }

    // Called on every NetworkBehaviour when it is activated on a client.
    // Objects on the host have this function called, as there is a local client on the host.
    // The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.
    public override void OnStartClient()
    {
        // Spawn a seperate instance of the UI on each Client
        GameObject ClientSideUI = Instantiate(UIPrefab, transform);

        LobbyUI = ClientSideUI.GetComponent<LobbyMenuUI>();

        // wire up all events to handlers in PlayerUI
        OnReadyPlayersChanged = LobbyUI.UpdateButtonText;
        OnToggleStateChanged = LobbyUI.UpdateToggles;

        // Invoke all event handlers with the initial data
        OnReadyPlayersChanged.Invoke(ReadyPlayers);
        OnToggleStateChanged.Invoke(ToggleState1, ToggleState2);
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
