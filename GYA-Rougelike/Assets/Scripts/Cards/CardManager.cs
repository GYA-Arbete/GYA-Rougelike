using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    public GameObject TargetLineRend;

    [Header("Variables for Snapping")]
    public double[] SnapPointsDistance;
    public bool[] CardInPoint;

    [Header("SnapPoints")]
    public Transform CardViewSnapPointsParent;
    public Transform MoveQueueSnapPointsParent;
    public Transform[] SnapPoints;

    [Header("Stuff for CardInfoText")]
    public TextAsset CardInfoTextJson;
    public CardList CardTypesText;

    [Header("Other Scripts")]
    public BarScript EnergyBarScript;

    [System.Serializable]
    public class CardList
    {
        public List<Strings> cardList;
    }

    [System.Serializable]
    public class Strings
    {
        public string CardInfo;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get all SnapPoints
        Transform[] Temp1 = CardViewSnapPointsParent.GetComponentsInChildren<Transform>();
        Transform[] Temp2 = MoveQueueSnapPointsParent.GetComponentsInChildren<Transform>();

        // Create one array to merge these 2 arrays
        SnapPoints = new Transform[Temp1.Length + Temp2.Length - 2];

        // Merge these arrays into one
        // Also remove the parents
        for (int i = 1; i < Temp1.Length; i++)
        {
            SnapPoints[i - 1] = Temp1[i];
        }
        for (int i = 1; i < Temp2.Length; i++)
        {
            if (Temp2[i].name != "MoveQueue SnapPoints")
            {
                SnapPoints[Temp1.Length + i - 2] = Temp2[i];
            }
        }

        SnapPointsDistance = new double[SnapPoints.Length];
        CardInPoint = new bool[SnapPoints.Length];

        CardTypesText = JsonUtility.FromJson<CardList>(CardInfoTextJson.text);
    }

    public void SetCardInPoint(List<int> Indexes)
    {
        foreach (int Index in Indexes)
        {
            CardInPoint[Index] = true;
        }
    }

    public void RemoveFromPoint(Transform Card, int SnappedToPoint)
    {
        // If in MoveQueue
        if (Card.position.y > -9)
        {
            // If removing card from MoveQueue, add back the energy
            EnergyBarScript.UpdateBar(Card.GetComponent<CardStats>().Energy);
        }

        CardInPoint[SnappedToPoint] = false;
    }

    public int SnapToPoint(Transform Card)
    {
        // Calculate position to each SnapPoint
        for (int i = 0; i < SnapPoints.Length; i++)
        {
            // Calc delta x & delta y
            float dX = Math.Abs(Card.position.x - SnapPoints[i].position.x);
            float dY = Math.Abs(Card.position.y - SnapPoints[i].position.y);

            // Calc actual distance
            SnapPointsDistance[i] = Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));
        }

        // Find closest SnapPoint
        int ClosestDistanceIndex = Array.IndexOf(SnapPointsDistance, SnapPointsDistance.Min());

        int SnappedPoint = 0;

        // Snap the card to the non-occupied SnapPoint as far left as possible
        // If in CardView
        if (ClosestDistanceIndex < 15)
        {
            for (int i = 0; i <= ClosestDistanceIndex; i++)
            {
                if (CardInPoint[i] == false)
                {
                    // Snap to the first non-occupied SnapPoint
                    Card.position = SnapPoints[i].position;

                    CardInPoint[i] = true;
                    SnappedPoint = i;
                    
                    // Jump out of for loop
                    break;
                }
            }
        }
        // If in MoveQueue
        else
        {
            for (int i = 15;  i <= ClosestDistanceIndex; i++)
            {
                if (CardInPoint[i] == false)
                {
                    // Snap to the first non-occupied SnapPoint
                    Card.position = SnapPoints[i].position;

                    CardInPoint[i] = true;
                    SnappedPoint = i;

                    // Jump out of for loop
                    break;
                }
            }
        }

        // If in the upper half of the screen, eg in MoveQueue
        if (Card.position.y > -9)
        {
            // Update EnergyBar to match (Set to negative to reduce TotalEnergy)
            EnergyBarScript.UpdateBar(-Card.GetComponent<CardStats>().Energy);
        }

        return SnappedPoint;
    }

    public void HideTargetIndicator()
    {
        TargetLineRend.SetActive(false);
    }

    public void DrawTargetIndicator(Vector3[] Points)
    {
        TargetLineRend.SetActive(true);

        LineRenderer LineRend = TargetLineRend.GetComponent<LineRenderer>();

        LineRend.positionCount = 50;

        int SegmentIndex = 0;

        for (int i = 0; i < 100; i += 2)
        {
            float t = (float)i / 100;

            Vector3 Position = CalcBezierPoint(Points, t);

            LineRend.SetPosition(SegmentIndex, Position);

            SegmentIndex++;
        }

        // Set texture scale to avoid texture stretching
        LineRend.material.mainTextureScale = new Vector2(1f / LineRend.startWidth, 1.0f);
    }

    Vector3 CalcBezierPoint(Vector3[] Points, float t)
    {
        float u = 1 - t;

        Vector3 Point = u * u * Points[0];
        Point += 2 * u * t * Points[1];
        Point += t * t * Points[2];

        return Point;
    }
}
