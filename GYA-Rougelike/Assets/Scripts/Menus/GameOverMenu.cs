using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : NetworkBehaviour
{
    [Header("Viewable Elements")]
    public GameObject GameOverMenuCanvas;
    public Button QuitButton;
    public Button RestartButton;

    [Header("Other Scripts")]
    public PauseMenu PauseMenuScript;

    // Start is called before the first frame update
    void Start()
    {
        QuitButton.onClick.AddListener(Quit);
        RestartButton.onClick.AddListener(Restart);
    }

    [ClientRpc]
    public void SetGameOverMenuVisibility(bool State)
    {
        GameOverMenuCanvas.SetActive(State);
    }

    [Command(requiresAuthority = false)]
    void Quit()
    {
        SetGameOverMenuVisibility(false);
        PauseMenuScript.Quit();
    }

    [Command(requiresAuthority = false)]
    void Restart()
    {
        SetGameOverMenuVisibility(false);
        PauseMenuScript.Restart();
    }
}
