using Unity.VisualScripting;
using UnityEngine;

public class DragDropCardComponent : MonoBehaviour
{
    [Header("Camera")]
    public Camera RoomViewCamera;

    [Header("Variables")]
    // Int for which point the Card has snapped to
    public int SnappedToPoint;

    [Header("Other Scripts")]
    public DragDropCardManager DragDropCardManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Object[] temp = Resources.FindObjectsOfTypeAll(typeof(GameObject));
        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i].name == "RoomViewCamera")
            {
                RoomViewCamera = temp[i].GetComponent<Camera>();
            }
        }

        DragDropCardManagerScript = FindObjectOfType<DragDropCardManager>();
    }

    public void OnMouseDown()
    {
        DragDropCardManagerScript.RemoveFromPoint(transform, SnappedToPoint);
    }

    public void OnMouseUp()
    {
        SnappedToPoint = DragDropCardManagerScript.SnapToPoint(transform);
    }

    public void OnMouseDrag()
    {
        // Update sprite position
        Vector2 mousePosition = RoomViewCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        transform.Translate(mousePosition);
    }
}
