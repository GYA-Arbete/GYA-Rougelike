using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PauseMenu : MonoBehaviour
{
    [Header("Viewable Elements")]
    public GameObject PauseMenuCanvas;
    public Button UnpauseButton;
    public Button RestartButton;
    public Button QuitButton;

    [Header("Variables")]
    public bool IsPaused = false;

    [Header("Other Scripts")]
    public MapGen MapGenScript;
    public StartRoom StartRoomScript;
    public LootRoom LootRoomScript;
    public CampRoom CampRoomScript;
    public CombatSystem CombatSystemScript;
    public CardSpawner CardSpawnerScript;
    public PlayerManager PlayerManagerScript;

    void Start()
    {
        UnpauseButton.onClick.AddListener(Unpause);
        RestartButton.onClick.AddListener(Restart);
        QuitButton.onClick.AddListener(Quit);
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
        // If in combat, end combat
        if (CombatSystemScript.InCombat)
        {
            CombatSystemScript.EndCombat();
            CardSpawnerScript.DespawnCards();
        }
        else if (LootRoomScript.InLootRoom)
        {
            LootRoomScript.ExitLootRoom();
        }
        else if (CampRoomScript.InCampRoom)
        {
            CampRoomScript.ExitCampRoom();
        }

        MapGenScript.DeleteMap();

        StartRoomScript.EnterStartRoom();

        PlayerManagerScript.ResetPlayers();

        Unpause();
    }

    void Quit()
    {
        NetworkManager NetworkManager = FindAnyObjectByType<NetworkManager>();

        if (NetworkManager.mode == NetworkManagerMode.Host)
        {
            NetworkManager.StopHost();
        }
        else
        {
            NetworkManager.StopClient();
        }
    }
}
