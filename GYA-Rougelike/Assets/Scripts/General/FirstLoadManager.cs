using UnityEngine;

// A simple script to handle the first load from main menu
// and making sure functions are executed in the right order
// so everything needed is actually loaded
public class FirstLoadManager : MonoBehaviour
{
    [Header("Other Scripts")]
    public CardInventory CardInventoryScript;
    public StartRoom StartRoomScript;
    public PlayerManager PlayerManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        CardInventoryScript.GetCardTypes();

        CardInventoryScript.ResetInventory();

        PlayerManagerScript.SpawnPlayer();

        StartRoomScript.EnterStartRoom();
    }
}
