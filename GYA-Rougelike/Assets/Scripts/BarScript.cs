using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Script for controlling the different bars showing amount of an int
// Most off them being EnergyBar or HealthBar

public class BarScript : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public Slider Slider;

    public int CurrentValue;
    public int MaxValue;

    [Header("Stuff for showing BarValue")]
    public Color FillColor;
    public Image Fill;

    private void Start()
    {
        Slider = gameObject.GetComponent<Slider>();   
    }

    public void SetupBar(int MaxVal, Color Color)
    {
        Slider = gameObject.GetComponent<Slider>();

        MaxValue = MaxVal;
        FillColor = Color;

        ResetBar();
    }

    public void ResetBar()
    {
        CurrentValue = MaxValue;

        Slider.maxValue = MaxValue;
        Slider.value = CurrentValue;

        Text.text = $"{CurrentValue} / {MaxValue}";
    }

    public void UpdateBar(int ChangingValue)
    {
        // Send positive value to add to CurrentValue, Send negative value to remove from CurrentValue

        CurrentValue += ChangingValue;

        if (CurrentValue > MaxValue)
        {
            CurrentValue = MaxValue;
        }

        Slider.value = CurrentValue;

        Fill.color = FillColor;

        Text.text = $"{CurrentValue} / {MaxValue}";
    }
}
