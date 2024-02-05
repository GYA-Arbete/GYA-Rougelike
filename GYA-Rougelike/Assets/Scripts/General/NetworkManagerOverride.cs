using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManagerOverride : NetworkManager
{
    [Header("Override Vars")]
    public GameObject LobbyPrefab;
    public List<LobbyMenu> ClientsLobbyMenuScripts;

    [Server]
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject LobbyInstance = Instantiate(LobbyPrefab);

        ClientsLobbyMenuScripts.Add(LobbyInstance.GetComponent<LobbyMenu>());

        NetworkServer.Spawn(LobbyInstance, conn);

        base.OnServerAddPlayer(conn);
    }
}
