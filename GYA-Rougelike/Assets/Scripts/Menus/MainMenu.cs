using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Mirror.Discovery;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    public NetworkManager NetworkManager;
    public NetworkDiscovery NetworkDiscovery;

    [Header("Main Menu Items")]
    public Button PlayGameButton;
    public Button OptionsButton;
    public Button QuitGameButton;
    public RectTransform MainButtons;

    [Header("Play Buttons")]
    public Button PlayGoBackButton;
    public Button HostButton;
    public Button JoinButton;
    public RectTransform PlayButtons;

    private readonly float Offset = 3.7f;
    private Dictionary<long, ServerResponse> DiscoveredServers = new Dictionary<long, ServerResponse>();

    // Start is called before the first frame update
    void Start()
    {
        PlayGameButton.onClick.AddListener(Play);
        OptionsButton.onClick.AddListener(OpenOptions);
        QuitGameButton.onClick.AddListener(QuitGame);

        HostButton.onClick.AddListener(HostGame);
        JoinButton.onClick.AddListener(JoinGame);
        PlayGoBackButton.onClick.AddListener(PlayGoBack);
    }

    void Play()
    {
        MainButtons.position -= new Vector3(Offset, 0f, 0f);
        PlayButtons.position -= new Vector3(Offset, 0f, 0f);
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

    void HostGame()
    {
        DiscoveredServers.Clear();
        NetworkManager.StartHost();
        NetworkDiscovery.AdvertiseServer();
    }

    void JoinGame()
    {
        DiscoveredServers.Clear();
        NetworkDiscovery.StartDiscovery();
    }

    public void OnDiscoveredServer(ServerResponse info)
    {
        NetworkManager.StartClient(info.uri);
    }

    void PlayGoBack()
    {
        MainButtons.position += new Vector3(Offset, 0f, 0f);
        PlayButtons.position += new Vector3(Offset, 0f, 0f);
    }
}
