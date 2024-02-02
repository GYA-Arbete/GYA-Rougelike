using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class LobbyMenu : NetworkBehaviour
{
    [Header("Ready Toggles")]
    public Toggle[] Toggles;
    public int ReadyPlayers = 0;

    [Header("StartButton")]
    public Button StartButton;
    public TextMeshProUGUI ButtonText;

    public NetworkManager NetworkManager;

    // Start is called before the first frame update
    void Start()
    {
        Toggles[0].onValueChanged.AddListener(delegate { OnReadyToggled(Toggles[0]); });
        Toggles[1].onValueChanged.AddListener(delegate { OnReadyToggled(Toggles[1]); });

        Toggles[0].GetComponent<NetworkIdentity>().RemoveClientAuthority();
        Toggles[1].GetComponent<NetworkIdentity>().RemoveClientAuthority();

        UpdateButtonText();
    }

    // Give authority to the connected player over its own elements
    public void PlayerConnected()
    {
        Toggles[NetworkServer.connections.Count - 1].GetComponent<NetworkIdentity>().AssignClientAuthority(NetworkServer.connections[NetworkServer.connections.Count - 1]);
    }

    void OnReadyToggled(Toggle toggle)
    {
        if (toggle.isOn)
        {
            ReadyPlayers++;
        }
        else
        {
            ReadyPlayers--;
        }

        UpdateButtonText();
    }

    void UpdateButtonText()
    {
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
    }
}
