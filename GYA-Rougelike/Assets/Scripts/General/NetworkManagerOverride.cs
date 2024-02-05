using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManagerOverride : NetworkManager
{
    [Header("Override Vars")]
    public GameObject LobbyPrefab;
    public List<LobbyMenu> ClientsLobbyMenuScripts;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject roomPlayerInstance = Instantiate(LobbyPrefab);

        ClientsLobbyMenuScripts.Add(roomPlayerInstance.GetComponent<LobbyMenu>());

        NetworkServer.Spawn(roomPlayerInstance.gameObject, conn);
    }
}
