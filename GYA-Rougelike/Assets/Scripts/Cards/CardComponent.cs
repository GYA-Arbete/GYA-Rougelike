using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CardComponent : MonoBehaviour
{
    [Header("Stuff for CardInfo")]
    public GameObject CardInfoHolder;
    public TextMeshProUGUI InfoTextElement;

    [Header("Camera")]
    public Camera RoomViewCamera;

    [Header("Variables")]
    // Int for which point the Card has snapped to
    public int SnappedToPoint;
    public bool Dragging;

    [Header("Other Scripts")]
    public CardManager CardManagerScript;
    public CombatSystem CombatSystemScript;

    public void Setup(int SnappedPoint, string InfoText)
    {
        UnityEngine.Object[] temp = Resources.FindObjectsOfTypeAll(typeof(GameObject));
        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i].name == "RoomViewCamera")
            {
                RoomViewCamera = temp[i].GetComponent<Camera>();
            }
        }

        CardManagerScript = FindObjectOfType<CardManager>();
        CombatSystemScript = FindObjectOfType<CombatSystem>();

        SnappedToPoint = SnappedPoint;
        InfoTextElement.text = InfoText;
        Dragging = false;
    }

    public void OnMouseDown()
    {
        CardManagerScript.RemoveFromPoint(transform, SnappedToPoint);
    }

    public void OnMouseUp()
    {
        Dragging = false;

        SnappedToPoint = CardManagerScript.SnapToPoint(transform, SnappedToPoint);

        CardManagerScript.HideTargetIndicator();
    }

    public void OnMouseDrag()
    {
        Dragging = true;

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

        CardManagerScript.DrawTargetIndicator(Points);
    }

    public void OnMouseOver()
    {
        // Dont show CardInfo when dragging card or when in MoveQueue (top edge of CardView object is y = -12, hence the value)
        if (Dragging || transform.position.y > -12)
        {
            CardInfoHolder.SetActive(false);
        }
        else
        {
            CardInfoHolder.SetActive(true);
        }
    }

    public void OnMouseExit()
    {
        CardInfoHolder.SetActive(false);
    }
}
