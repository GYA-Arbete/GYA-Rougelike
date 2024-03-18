using Mirror;
using UnityEngine;

public class StartRoom : NetworkBehaviour
{
    [Header("Other Scripts")]
    public CameraSwitch CameraSwitchScript;
    public CardInventory CardInventoryScript;
    public MapGen MapGenScript;
    public CardChoice CardChoiceScript;
    public PlayerManager PlayerManagerScript;

    public void EnterStartRoom()
    {
        PlayerManagerScript.SetPlayerSpriteVisibility(false);

        CameraSwitchScript.SetViewToRoom();

        CardInventoryScript.ResetInventory();

        CardChoiceScript.StartChoice("StartRoom", false);
    }

    [Command(requiresAuthority=false)]
    public void ExitStartRoom()
    {
        PlayerManagerScript.SetPlayerSpriteVisibility(true);

        StartCoroutine(MapGenScript.CreateMap());

        CameraSwitchScript.SetViewToMap();
    }
}
