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

    public int ID;

    // Start is called before the first frame update
    void Start()
    {
        Toggle.onValueChanged.AddListener(delegate { ValueChanged(Toggle, null); });

        // Call function to make sure the correct background color is set on start
        ValueChanged(Toggle, null);
    }

    public void ValueChanged(Toggle change, bool? ToggleValueOverride)
    {
        if (ToggleValueOverride != null)
        {
            Toggle.isOn = (bool)ToggleValueOverride;
        }

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
