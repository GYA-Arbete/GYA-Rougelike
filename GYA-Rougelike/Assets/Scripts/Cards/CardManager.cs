using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    public GameObject[] TargetIndicators;
    public Transform[] PlayerPositions;

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

    [Serializable]
    public class CardList
    {
        public List<Strings> cardList;
    }

    [Serializable]
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

    public int SnapToPoint(Transform Card, int StartPoint)
    {
        // If Energy == 0
        if (EnergyBarScript.CurrentValue == 0)
        {
            // Snap to point where card was previously
            Card.position = SnapPoints[StartPoint].position;

            // Return the point where card was previously
            return StartPoint;
        }

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
        foreach (GameObject TargetIndicator in TargetIndicators)
        {
            TargetIndicator.SetActive(false);
        }
    }

    public void DrawTargetIndicator(Vector3[] Points, int Type)
    {
        // CardTypes that affect both players
        if (Type == 1 || Type == 2 || Type == 6)
        {
            TargetIndicators[0].SetActive(false);
            TargetIndicators[1].SetActive(true);
            TargetIndicators[2].SetActive(true);
        }
        // CardTypes that affect both players and enemy
        else if (Type == 5)
        {
            TargetIndicators[0].SetActive(true);
            TargetIndicators[1].SetActive(true);
            TargetIndicators[2].SetActive(true);
        }
        // CardTypes that affect enemy
        else
        {
            TargetIndicators[0].SetActive(true);
            TargetIndicators[1].SetActive(false);
            TargetIndicators[2].SetActive(false);
        }

        for (int i = 0; i < TargetIndicators.Length; i++)
        {
            // Calculate line for each LineRend that is enabled
            if (TargetIndicators[i].activeSelf)
            {
                LineRenderer LineRend = TargetIndicators[i].GetComponent<LineRenderer>();
                LineRend.positionCount = 50;

                int SegmentIndex = 0;

                // If PlayerTarget recalc some points
                if (i > 0)
                {
                    float Z = 2;
                    float YOffset = 0.8f;
                    float NewX = Math.Min(Points[0].x, PlayerPositions[i - 1].position.x) + Math.Abs((Points[0].x - PlayerPositions[i - 1].position.x) / 2);
                    float NewY = Math.Min(Points[0].y, PlayerPositions[i - 1].position.y) + YOffset + Math.Abs((Points[0].y - PlayerPositions[i - 1].position.y) / 2);
                    Points[1] = new(NewX, NewY, Z);
                    Points[2] = new(PlayerPositions[i - 1].position.x, PlayerPositions[i - 1].position.y, Z);
                }

                for (int j = 0; j < 100; j += 2)
                {
                    float t = (float)j / 100;

                    Vector3 Position = CalcBezierPoint(Points, t);

                    LineRend.SetPosition(SegmentIndex, Position);

                    SegmentIndex++;
                }

                // Set texture scale to avoid texture stretching
                LineRend.material.mainTextureScale = new Vector2(1f / LineRend.startWidth, 1.0f);
            }
        }
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
