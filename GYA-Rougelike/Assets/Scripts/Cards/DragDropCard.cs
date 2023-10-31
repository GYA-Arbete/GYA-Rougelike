using System;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class DragDropCard : MonoBehaviour
{
    [Header("Camera")]
    public Camera RoomViewCamera;

    [Header("Variables for Snapping")]
    public bool InMoveQueue;
    public double[] SnapPointsDistance;

    [Header("SnapPoints")]
    public Transform MoveQueueSnapPointsParent;
    public Transform CardViewSnapPointsParent;
    public Transform[] SnapPoints;

    [Header("Other Scripts")]
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
            else if (temp[i].name == "RoomViewCamera")
            {
                RoomViewCamera = temp[i].GetComponent<Camera>();
            }
            else if (temp[i].name == "EnergyBar")
            {
                EnergyBarScript = temp[i].GetComponent<BarScript>();
            }
        }
        Transform[] Temp1 = MoveQueueSnapPointsParent.GetComponentsInChildren<Transform>();
        Transform[] Temp2 = CardViewSnapPointsParent.GetComponentsInChildren<Transform>();

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

        ThisCardsStats = GetComponent<CardStats>();
    }

    public void OnMouseDown()
    {
        if (InMoveQueue)
        {
            // If removing card from MoveQueue, add back the energy
            EnergyBarScript.UpdateBar(ThisCardsStats.Energy);

            InMoveQueue = false;
        }
    }

    public void OnMouseUp()
    {
        // Calculate position to each MoveQueue SnapPoint
        for (int i = 0; i < SnapPointsDistance.Length; i++)
        {
            // Calc delta x & delta y
            float dX = Math.Abs(gameObject.transform.position.x - SnapPoints[i].position.x);
            float dY = Math.Abs(gameObject.transform.position.y - SnapPoints[i].position.y);

            // Calc actual distance
            SnapPointsDistance[i] = Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));
        }

        // Find closest SnapPoint
        int ClosestDistanceIndex = Array.IndexOf(SnapPointsDistance, SnapPointsDistance.Min());

        // Snap to the closest SnapPoint
        gameObject.transform.position = SnapPoints[ClosestDistanceIndex].position;

        // If in the upper half of the screen, eg in MoveQueue
        if (transform.position.y > -9)
        {
            // Update EnergyBar to match (Set to negative to reduce TotalEnergy)
            EnergyBarScript.UpdateBar(-ThisCardsStats.Energy);

            InMoveQueue = true;
        }
    }

    public void OnMouseDrag()
    {
        // Update sprite position
        Vector2 mousePosition = RoomViewCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        transform.Translate(mousePosition);
    }
}
