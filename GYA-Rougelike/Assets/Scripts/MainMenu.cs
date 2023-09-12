using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Main Menu Items")]

    public Button PlayGameButton;
    public Button QuitGameButton;

    // Start is called before the first frame update
    void Start()
    {
        PlayGameButton.onClick.AddListener(StartGame);
        QuitGameButton.onClick.AddListener(QuitGame);
    }

    void StartGame()
    {
        SceneManager.LoadScene("MapGenerator");
    }

    void QuitGame()
    {
        // Quits the game WOO
        Application.Quit();
        Debug.Log("I QUIT!!");
    }
}
