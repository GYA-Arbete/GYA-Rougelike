using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMenuUI : MonoBehaviour
{
    public Toggle[] Toggles = new Toggle[2];
    public Canvas MainCanvas;
    public Button StartButton;
    public Button ReadyButton;
    public TextMeshProUGUI ButtonText;
    public LobbyMenu LobbyMenuScript;

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

        StartButton.onClick.AddListener(LobbyMenuScript.StartGame);
        ReadyButton.onClick.AddListener(UpdateReadyButtonText);
    }

    public void SetupUI()
    {
        MainCanvas.worldCamera = FindAnyObjectByType<CameraSwitch>().MapViewCamera;
    }

    void UpdateReadyButtonText()
    {
        TextMeshProUGUI ReadyButtonText = ReadyButton.GetComponentInChildren<TextMeshProUGUI>();
        if (ReadyButtonText.text == "Ready")
        {
            ReadyButtonText.text = "Unready";
            LobbyMenuScript.OnReadyToggled(Toggles[PlayerNumber - 1], PlayerNumber, true);
        }
        else
        {
            ReadyButtonText.text = "Ready";
            LobbyMenuScript.OnReadyToggled(Toggles[PlayerNumber - 1], PlayerNumber, false);
        }
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
        Toggles[0].GetComponent<ToggleHandler>().ValueChanged(Toggles[0], State1);
        Toggles[1].GetComponent<ToggleHandler>().ValueChanged(Toggles[1], State2);
    }
}
