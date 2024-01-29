using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Main Menu Items")]
    public Button PlayGameButton;
    public Button OptionsButton;
    public Button QuitGameButton;

    // Start is called before the first frame update
    void Start()
    {
        PlayGameButton.onClick.AddListener(StartGame);
        OptionsButton.onClick.AddListener(OpenOptions);
        QuitGameButton.onClick.AddListener(QuitGame);
    }

    void StartGame()
    {
        SceneManager.LoadScene("MapGenerator");
    }

    void OpenOptions()
    {
        // Do stuff
    }

    void QuitGame()
    {
        // Quits the game WOO
        Application.Quit();
        Debug.Log("I QUIT!!");
    }
}
