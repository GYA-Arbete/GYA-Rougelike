using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Script for controlling the different bars showing amount of an int
// Most off them being EnergyBar or HealthBar

public class BarScript : MonoBehaviour
{
    public TextMeshProUGUI Text;

    public void UpdateBar(int Amount, int MaxAmount)
    {
        Slider slider = transform.GetComponent<Slider>();
        slider.value = Amount / MaxAmount;

        Text.text = $"{Amount} / {MaxAmount}";
    }
}
