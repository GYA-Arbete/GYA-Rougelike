using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

// Script for showing info about card when hovering above the text-area
// Should be attached to the text-area object
public class HoverCardManager : MonoBehaviour
{
    public GameObject CardInfoHolder;
    public TextMeshProUGUI InfoTextElement;

    [Header("Camera")]
    public Camera RoomViewCamera;

    [Header("Other Scripts")]
    public CardManager CardManagerScript;

    private string InfoText = "";

    public void Setup(string infoText)
    {
        InfoText = infoText;

        UnityEngine.Object[] temp = Resources.FindObjectsOfTypeAll(typeof(GameObject));
        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i].name == "RoomViewCamera")
            {
                RoomViewCamera = temp[i].GetComponent<Camera>();
            }
        }

        CardManagerScript = FindObjectOfType<CardManager>();
    }

    // When hover over cards text-area, show info about card
    public void OnMouseOver()
    {
        CardInfoHolder.SetActive(true);
    }
}
