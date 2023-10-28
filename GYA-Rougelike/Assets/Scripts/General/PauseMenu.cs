using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("Viewable Elements")]
    public GameObject PauseMenuCanvas;
    public Button UnpauseButton;
    public Button RestartButton;
    public Button MainMenuButton;

    [Header("Variables")]
    public bool IsPaused = false;

    [Header("Other Scripts")]
    public MapGen MapGenScript;
    public StartRoom StartRoomScript;

    void Start()
    {
        UnpauseButton.onClick.AddListener(Unpause);
        RestartButton.onClick.AddListener(Restart);
        MainMenuButton.onClick.AddListener(ToMainMenu);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (!IsPaused)
            {
                Pause();
            }
            else
            {
                Unpause();
            }
        }
    }

    void Pause()
    {
        PauseMenuCanvas.SetActive(true);
        IsPaused = true;
    }

    void Unpause()
    {
        PauseMenuCanvas.SetActive(false);
        IsPaused = false;
    }

    void Restart()
    {
        MapGenScript.DeleteMap();

        StartRoomScript.EnterStartRoom();

        Unpause();
    }

    void ToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
