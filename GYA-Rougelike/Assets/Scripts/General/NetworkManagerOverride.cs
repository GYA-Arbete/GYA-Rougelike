using Mirror;
using UnityEngine;

public class NetworkManagerOverride : NetworkManager
{
    [Header("Override Vars")]
    public LobbyMenu LobbyMenuScript;

    // If join as client, give you authority to 2nd players objects
    public override void OnClientConnect()
    {
        if (LobbyMenuScript == null)
        {
            LobbyMenuScript = FindAnyObjectByType<LobbyMenu>();
        }

        LobbyMenuScript.PlayerConnected();

        base.OnClientConnect();
    }
}
