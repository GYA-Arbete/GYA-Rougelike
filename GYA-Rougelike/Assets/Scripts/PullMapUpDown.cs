using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PullMapUpDown : MonoBehaviour
{
    [Header("Buttons")]

    public Button PullMapUpDownButton;

    [Header("Map Objects")]

    public Transform MapObjectsParent;

    // Private as it's not helpful to see in editor but declared here so everything in script can access it
    private bool IsDown = false;

    // Start is called before the first frame update
    void Start()
    {
        PullMapUpDownButton.onClick.AddListener(MapUpDown);
    }

    void MapUpDown()
    {
        if (!IsDown)
        {
            Transform[] MapObjects = GetComponentsInChildren<Transform>();

            foreach (Transform obj in MapObjects)
            {
                // Check if obj is a text object by seeing if it has a text component
                // We do this since all text objects will otherwise get moved from their parent objects
                if (obj.GetComponent<TextMeshProUGUI>() == null)
                {
                    obj.position = new Vector3(obj.position.x, obj.position.y - 5, obj.position.z); // * Time.deltaTime
                }
            }

            MapObjects = null;

            IsDown = true;
        }
        else
        {
            Transform[] MapObjects = GetComponentsInChildren<Transform>();

            foreach (Transform obj in MapObjects) 
            {
                // Check if obj is a text object by seeing if it has a text component
                // We do this since all text objects will otherwise get moved from their parent objects
                if (obj.GetComponent<TextMeshProUGUI>() == null)
                {
                    obj.position = new Vector3(obj.position.x, obj.position.y + 5, obj.position.z); // * Time.deltaTime
                }
            }

            MapObjects = null;
            IsDown = false;
        }
    }
}
