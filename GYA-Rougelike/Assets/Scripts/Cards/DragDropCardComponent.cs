using System;
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
    public CombatSystem CombatSystemScript;

    public void Setup(int SnappedPoint)
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
        CombatSystemScript = FindObjectOfType<CombatSystem>();

        SnappedToPoint = SnappedPoint;
    }

    public void OnMouseDown()
    {
        DragDropCardManagerScript.RemoveFromPoint(transform, SnappedToPoint);
    }

    public void OnMouseUp()
    {
        SnappedToPoint = DragDropCardManagerScript.SnapToPoint(transform);

        DragDropCardManagerScript.HideTargetIndicator();
    }

    public void OnMouseDrag()
    {
        // Update sprite position
        Vector2 mousePosition = RoomViewCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        transform.Translate(mousePosition);

        // Update target indicator
        Vector3[] Points = new Vector3[3];

        float Z = 2;

        // Calculate each point of line
        Points[0] = transform.position; // Card position
        Points[0].z = Z;

        float YOffset = 0.8f;
        float X2 = Math.Min(transform.position.x, CombatSystemScript.EnemyTarget.position.x) + Math.Abs((transform.position.x - CombatSystemScript.EnemyTarget.position.x) / 2);
        float Y2 = Math.Min(transform.position.y, CombatSystemScript.EnemyTarget.position.y) + YOffset + Math.Abs((transform.position.y - CombatSystemScript.EnemyTarget.position.y) / 2);
        Points[1] = new Vector3(X2, Y2, Z); // Middle point

        Points[2] = CombatSystemScript.EnemyTarget.position; // Enemy position
        Points[2].z = Z;

        DragDropCardManagerScript.DrawTargetIndicator(Points);
    }
}
