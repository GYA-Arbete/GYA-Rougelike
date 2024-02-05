using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMenuUI : MonoBehaviour
{
    public Canvas MainCanvas;

    public void SetLocalPlayer()
    {
        MainCanvas.gameObject.SetActive(true);
    }
}
