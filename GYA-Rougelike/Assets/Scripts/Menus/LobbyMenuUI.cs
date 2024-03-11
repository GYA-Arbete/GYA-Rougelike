using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMenuUI : MonoBehaviour
{
    public Toggle[] Toggles = new Toggle[2];
    public Canvas MainCanvas;
    public Button StartButton;
    public TextMeshProUGUI ButtonText;
    public LobbyMenu LobbyMenuScript;

    public bool OverwritingToggleState;
    public int PlayerNumber = 0;

    void Start()
    {
        LobbyMenuScript = FindAnyObjectByType<LobbyMenu>();
        PlayerNumber = FindAnyObjectByType<NetworkManagerOverride>().numPlayers;
        // This is dumb as fuck but ensures the number doesnt fail to assign for Player 2
        if (PlayerNumber != 1)
        {
            PlayerNumber = 2;
        }

        Toggles[0].onValueChanged.AddListener(delegate { LobbyMenuScript.OnReadyToggled(Toggles[0], OverwritingToggleState, PlayerNumber); });
        Toggles[1].onValueChanged.AddListener(delegate { LobbyMenuScript.OnReadyToggled(Toggles[1], OverwritingToggleState, PlayerNumber); });
        StartButton.onClick.AddListener(LobbyMenuScript.StartGame);
    }

    public void SetupUI()
    {
        MainCanvas.worldCamera = FindAnyObjectByType<CameraSwitch>().MapViewCamera;
    }

    public void UpdateButtonText(int ReadyPlayers)
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

    public void UpdateToggles(bool State1, bool State2)
    {
        // Prevents double-toggling of toggles
        OverwritingToggleState = true;
        Toggles[0].GetComponent<ToggleHandler>().ValueChanged(Toggles[0], State1);
        Toggles[1].GetComponent<ToggleHandler>().ValueChanged(Toggles[1], State2);
        OverwritingToggleState = false;
    }
}
