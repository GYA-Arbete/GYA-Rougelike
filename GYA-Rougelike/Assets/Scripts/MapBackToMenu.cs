using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapBackToMenu : MonoBehaviour
{
    [Header("Button")]

    public Button BackToMenuButton;

    // Start is called before the first frame update
    void Start()
    {
        BackToMenuButton.onClick.AddListener(BackToMenu);
    }

    void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
