using Mirror;
using UnityEngine;

public class StartRoom : NetworkBehaviour
{
    [Header("Viewable Elements")]
    public GameObject[] ElementsToHide;

    [Header("Other Scripts")]
    public CameraSwitch CameraSwitchScript;
    public CardInventory CardInventoryScript;
    public MapGen MapGenScript;
    public CardChoice CardChoiceScript;

    public void EnterStartRoom()
    {
        ToggleOtherElementsVisibility();

        CameraSwitchScript.SetViewToRoom();

        CardInventoryScript.ResetInventory();

        CardChoiceScript.StartChoice("StartRoom", false);
    }

    [Command(requiresAuthority=false)]
    public void ExitStartRoom()
    {
        ToggleOtherElementsVisibility();

        StartCoroutine(MapGenScript.CreateMap());

        CameraSwitchScript.SetViewToMap();
    }

    [ClientRpc]
    void ToggleOtherElementsVisibility()
    {
        foreach (GameObject Element in ElementsToHide)
        {
            Element.SetActive(!Element.activeSelf);
        }
    }
}
