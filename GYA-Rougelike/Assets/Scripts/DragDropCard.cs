using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class DragDropCard : MonoBehaviour
{
    public bool isDragging;

    [Header("Stuff for snapping")]
    public Transform MoveQueueSnapPointsParent;
    public Transform[] MoveQueueSnapPoints;
    public double[] SnapPointsDistance;

    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Object[] temp = Resources.FindObjectsOfTypeAll(typeof(GameObject));
        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i].name == "MoveQueue SnapPoints")
            {
                GameObject temp2 = (GameObject)temp[i];
                MoveQueueSnapPointsParent = temp2.transform;
                break;
            }
        }
        MoveQueueSnapPoints = MoveQueueSnapPointsParent.GetComponentsInChildren<Transform>();

        SnapPointsDistance = new double[MoveQueueSnapPoints.Length - 1];
    }

    public void OnMouseDown()
    {
        isDragging = true;
    }

    public void OnMouseUp()
    {
        isDragging = false;

        // Find closest SnapPoint and snap to it
        double ClosestDistance = SnapPointsDistance.Min();
        for (int i = 1; i < MoveQueueSnapPoints.Length; i++)
        {
            if (SnapPointsDistance[i - 1] == ClosestDistance)
            {
                // Snap to closest point
                gameObject.transform.position = MoveQueueSnapPoints[i].position;
                break;
            }
        }
    }

    public void OnMouseDrag()
    {
        // Update sprite position
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        transform.Translate(mousePosition);

        // Calculate position to each MoveQueue SnapPoint
        for (int i = 1; i < MoveQueueSnapPoints.Length; i++)
        {
            // Calc delta x & delta y
            float dX = Math.Abs(gameObject.transform.position.x - MoveQueueSnapPoints[i].position.x);
            float dY = Math.Abs(gameObject.transform.position.y - MoveQueueSnapPoints[i].position.y);

            // Calc actual distance
            SnapPointsDistance[i - 1] = Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));
        }
    }
}
