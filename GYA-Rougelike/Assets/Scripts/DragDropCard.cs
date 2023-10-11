using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class DragDropCard : MonoBehaviour
{
    public bool isDragging;
    public bool InMoveQueue;

    [Header("Stuff for snapping")]
    public Transform MoveQueueSnapPointsParent;
    public Transform CardViewSnapPointsParent;
    public Transform[] SnapPoints;
    public double[] SnapPointsDistance;

    public Transform[] Temp1;
    public Transform[] Temp2;

    [Header("EnergyBar stuff")]
    public BarScript EnergyBarScript;
    public CardStats ThisCardsStats;

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
            }
            else if (temp[i].name == "CardSpawner")
            {
                GameObject temp2 = (GameObject)temp[i];
                CardViewSnapPointsParent = temp2.transform;
            }
        }
        Temp1 = MoveQueueSnapPointsParent.GetComponentsInChildren<Transform>();
        Temp2 = CardViewSnapPointsParent.GetComponentsInChildren<Transform>();

        SnapPoints = new Transform[Temp1.Length + Temp2.Length - 2];

        // Merge these arrays into one
        // Also remove the parents
        for (int i = 1; i < Temp1.Length; i++)
        {
            SnapPoints[i - 1] = Temp1[i];
        }
        for (int i = 1; i < Temp2.Length; i++)
        {
            if (Temp2[i].name != "CardSpawner")
            {
                SnapPoints[Temp1.Length + i - 2] = Temp2[i];
            }
        }

        SnapPointsDistance = new double[SnapPoints.Length];

        EnergyBarScript = FindObjectOfType<BarScript>();
        ThisCardsStats = GetComponent<CardStats>();
    }

    public void OnMouseDown()
    {
        isDragging = true;

        if (InMoveQueue)
        {
            // Update EnergyBar to match
            EnergyBarScript.UpdateBar(ThisCardsStats.Energy);

            InMoveQueue = false;
        }
    }

    public void OnMouseUp()
    {
        isDragging = false;

        // Find closest SnapPoint and snap to it
        double ClosestDistance = SnapPointsDistance.Min();
        for (int i = 0; i < SnapPointsDistance.Length; i++)
        {
            if (SnapPointsDistance[i] == ClosestDistance)
            {
                // Snap to closest point
                gameObject.transform.position = SnapPoints[i].position;

                if (i < 15)
                {
                    // Update EnergyBar to match (Set to negative to reduce TotalEnergy)
                    EnergyBarScript.UpdateBar(-ThisCardsStats.Energy);

                    InMoveQueue = true;
                }
                
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
        for (int i = 0; i < SnapPointsDistance.Length; i++)
        {
            // Calc delta x & delta y
            float dX = Math.Abs(gameObject.transform.position.x - SnapPoints[i].position.x);
            float dY = Math.Abs(gameObject.transform.position.y - SnapPoints[i].position.y);

            // Calc actual distance
            SnapPointsDistance[i] = Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));
        }
    }
}
