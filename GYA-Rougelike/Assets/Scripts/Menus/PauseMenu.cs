using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PauseMenu : NetworkBehaviour
{
    [Header("Viewable Elements")]
    public GameObject PauseMenuCanvas;
    public Button UnpauseButton;
    public Button RestartButton;
    public Button QuitButton;

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
            PauseMenuCanvas.SetActive(!PauseMenuCanvas.activeSelf);
        }
    }

    void Unpause()
    {
        PauseMenuCanvas.SetActive(false);
    }

    [ClientRpc]
    // Forces all clients to unpause (only done on game restart)
    void RpcUnpause()
    {
        PauseMenuCanvas.SetActive(false);
    }

    [Command(requiresAuthority = false)]
    void Restart()
    {
        // If in combat, end combat
        if (CombatSystemScript.InCombat)
        {
            CombatSystemScript.EndCombat();
            CardSpawnerScript.DespawnCards();
        }
        // If Canvas is active, meaning currently in LootRoom
        else if (LootRoomScript.LootRoomCanvas.activeSelf)
        {
            LootRoomScript.ExitLootRoom();
        }
        // If CampRoomCanvas is active, meaning currently in CampRoom
        else if (CampRoomScript.CampRoomCanvas.activeSelf)
        {
            CampRoomScript.ExitCampRoom();
        }

        MapGenScript.DeleteMap();

        StartRoomScript.EnterStartRoom();

        PlayerManagerScript.ResetPlayers();

        RpcUnpause();
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
