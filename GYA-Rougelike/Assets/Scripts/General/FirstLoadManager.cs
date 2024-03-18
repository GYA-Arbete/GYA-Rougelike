using UnityEngine;

// A simple script to handle the first load from main menu
// and making sure functions are executed in the right order
// so everything needed is actually loaded
public class FirstLoadManager : MonoBehaviour
{
    [Header("Other Scripts")]
    public CardInventory CardInventoryScript;

    // Start is called before the first frame update
    void Start()
    {
        CardInventoryScript.GetCardTypes();
    }
}
