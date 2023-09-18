using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PullMapUpDown : MonoBehaviour
{
    [Header("Buttons")]

    public Button PullMapUpDownButton;

    [Header("Main Objects")]

    public Transform MainCamera;

    [Header("Dont Move Canvases")]

    public Transform DontMoveCanvas1;
    public Transform[] DontMoveCanvasElements1;

    [Space]

    public Transform DontMoveCanvas2;
    public Transform[] DontMoveCanvasElements2;

    [Header("Modifiers")]

    public float MoveDistance;

    // Private as it's not helpful to see in editor but declared here so everything in script can access it
    private bool IsDown = true;

    // Start is called before the first frame update
    void Start()
    {
        PullMapUpDownButton.onClick.AddListener(MapUpDown);
    }

    public void MapUpDown()
    {
        // Put into arrays every element that "shouldnt me moved", eg will be moved won as much as camera is moved up
        DontMoveCanvasElements1 = DontMoveCanvas1.GetComponentsInChildren<Transform>();
        DontMoveCanvasElements2 = DontMoveCanvas2.GetComponentsInChildren<Transform>();

        // Here we pan the camera upp/down while carying some elements with it
        // We do this since we want simple code and less items to do the math to move
        if (!IsDown)
        {
            MainCamera.position = new Vector3(MainCamera.position.x, MainCamera.position.y + MoveDistance, MainCamera.position.z); // * Time.deltaTime

            // Move the Canvas elements in opposite direction of Camera, gives ilussion of canvas not moving while keeping it tied to MainCamera
            // Ignorerar i == 0 eftersom det är parent och inte behöver flyttas
            for (int i = 1; i < DontMoveCanvasElements1.Length; i++)
            {
                DontMoveCanvasElements1[i].position = new Vector3(DontMoveCanvasElements1[i].position.x, DontMoveCanvasElements1[i].position.y - MoveDistance, DontMoveCanvasElements1[i].position.z); // * Time.deltaTime
            }

            for (int i = 1; i < DontMoveCanvasElements2.Length; i++)
            {
                // Om Elementet inte har kombonentet för text, eg inte är text
                if (DontMoveCanvasElements2[i].GetComponent<TextMeshProUGUI>() == null)
                {
                    DontMoveCanvasElements2[i].position = new Vector3(DontMoveCanvasElements2[i].position.x, DontMoveCanvasElements2[i].position.y - MoveDistance, DontMoveCanvasElements2[i].position.z); // * Time.deltaTime
                }
            }

            IsDown = true;
        }
        else
        {
            MainCamera.position = new Vector3(MainCamera.position.x, MainCamera.position.y - MoveDistance, MainCamera.position.z); // * Time.deltaTime

            // Move the Canvas in opposite direction of Camera, gives ilussion of canvas not moving while keeping it tied to MainCamera
            // Ignorerar i == 0 eftersom det är parent och inte behöver flyttas
            for (int i = 1; i < DontMoveCanvasElements1.Length; i++)
            {
                DontMoveCanvasElements1[i].position = new Vector3(DontMoveCanvasElements1[i].position.x, DontMoveCanvasElements1[i].position.y + MoveDistance, DontMoveCanvasElements1[i].position.z); // * Time.deltaTime
            }

            for (int i = 1; i < DontMoveCanvasElements2.Length; i++)
            {
                // Om Elementet inte har kombonentet för text, eg inte är text
                if (DontMoveCanvasElements2[i].GetComponent<TextMeshProUGUI>() == null)
                {
                    DontMoveCanvasElements2[i].position = new Vector3(DontMoveCanvasElements2[i].position.x, DontMoveCanvasElements2[i].position.y + MoveDistance, DontMoveCanvasElements2[i].position.z); // * Time.deltaTime
                }
            }

            IsDown = false;
        }
    }
}
