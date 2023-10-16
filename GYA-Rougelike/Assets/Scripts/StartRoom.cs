using UnityEngine;
using UnityEngine.UI;

public class StartRoom : MonoBehaviour
{
    public GameObject[] ElementsToHide;

    public GameObject StartChoice;
    public Button[] ChoiceButtons;

    public PullMapUpDown ViewSwitchScript;

    // Start is called before the first frame update
    void Start()
    {
        ChoiceButtons[0].onClick.AddListener(Choose1);
        ChoiceButtons[1].onClick.AddListener(Choose2);
        ChoiceButtons[2].onClick.AddListener(Choose3);

        EnterStartRoom();
    }

    public void EnterStartRoom()
    {
        foreach (GameObject Element in ElementsToHide)
        {
            Element.SetActive(false);
        }

        StartChoice.SetActive(true);

        ViewSwitchScript.SetViewRoom();
    }

    void ExitStartRoom()
    {
        foreach (GameObject Element in ElementsToHide)
        {
            Element.SetActive(true);
        }

        StartChoice.SetActive(false);

        ViewSwitchScript.SetViewMap();
    }

    void Choose1()
    {
        Debug.Log("Chooooose 1");
        ExitStartRoom();
    }

    void Choose2()
    {
        Debug.Log("Chooooose 2");
        ExitStartRoom();
    }

    void Choose3()
    {
        Debug.Log("Chooooose 3");
        ExitStartRoom();
    }
}
