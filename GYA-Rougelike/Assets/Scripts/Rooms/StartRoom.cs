using UnityEngine;

public class StartRoom : MonoBehaviour
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
        foreach (GameObject Element in ElementsToHide)
        {
            Element.SetActive(false);
        }

        CameraSwitchScript.SetViewToRoom();

        CardInventoryScript.ResetInventory();

        CardChoiceScript.StartChoice("StartRoom", false);
    }

    public void ExitStartRoom()
    {
        foreach (GameObject Element in ElementsToHide)
        {
            Element.SetActive(true);
        }

        StartCoroutine(MapGenScript.CreateMap());

        CameraSwitchScript.SetViewToMap();
    }
}
