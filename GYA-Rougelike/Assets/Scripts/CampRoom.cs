using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampRoom : MonoBehaviour
{
    public GameObject CampRoomCanvas;

    public Button RestButton;

    public GameObject[] Players;

    public PullMapUpDown ViewSwitchScript;

    // Start is called before the first frame update
    void Start()
    {
        RestButton.onClick.AddListener(DoRest);

        ViewSwitchScript = FindAnyObjectByType<PullMapUpDown>();
    }

    public void EnterRoom()
    {
        CampRoomCanvas.SetActive(true);
    }

    void ExitRoom()
    {
        CampRoomCanvas.SetActive(false);

        ViewSwitchScript.SetViewMap();
    }

    void DoRest()
    {
        foreach (GameObject Player in Players)
        {
            if (Player != null)
            {
                HealthSystem HealthSystemScript = Player.GetComponent<HealthSystem>();

                // Add back 10 health
                HealthSystemScript.Heal(10);
            }
        }

        ExitRoom();
    }
}
