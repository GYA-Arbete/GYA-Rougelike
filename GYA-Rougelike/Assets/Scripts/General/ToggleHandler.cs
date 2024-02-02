using UnityEngine;
using UnityEngine.UI;


// Custom script for setting background for Toggle as I didnt like the built in Unity way
public class ToggleHandler : MonoBehaviour
{
    [Header("Options")]
    public Color OnColor;
    public Color OffColor;

    [Header("Elements")]
    public Toggle Toggle;
    public Image Background;

    // Start is called before the first frame update
    void Start()
    {
        Toggle.onValueChanged.AddListener(delegate { ValueChanged(Toggle); });

        // Call function to make sure the correct background color is set on start
        ValueChanged(Toggle);
    }

    void ValueChanged(Toggle change)
    {
        if (Toggle.isOn)
        {
            Background.color = OnColor;
        }
        else
        {
            Background.color = OffColor;
        }
    }
}
